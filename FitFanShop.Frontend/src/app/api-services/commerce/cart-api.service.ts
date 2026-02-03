import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CartDto, AddCartItemCommand, UpdateCartItemDto } from './cart-api.model';

@Injectable({
  providedIn: 'root'
})
export class CartApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/cart`;

  getCart(): Observable<CartDto> {
    return this.http.get<CartDto>(this.apiUrl);
  }

  addItem(command: AddCartItemCommand): Observable<CartDto> {
    return this.http.post<CartDto>(`${this.apiUrl}/items`, command);
  }

  updateItem(itemId: number, dto: UpdateCartItemDto): Observable<CartDto> {
    return this.http.put<CartDto>(`${this.apiUrl}/items/${itemId}`, dto);
  }

  removeItem(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${itemId}`);
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(this.apiUrl);
  }
}
