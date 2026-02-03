import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoryDto } from './category-api.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/categories`;

  getAll(page: number = 1, pageSize: number = 100): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }

  getById(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`);
  }
}
