import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DiscountDto, CreateDiscountCommand, UpdateDiscountCommand } from './discount-api.model';

@Injectable({
  providedIn: 'root'
})
export class DiscountApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Discounts`;

  getAll(): Observable<DiscountDto[]> {
    return this.http.get<DiscountDto[]>(this.apiUrl);
  }

  getActive(): Observable<DiscountDto[]> {
    return this.http.get<DiscountDto[]>(`${this.apiUrl}/active`);
  }

  getById(id: number): Observable<DiscountDto> {
    return this.http.get<DiscountDto>(`${this.apiUrl}/${id}`);
  }

  create(command: CreateDiscountCommand): Observable<DiscountDto> {
    return this.http.post<DiscountDto>(this.apiUrl, command);
  }

  update(id: number, command: UpdateDiscountCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
