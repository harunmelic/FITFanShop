export interface ImageDto {
  id: number;
  fileName: string;
  fileUrl: string;
  fileSize: number;
  mimeType: string;
  uploadDate: Date;
}

export interface UploadImageResponse {
  id: number;
  fileName: string;
  fileUrl: string;
  fileSize: number;
}

export interface GetImagesResponse {
  images: ImageDto[];
  totalCount: number;
}
