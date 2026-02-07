export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  categoryId?: number;
  categoryIds?: number[];  // Backend returns array of category IDs
  categoryName?: string;
  stock?: number;
  isEnabled?: boolean;  // Changed from isActive to match backend
  createdAt?: Date;
  updatedAt?: Date;
}

export interface GetProductsResponse {
  products: Product[];
  totalCount: number;
}

export interface CreateProductCommand {
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  categoryIds: number[];  // Changed from categoryId to categoryIds (array)
  stock?: number;
  variants?: ProductVariant[];  // Added variants
}

export interface ProductVariant {
  size?: string;
  color?: string;
  sku?: string;
  price?: number;
  stock?: number;
}

export interface UpdateProductCommand {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  categoryId?: number;
  stock?: number;
  isEnabled?: boolean;  // Changed from isActive to match backend
}
