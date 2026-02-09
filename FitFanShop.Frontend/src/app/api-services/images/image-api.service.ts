import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ImageDto, UploadImageResponse, GetImagesResponse } from './image-api.model';

/**
 * Image API Service
 * 
 * Backend endpoints needed:
 * 
 * POST /api/images/upload
 * - Content-Type: multipart/form-data
 * - Body: { file: File }
 * - Returns: { id, fileName, fileUrl, fileSize }
 * - Uploads image to server storage (disk or cloud)
 * - Saves metadata to database
 * 
 * GET /api/images
 * - Returns array of uploaded images with metadata
 * 
 * GET /api/images/:id
 * - Returns single image metadata
 * 
 * DELETE /api/images/:id
 * - Deletes image from storage and database
 */
@Injectable({
  providedIn: 'root'
})
export class ImageApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/images`;
  private http = inject(HttpClient);

  constructor() {
    console.log('🔧 ImageApiService initialized');
    console.log('Environment API URL:', environment.apiUrl);
    console.log('Images base URL:', this.baseUrl);
  }

  /**
   * Upload image to server
   * @param file Image file to upload
   * @returns Observable with uploaded image metadata
   */
  uploadImage(file: File): Observable<UploadImageResponse> {
    const uploadUrl = `${this.baseUrl}/upload`;
    console.log('🚀 Uploading image to:', uploadUrl);
    console.log('File details:', { name: file.name, size: file.size, type: file.type });
    
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<UploadImageResponse>(uploadUrl, formData);
  }

  /**
   * Get all uploaded images
   * @returns Observable with array of images
   */
  getAllImages(): Observable<ImageDto[]> {
    const listUrl = this.baseUrl;
    console.log('📸 Fetching images from:', listUrl);
    
    return this.http.get<ImageDto[]>(listUrl);
  }

  /**
   * Get image by ID
   * @param id Image ID
   * @returns Observable with image metadata
   */
  getImageById(id: number): Observable<ImageDto> {
    const getUrl = `${this.baseUrl}/${id}`;
    console.log('🔍 Fetching image by ID from:', getUrl);
    
    return this.http.get<ImageDto>(getUrl);
  }

  /**
   * Delete image
   * @param id Image ID to delete
   * @returns Observable
   */
  deleteImage(id: number): Observable<void> {
    const deleteUrl = `${this.baseUrl}/${id}`;
    console.log('🗑️ Deleting image:', deleteUrl);
    
    return this.http.delete<void>(deleteUrl);
  }
}
