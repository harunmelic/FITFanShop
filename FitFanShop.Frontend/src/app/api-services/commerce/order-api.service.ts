import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
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
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  /**
   * Check if user can review a specific product and get orderItemId
   */
  canUserReviewProduct(productId: number): Observable<CanReviewResponse> {
    return this.http.get<CanReviewResponse>(`${this.apiUrl}/orders/can-review/${productId}`);
  }

  /**
   * Get user's orders with review information
   */
  getUserOrders(page?: number, pageSize?: number): Observable<UserOrderDto[]> {
    const params: any = {};
    if (page !== undefined) params.page = page.toString();
    if (pageSize !== undefined) params.pageSize = pageSize.toString();

    return this.http.get<UserOrderDto[]>(`${this.apiUrl}/orders/user`, { params });
  }

  /**
   * Get specific order items that can be reviewed for a product
   */
  getReviewableOrderItems(productId: number): Observable<OrderItemDto[]> {
    return this.http.get<OrderItemDto[]>(`${this.apiUrl}/orders/reviewable-items/${productId}`);
  }

  /**
   * Temporary mock function for development - remove when backend is ready
   * This simulates that a logged-in user can review any product with a mock orderItemId
   */
  canUserReviewProductMock(productId: number, isLoggedIn: boolean): Observable<CanReviewResponse> {
    if (!isLoggedIn) {
      return of({
        canReview: false,
        reason: 'Morate biti prijavljeni da biste ostavili recenziju'
      });
    }

    // Mock response - assume user has purchased the product
    return of({
      canReview: true,
      orderItemId: Math.floor(Math.random() * 1000000) + 1000, // Random mock orderItemId
      hasAlreadyReviewed: false
    });
  }
}