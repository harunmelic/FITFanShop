import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProductDto, GetAllProductsQuery } from './product-api.model';

@Injectable({
  providedIn: 'root'
})
export class ProductApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/products`;

  getAll(query?: GetAllProductsQuery): Observable<ProductDto[]> {
    let params = new HttpParams();
    
    if (query) {
      if (query.page) params = params.set('page', query.page.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.categoryId) params = params.set('categoryId', query.categoryId.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.minPrice) params = params.set('minPrice', query.minPrice.toString());
      if (query.maxPrice) params = params.set('maxPrice', query.maxPrice.toString());
      if (query.isEnabled !== undefined) params = params.set('isEnabled', query.isEnabled.toString());
    }

    return this.http.get<ProductDto[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }
}
