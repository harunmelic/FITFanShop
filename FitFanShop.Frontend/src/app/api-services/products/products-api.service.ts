import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Product,
  GetProductsResponse,
  CreateProductCommand,
  UpdateProductCommand
} from './products-api.model';

@Injectable({
  providedIn: 'root'
})
export class ProductsApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/products`;
  private http = inject(HttpClient);

  /**
   * GET /api/products
   * Get all products (currently only returns enabled products)
   */
  getProducts(pageSize: number = 1000): Observable<GetProductsResponse> {
    return this.http.get<GetProductsResponse>(`${this.baseUrl}?pageSize=${pageSize}`);
  }

  /**
   * GET /api/products/:id
   * Get product by ID
   */
  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  /**
   * POST /api/products
   * Create new product
   */
  createProduct(payload: CreateProductCommand): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}`, payload);
  }

  /**
   * PUT /api/products/:id
   * Update existing product
   */
  updateProduct(id: number, payload: UpdateProductCommand): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, payload);
  }

  /**
   * DELETE /api/products/:id
   * Delete product
   */
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
