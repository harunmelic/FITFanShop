import { Component, OnInit } from '@angular/core';
import { ImageApiService } from '../../../api-services/images/image-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

interface UploadedImage {
  id: number;
  name: string;
  url: string;
  size: number;
  uploadDate: Date;
  source: 'api' | 'localStorage'; // Track source for debugging
}

/**
 * Pictures Management Component - Hybrid Version
 * 
 * This version intelligently tries backend API first, but falls back to localStorage:
 * 
 * BACKEND API (preferred):
 * - POST /api/images/upload - Upload image files  
 * - GET /api/images - Fetch all images
 * - DELETE /api/images/:id - Delete image
 * 
 * LOCALSTORAGE FALLBACK:
 * - Works immediately without backend
 * - 5MB browser storage limit
 * - Images lost when clearing browser data
 * 
 * Auto-switches to backend when endpoints become available!
 */
@Component({
  selector: 'app-pictures',
  templateUrl: './pictures.component.html',
  styleUrls: ['./pictures.component.scss'],
  standalone: false
})
export class PicturesHybridComponent implements OnInit {
  images: UploadedImage[] = [];
  isDragging = false;
  isUploading = false;
  uploadProgress: number = 0;
  storageMode: 'api' | 'localStorage' = 'localStorage'; // Current active mode
  backendAvailable = false;

  private readonly STORAGE_KEY = 'admin_images';

  constructor(
    private imageApiService: ImageApiService,
    private toasterService: ToasterService
  ) { }

  ngOnInit(): void {
    console.log('=== PICTURES HYBRID COMPONENT INIT ===');
    this.detectBackendAndLoad();
  }

  /**
   * Smart backend detection and image loading  
   */
  private detectBackendAndLoad(): void {
    console.log('🔍 Testing backend availability...');
    
    // First try to load from backend
    this.imageApiService.getAllImages().subscribe({
      next: (images) => {
        console.log('✅ Backend API available! Using server storage.');
        this.backendAvailable = true;
        this.storageMode = 'api';
        this.images = images.map(img => ({
          id: img.id,
          name: img.fileName,
          url: img.fileUrl,
          size: img.fileSize,
          uploadDate: new Date(img.uploadDate),
          source: 'api'
        }));
        
        // Test existing image URLs for static file issues
        this.testExistingImageUrls();
      },
      error: (error) => {
        console.log('❌ Backend API not available, using localStorage fallback');
        console.log(`Error: ${error.status} ${error.statusText}`);
        this.backendAvailable = false;
        this.storageMode = 'localStorage';
        this.loadFromLocalStorage();
        this.toasterService.info('Using browser storage 💾 (Backend will auto-connect when ready)');
      }
    });
  }

  /**
   * Test existing image URLs to detect static file serving issues
   */
  private testExistingImageUrls(): void {
    if (this.images.length === 0) {
      this.toasterService.success('Connected to server storage 🌐 - No images to load');
      return;
    }
    
    console.log(`🔍 Testing ${this.images.length} existing image URLs for static file access...`);
    let workingCount = 0;
    let brokenCount = 0;
    
    this.images.forEach((image, index) => {
      const img = new Image();
      img.onload = () => {
        workingCount++;
        console.log(`✅ Image ${index + 1}/${this.images.length} OK:`, image.url);
        
        if (workingCount + brokenCount === this.images.length) {
          this.reportImageAccessResults(workingCount, brokenCount);
        }
      };
      img.onerror = () => {
        brokenCount++;
        console.error(`❌ Image ${index + 1}/${this.images.length} BROKEN:`, image.url);
        console.error('Full URL that failed:', `https://localhost:7260${image.url}`);
        
        if (workingCount + brokenCount === this.images.length) {
          this.reportImageAccessResults(workingCount, brokenCount);
        }
      };
      img.src = image.url;
    });
  }

  /**
   * Report results of image URL testing
   */
  private reportImageAccessResults(working: number, broken: number): void {
    if (broken > 0) {
      console.error(`🚨 STATIC FILES ISSUE: ${broken}/${working + broken} images not accessible`);
      console.error('💡 SOLUTION: Add app.UseStaticFiles() to backend Program.cs');
      console.error('📚 See: BACKEND_STATIC_FILES_GUIDE.md for full setup');
      
      this.toasterService.error(
        `⚠️ ${broken} images not accessible. Backend needs static file serving configuration.`,
        8000
      );
    } else {
      console.log(`✅ All ${working} images accessible - static files working correctly`);
      this.toasterService.success(`Connected to server storage 🌐 - All ${working} images loaded!`);
    }
  }

  /**
   * Load images from localStorage
   */
  private loadFromLocalStorage(): void {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (stored) {
        const data = JSON.parse(stored);
        this.images = data.images.map((img: any) => ({
          ...img,
          uploadDate: new Date(img.uploadDate),
          source: 'localStorage'
        }));
        console.log(`📱 Loaded ${this.images.length} images from localStorage`);
      }
    } catch (error) {
      console.error('Error loading from localStorage:', error);
      this.images = [];
    }
  }

  /**
   * Save current images to localStorage
   */
  private saveToLocalStorage(): void {
    try {
      const data = {
        images: this.images.filter(img => img.source === 'localStorage'),
        lastUpdated: new Date().toISOString()
      };
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(data));
      console.log('💾 Saved to localStorage');
    } catch (error: unknown) {
      console.error('Error saving to localStorage:', error);
      if (error instanceof Error && error.name === 'QuotaExceededError') {
        this.toasterService.error('Storage quota exceeded! Please delete some images.');
      }
    }
  }

  // Drag & Drop handlers
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files) {
      this.handleFiles(files);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.handleFiles(input.files);
      input.value = '';
    }
  }

  handleFiles(files: FileList): void {
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    const maxSize = 10 * 1024 * 1024; // 10MB

    Array.from(files).forEach(file => {
      if (!allowedTypes.includes(file.type)) {
        this.toasterService.error(`File ${file.name} is not a valid image type. Allowed: JPEG, PNG, GIF, WebP`);
        return;
      }

      if (file.size > maxSize) {
        this.toasterService.error(`File ${file.name} is too large. Maximum size is 10MB.`);
        return;
      }

      // Smart upload: try API first, fallback to localStorage
      this.smartUpload(file);
    });
  }

  /**
   * Smart upload: API first, localStorage fallback
   */
  private smartUpload(file: File): void {
    if (this.backendAvailable) {
      this.uploadToAPI(file);
    } else {
      this.uploadToLocalStorage(file);
    }
  }

  /**
   * Upload to backend API
   */
  private uploadToAPI(file: File): void {
    console.log('⬆️ Uploading to API:', file.name);
    this.isUploading = true;
    
    this.imageApiService.uploadImage(file).subscribe({
      next: (response) => {
        console.log('✅ API upload response:', response);
        console.log('🔗 Generated image URL:', response.fileUrl);
        console.log('📁 File name from server:', response.fileName);
        console.log('📊 File size from server:', response.fileSize);
        
        const newImage: UploadedImage = {
          id: response.id,
          name: response.fileName,
          url: response.fileUrl,
          size: response.fileSize,
          uploadDate: new Date(),
          source: 'api'
        };
        this.images.push(newImage);
        this.toasterService.success(`Image ${file.name} uploaded to server! 🌐`);
        this.isUploading = false;
        
        // Test if the URL is actually accessible
        console.log('🧪 Testing if uploaded image URL is accessible...');
        this.testSingleImageUrl(response.fileUrl, file.name);
      },
      error: (error) => {
        console.log('❌ API upload failed, trying localStorage fallback...');
        this.backendAvailable = false;
        this.storageMode = 'localStorage';
        this.uploadToLocalStorage(file);
      }
    });
  }

  /**
   * Test single image URL accessibility
   */
  private testSingleImageUrl(url: string, fileName: string): void {
    console.log('🧪 Testing image URL accessibility...');
    console.log('📍 Relative URL from backend:', url);
    console.log('🌐 Full URL being tested:', `https://localhost:7260${url}`);
    console.log('💾 Expected backend file location:', `YourBackendProject${url.replace(/\//g, '\\')})`);
    
    const img = new Image();
    img.onload = () => {
      console.log('✅ GREAT! Newly uploaded image is accessible:', url);
      this.toasterService.success(`🌐 Image ${fileName} uploaded and accessible!`);
    };
    img.onerror = () => {
      console.error('❌ PROBLEM! Newly uploaded image is NOT accessible:', url);
      console.error('🔧 Backend uploaded file but static serving not working');
      console.error('💡 Auto-switching to localStorage mode until backend is fixed');
      
      // Auto-fallback: remove the broken image and switch to localStorage
      this.handleStaticFilesFallback(fileName);
    };
    img.src = url;
  }

  /**
   * Handle static files failure - switch to localStorage
   */
  private handleStaticFilesFallback(lastUploadedFileName: string): void {
    console.log('🔄 Switching to localStorage mode due to static files issue');
    
    // Remove the last uploaded image that can't be displayed
    const lastImage = this.images[this.images.length - 1];
    if (lastImage && lastImage.name.includes(lastUploadedFileName)) {
      this.images.pop();
      console.log('🗑️ Removed undisplayable image from UI');
    }
    
    // Switch to localStorage mode
    this.backendAvailable = false;
    this.storageMode = 'localStorage';
    
    // Load any existing localStorage images
    this.loadFromLocalStorage();
    
    this.toasterService.warning(
      '⚠️ Backend uploads work but static files not configured. Switched to browser storage. ' + 
      'Images will auto-sync to server when backend static files are fixed.',
      12000
    );
  }

  /**
   * Test basic static file access to see if configuration works at all
   */
  private testBasicStaticFileAccess(): void {
    console.log('🧪 Testing basic static file access...');
    
    // Try to access the uploads directory directly
    const testUrls = [
      '/uploads/',
      '/uploads/images/',
      '/uploads/test.txt'
    ];
    
    testUrls.forEach(testUrl => {
      fetch(`https://localhost:7260${testUrl}`)
        .then(response => {
          console.log(`📁 ${testUrl} → ${response.status} (${response.status === 200 ? 'OK' : 'Not OK'})`);
          if (response.status === 404) {
            console.error(`   ❌ Directory not served by backend static files`);
          } else if (response.status === 403) {
            console.log(`   ✅ Directory exists but browsing disabled (normal)`);
          } else {
            console.log(`   ✅ Static files working for this path`);
          }
        })
        .catch(error => {
          console.error(`❌ ${testUrl} → Network error:`, error);
        });
    });
  }

  /**
   * Upload to localStorage
   */
  private uploadToLocalStorage(file: File): void {
    console.log('📱 Uploading to localStorage:', file.name);
    this.isUploading = true;

    const reader = new FileReader();
    reader.onload = (e) => {
      const dataUrl = e.target?.result as string;
      const newImage: UploadedImage = {
        id: Date.now(), // Simple ID generation
        name: file.name,
        url: dataUrl,
        size: file.size,
        uploadDate: new Date(),
        source: 'localStorage'
      };
      
      this.images.push(newImage);
      this.saveToLocalStorage();
      this.toasterService.success(`Image ${file.name} saved locally! 💾`);
      this.isUploading = false;
    };

    reader.onerror = () => {
      this.toasterService.error(`Failed to process ${file.name}`);
      this.isUploading = false;
    };

    reader.readAsDataURL(file);
  }

  /**
   * Smart delete: API or localStorage  
   */
  deleteImage(image: UploadedImage): void {
    if (confirm(`Are you sure you want to delete ${image.name}?`)) {
      if (image.source === 'api' && this.backendAvailable) {
        this.deleteFromAPI(image);
      } else {
        this.deleteFromLocalStorage(image);
      }
    }
  }

  private deleteFromAPI(image: UploadedImage): void {
    this.imageApiService.deleteImage(image.id).subscribe({
      next: () => {
        this.images = this.images.filter(img => img.id !== image.id);
        this.toasterService.success(`${image.name} deleted from server! 🌐`);
      },
      error: (error) => {
        console.error('API delete failed:', error);
        this.toasterService.error(`Failed to delete ${image.name} from server`);
      }
    });
  }

  private deleteFromLocalStorage(image: UploadedImage): void {
    this.images = this.images.filter(img => img.id !== image.id);
    this.saveToLocalStorage();
    this.toasterService.success(`${image.name} deleted locally! 💾`);
  }

  /**
   * Test backend connection and sync localStorage images
   */
  testBackendConnection(): void {
    console.log('🔄 Testing backend connection and syncing localStorage images...');
    this.toasterService.info('Testing backend connection...');
    
    this.imageApiService.getAllImages().subscribe({
      next: (images) => {
        this.backendAvailable = true;
        this.storageMode = 'api';
        
        // Load server images first
        this.images = images.map(img => ({
          id: img.id,
          name: img.fileName,
          url: img.fileUrl,
          size: img.fileSize,
          uploadDate: new Date(img.uploadDate),
          source: 'api'
        }));
        
        this.toasterService.success('Backend connection successful! 🌐');
        console.log('✅ Backend is now available');
        
        // Test if static files work now
        this.testExistingImageUrls();
        
        // Sync localStorage images to server
        this.syncLocalStorageImagesToServer();
      },
      error: (error) => {
        this.backendAvailable = false;
        this.storageMode = 'localStorage';
        this.toasterService.error(`Backend still unavailable: ${error.status} ${error.statusText}`);
        console.log('❌ Backend still not available');
      }
    });
  }

  /**
   * Sync localStorage images to server when backend becomes available
   */
  private syncLocalStorageImagesToServer(): void {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (!stored) {
        console.log('📱 No localStorage images to sync');
        return;
      }
      
      const data = JSON.parse(stored);
      const localImages = data.images || [];
      
      if (localImages.length === 0) {
        console.log('📱 No localStorage images found');
        return;
      }
      
      console.log(`🔄 Found ${localImages.length} localStorage images to sync to server...`);
      this.toasterService.info(`Syncing ${localImages.length} local images to server...`);
      
      localImages.forEach((localImage: any, index: number) => {
        // Convert data URL back to blob and upload
        fetch(localImage.url)
          .then(res => res.blob())
          .then(blob => {
            const file = new File([blob], localImage.name, { type: blob.type });
            
            console.log(`📤 Syncing ${index + 1}/${localImages.length}: ${file.name}`);
            
            this.imageApiService.uploadImage(file).subscribe({
              next: (response) => {
                console.log(`✅ Synced ${file.name} to server`);
                
                // Add synced image to current array (avoid duplicates)
                if (!this.images.some(img => img.name === response.fileName)) {
                  const syncedImage: UploadedImage = {
                    id: response.id,
                    name: response.fileName,
                    url: response.fileUrl,
                    size: response.fileSize,
                    uploadDate: new Date(),
                    source: 'api'
                  };
                  this.images.push(syncedImage);
                }
                
                // Clear localStorage after all images are synced
                if (index === localImages.length - 1) {
                  localStorage.removeItem(this.STORAGE_KEY);
                  console.log('🧹 Cleared localStorage after successful sync');
                  this.toasterService.success(`🌐 All ${localImages.length} images synced to server successfully!`);
                }
              },
              error: (error) => {
                console.error(`❌ Failed to sync ${file.name}:`, error);
                this.toasterService.error(`Failed to sync ${file.name} to server`);
              }
            });
          })
          .catch(error => {
            console.error(`❌ Failed to convert localStorage image ${localImage.name}:`, error);
          });
      });
      
    } catch (error) {
      console.error('❌ Error syncing localStorage images:', error);
    }
  }

  /**
   * Clean broken images from backend database
   */
  cleanBrokenImages(): void {
    if (!confirm('This will delete all images with broken URLs from the server database. Continue?')) {
      return;
    }

    console.log('🧹 Testing and cleaning broken images...');
    this.toasterService.info('Testing image accessibility...');
    
    let brokenImages: UploadedImage[] = [];
    let processedCount = 0;

    this.images.forEach((image) => {
      const img = new Image();
      img.onload = () => {
        processedCount++;
        if (processedCount === this.images.length) {
          this.deleteBrokenImagesFromServer(brokenImages);
        }
      };
      img.onerror = () => {
        brokenImages.push(image);
        processedCount++;
        if (processedCount === this.images.length) {
          this.deleteBrokenImagesFromServer(brokenImages);
        }
      };
      img.src = image.url;
    });
  }

  private deleteBrokenImagesFromServer(brokenImages: UploadedImage[]): void {
    if (brokenImages.length === 0) {
      this.toasterService.success('✅ No broken images found - all URLs work correctly!');
      return;
    }

    console.log(`🗑️ Deleting ${brokenImages.length} broken images from server...`);
    
    const deletePromises = brokenImages.map(img => 
      this.imageApiService.deleteImage(img.id).toPromise()
    );

    Promise.all(deletePromises).then(() => {
      // Remove from local array
      this.images = this.images.filter(img => 
        !brokenImages.some(broken => broken.id === img.id)
      );
      
      this.toasterService.success(`✅ Cleaned ${brokenImages.length} broken images from server database`);
      console.log('✅ Broken images cleanup completed');
    }).catch(error => {
      console.error('❌ Error cleaning broken images:', error);
      this.toasterService.error('Failed to clean some broken images');
    });
  }

  /**
   * Debug backend static files configuration
   */
  debugBackendStaticFiles(): void {
    console.log('🛠️ === BACKEND STATIC FILES DIAGNOSIS ===');
    this.toasterService.info('Running static files diagnosis... Check console for details.');
    
    const tests = [
      // Test basic static file endpoints
      { url: '/uploads/', description: 'Uploads directory access' },
      { url: '/uploads/images/', description: 'Images subdirectory access' },
      
      // Test actual image files from current images
      ...this.images.slice(0, 2).map(img => ({
        url: img.url, 
        description: `Actual image: ${img.name}`
      }))
    ];
    
    console.log('🧪 Testing static file endpoints:');
    
    tests.forEach((test, index) => {
      setTimeout(() => {
        fetch(`https://localhost:7260${test.url}`)
          .then(response => {
            console.log(`${index + 1}. ${test.description}:`);
            console.log(`   URL: https://localhost:7260${test.url}`);
            console.log(`   Status: ${response.status} ${response.statusText}`);
            
            if (response.status === 200) {
              console.log(`   ✅ SUCCESS - File/directory accessible`);
            } else if (response.status === 403) {
              console.log(`   ⚠️ FORBIDDEN - Directory exists but browsing disabled (this is normal)`);
            } else if (response.status === 404) {
              console.log(`   ❌ NOT FOUND - Check backend static files configuration`);
              console.log(`   💡 Make sure Program.cs has:`);
              console.log(`      app.UseStaticFiles(new StaticFileOptions { ... });`);
            } else {
              console.log(`   ❓ UNEXPECTED STATUS - Check backend logs`);
            }
          })
          .catch(error => {
            console.error(`${index + 1}. ${test.description}: NETWORK ERROR`, error);
          });
      }, index * 500); // Stagger requests
    });
    
    // Print backend configuration reminder
    setTimeout(() => {
      console.log('📋 Expected backend configuration:');
      console.log('   1. In Program.cs, BEFORE app.UseRouting():');
      console.log('      app.UseStaticFiles(new StaticFileOptions');
      console.log('      {');
      console.log('          FileProvider = new PhysicalFileProvider(');
      console.log('              Path.Combine(Directory.GetCurrentDirectory(), "uploads")),');
      console.log('          RequestPath = "/uploads"');
      console.log('      });');
      console.log('   2. Backend restarted after changes');
      console.log('   3. uploads/images/ directory exists in backend project root');
    }, tests.length * 500 + 1000);
  }

  // Utility methods
  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  copyToClipboard(url: string): void {
    navigator.clipboard.writeText(url).then(() => {
      this.toasterService.success('Image URL copied to clipboard!');
    });
  }

  clearAllImages(): void {
    if (confirm('Are you sure you want to delete all images? This action cannot be undone.')) {
      if (this.storageMode === 'api') {
        // Clear API images
        const apiImages = this.images.filter(img => img.source === 'api');
        Promise.all(apiImages.map(img => this.imageApiService.deleteImage(img.id).toPromise()))
          .then(() => {
            this.images = this.images.filter(img => img.source !== 'api');
            this.saveToLocalStorage(); // Keep localStorage images
            this.toasterService.success('All server images deleted! 🌐');
          })
          .catch(error => {
            console.error('Error deleting API images:', error);
            this.toasterService.error('Failed to delete some server images');
          });
      } else {
        // Clear localStorage images
        this.images = [];
        localStorage.removeItem(this.STORAGE_KEY);
        this.toasterService.success('All local images deleted! 💾');
      }
    }
  }

  getTotalStorageSize(): number {
    return this.images.reduce((total, img) => total + img.size, 0);
  }

  getTotalStorageSizeFormatted(): string {
    return this.formatFileSize(this.getTotalStorageSize());
  }

  getStorageStats(): string {
    const apiCount = this.images.filter(img => img.source === 'api').length;
    const localCount = this.images.filter(img => img.source === 'localStorage').length;
    
    if (apiCount > 0 && localCount > 0) {
      return `${apiCount} server + ${localCount} local images`;
    } else if (apiCount > 0) {
      return `${apiCount} server images`;
    } else {
      return `${localCount} local images`;
    }
  }
}