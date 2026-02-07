export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  categoryId?: number;
  categoryName?: string;
  stock?: number;
  isActive?: boolean;
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
  categoryId?: number;
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
  isActive?: boolean;
}
