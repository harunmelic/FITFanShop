import { Component, OnInit } from '@angular/core';
import { ImageApiService } from '../../../api-services/images/image-api.service';
import { ImageDto } from '../../../api-services/images/image-api.model';
import { ToasterService } from '../../../core/services/toaster.service';

interface UploadedImage {
  id: number;
  name: string;
  url: string;
  size: number;
  uploadDate: Date;
}

/**
 * Pictures Management Component - API Version
 * 
 * This version uses backend API for image storage:
 * - POST /api/images/upload - Upload image files
 * - GET /api/images - Fetch all images
 * - DELETE /api/images/:id - Delete image
 * 
 * Images are stored on server (disk/cloud) and metadata in database.
 * No localStorage limitations, images accessible from any device.
 */
@Component({
  selector: 'app-pictures',
  templateUrl: './pictures.component.html',
  styleUrls: ['./pictures.component.scss'],
  standalone: false
})
export class PicturesApiComponent implements OnInit {
  images: UploadedImage[] = [];
  isDragging = false;
  isUploading = false;
  uploadProgress: number = 0;

  constructor(
    private imageApiService: ImageApiService,
    private toasterService: ToasterService
  ) { }

  ngOnInit(): void {
    console.log('=== PICTURES API COMPONENT INIT ===');
    console.log('API URL:', this.imageApiService);
    this.loadImagesFromApi();
  }

  /**
   * Load images from backend API
   */
  private loadImagesFromApi(): void {
    console.log('Loading images from API...');
    this.imageApiService.getAllImages().subscribe({
      next: (images) => {
        console.log('✅ Images loaded successfully:', images);
        this.images = images.map(img => ({
          id: img.id,
          name: img.fileName,
          url: img.fileUrl,
          size: img.fileSize,
          uploadDate: new Date(img.uploadDate)
        }));
      },
      error: (error) => {
        console.error('❌ Error loading images:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url,
          body: error.error
        });
        
        if (error.status === 500) {
          this.toasterService.error('Backend server error. Check if ImagesController is implemented correctly.');
        } else if (error.status === 404) {
          this.toasterService.error('Images endpoint not found. Check if /api/images route exists.');
        } else if (error.status === 401) {
          this.toasterService.error('Authentication required. Please login as admin.');
        } else {
          this.toasterService.error(`Failed to load images: ${error.status} ${error.statusText}`);
        }
      }
    });
  }

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
      // Reset input so same file can be uploaded again
      input.value = '';
    }
  }

  handleFiles(files: FileList): void {
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    const maxSize = 10 * 1024 * 1024; // 10MB

    Array.from(files).forEach(file => {
      // Validate file type
      if (!allowedTypes.includes(file.type)) {
        this.toasterService.error(
          `File ${file.name} is not a valid image type. Allowed: JPEG, PNG, GIF, WebP`
        );
        return;
      }

      // Validate file size
      if (file.size > maxSize) {
        this.toasterService.error(`File ${file.name} is too large. Maximum size is 10MB.`);
        return;
      }

      // Upload to server
      this.uploadToServer(file);
    });
  }

  /**
   * Upload file to server via API
   */
  private uploadToServer(file: File): void {
    console.log('=== UPLOADING FILE ===');
    console.log('File name:', file.name);
    console.log('File size:', file.size);
    console.log('File type:', file.type);
    
    this.isUploading = true;
    
    this.imageApiService.uploadImage(file).subscribe({
      next: (response) => {
        console.log('✅ Upload successful:', response);
        const newImage: UploadedImage = {
          id: response.id,
          name: response.fileName,
          url: response.fileUrl,
          size: response.fileSize,
          uploadDate: new Date()
        };
        this.images.push(newImage);
        this.toasterService.success(`Image ${file.name} uploaded successfully!`);
        this.isUploading = false;
      },
      error: (error) => {
        console.error('❌ Upload failed:', error);
        console.error('Upload error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url,
          body: error.error
        });
        
        let errorMessage = `Failed to upload ${file.name}`;
        
        if (error.status === 500) {
          errorMessage += ': Server error. Check ImagesController.Upload method and database connection.';
        } else if (error.status === 404) {
          errorMessage += ': Upload endpoint not found. Check if /api/images/upload route exists.';
        } else if (error.status === 401) {
          errorMessage += ': Authentication required. Please login as admin.';
        } else if (error.status === 413) {
          errorMessage += ': File too large for server limits.';
        } else if (error.error?.message) {
          errorMessage += `: ${error.error.message}`;
        } else {
          errorMessage += `: ${error.status} ${error.statusText}`;
        }
        
        this.toasterService.error(errorMessage);
        this.isUploading = false;
      }
    });
  }

  deleteImage(id: number, name: string): void {
    console.log('=== DELETING IMAGE ===');
    console.log('Image ID:', id);
    console.log('Image name:', name);
    
    if (confirm(`Are you sure you want to delete ${name}?`)) {
      this.imageApiService.deleteImage(id).subscribe({
        next: () => {
          console.log('✅ Delete successful for ID:', id);
          this.images = this.images.filter(img => img.id !== id);
          this.toasterService.success(`Image ${name} deleted successfully!`);
        },
        error: (error) => {
          console.error('❌ Delete failed:', error);
          console.error('Delete error details:', {
            status: error.status,
            statusText: error.statusText,
            message: error.message,
            url: error.url,
            body: error.error
          });
          
          let errorMessage = `Failed to delete ${name}`;
          
          if (error.status === 500) {
            errorMessage += ': Server error. Check ImagesController.Delete method and database connection.';
          } else if (error.status === 404) {
            errorMessage += ': Delete endpoint not found or image does not exist.';
          } else if (error.status === 401) {
            errorMessage += ': Authentication required. Please login as admin.';
          } else if (error.error?.message) {
            errorMessage += `: ${error.error.message}`;
          } else {
            errorMessage += `: ${error.status} ${error.statusText}`;
          }
          
          this.toasterService.error(errorMessage);
        }
      });
    }
  }

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
    console.log('=== CLEARING ALL IMAGES ===');
    console.log('Images to delete:', this.images.length);
    
    if (confirm('Are you sure you want to delete all images? This action cannot be undone.')) {
      const deletePromises = this.images.map(img => 
        this.imageApiService.deleteImage(img.id).toPromise()
      );

      Promise.all(deletePromises).then(() => {
        console.log('✅ All images deleted successfully');
        this.images = [];
        this.toasterService.success('All images deleted successfully');
      }).catch(error => {
        console.error('❌ Error deleting some images:', error);
        console.error('Clear all error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url,
          body: error.error
        });
        
        let errorMessage = 'Failed to delete some images';
        
        if (error.status === 500) {
          errorMessage += ': Server error. Check ImagesController.Delete method and database connection.';
        } else if (error.status === 404) {
          errorMessage += ': Some images not found on server.';
        } else if (error.status === 401) {
          errorMessage += ': Authentication required. Please login as admin.';
        } else if (error.error?.message) {
          errorMessage += `: ${error.error.message}`;
        } else {
          errorMessage += `: ${error.status} ${error.statusText}`;
        }
        
        this.toasterService.error(errorMessage);
        console.log('Reloading images to sync with server...');
        this.loadImagesFromApi(); // Reload to sync with server
      });
    }
  }

  getTotalStorageSize(): number {
    return this.images.reduce((total, img) => total + img.size, 0);
  }

  getTotalStorageSizeFormatted(): string {
    return this.formatFileSize(this.getTotalStorageSize());
  }

  getStoragePercentage(): number {
    // For API version, we show progress relative to 100MB as reference
    // This is just for visual feedback, there's no actual limit on server
    const total = this.getTotalStorageSize();
    const reference = 100 * 1024 * 1024; // 100MB reference
    return Math.min((total / reference) * 100, 100);
  }
}
