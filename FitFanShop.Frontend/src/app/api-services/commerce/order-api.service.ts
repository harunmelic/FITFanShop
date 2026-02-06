import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  UserOrderDto, 
  CanReviewResponse, 
  OrderItemDto 
} from '../reviews/review-api.model';

@Injectable({
  providedIn: 'root'
})
export class OrderApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) { }

  /**
   * Check if user can review a specific product and get orderItemId
   */
  canUserReviewProduct(productId: number): Observable<CanReviewResponse> {
    return this.http.get<CanReviewResponse>(`${this.apiUrl}/can-review/${productId}`);
  }

  /**
   * Get user's orders with review information
   */
  getUserOrders(page?: number, pageSize?: number): Observable<UserOrderDto[]> {
    const params: any = {};
    if (page !== undefined) params.page = page.toString();
    if (pageSize !== undefined) params.pageSize = pageSize.toString();

    return this.http.get<UserOrderDto[]>(`${this.apiUrl}/user`, { params });
  }

  /**
   * Get specific order items that can be reviewed for a product
   */
  getReviewableOrderItems(productId: number): Observable<OrderItemDto[]> {
    return this.http.get<OrderItemDto[]>(`${this.apiUrl}/reviewable-items/${productId}`);
  }
}