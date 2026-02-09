import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WishlistDto, AddWishlistItemCommand } from './wishlist-api.model';

@Injectable({
  providedIn: 'root'
})
export class WishlistApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/wishlist`;

  getWishlist(): Observable<WishlistDto> {
    return this.http.get<WishlistDto>(this.apiUrl);
  }

  addItem(command: AddWishlistItemCommand): Observable<WishlistDto> {
    return this.http.post<WishlistDto>(`${this.apiUrl}/items`, command);
  }

  removeItem(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${itemId}`);
  }

  clearWishlist(): Observable<void> {
    return this.http.delete<void>(this.apiUrl);
  }

  checkIfInWishlist(productId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/check/${productId}`);
  }
}
