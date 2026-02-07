export interface ProductVariantDto {
  id: number;
  size: string;
  stockQuantity: number;
  sku: string;
}

export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  originalPrice?: number;
  discountPercentage?: number;
  imageUrl?: string;
  isEnabled: boolean;
  exclusive: boolean;
  categoryIds: number[];
  variants: ProductVariantDto[];
  reviewCount: number;
  averageRating: number;
}

export interface GetAllProductsQuery {
  page?: number;
  pageSize?: number;
  categoryId?: number;
  searchTerm?: string;
  minPrice?: number;
  maxPrice?: number;
  isEnabled?: boolean;
}
