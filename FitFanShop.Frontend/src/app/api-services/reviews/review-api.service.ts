import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PageResult } from '../../core/models/paging/page-result';
import { 
  ReviewDto, 
  CreateReviewCommand, 
  ProductReviewSummaryDto,
  ReportReviewCommand 
} from './review-api.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api`;

  /**
   * GET /api/reviews/product/{id}
   * Get all reviews for a specific product with summary
   */
  getProductReviews(productId: number): Observable<ProductReviewSummaryDto> {
    const url = `${this.apiUrl}/reviews/product/${productId}`;
    console.log('Calling reviews API:', url);
    return this.http.get<PageResult<ReviewDto>>(url).pipe(
      map(pageResult => {
        // Calculate average rating from reviews
        const averageRating = pageResult.items.length > 0
          ? pageResult.items.reduce((sum, review) => sum + review.rating, 0) / pageResult.items.length
          : 0;

        return {
          averageRating,
          totalReviews: pageResult.totalItems,
          reviews: pageResult.items
        };
      })
    );
  }

  /**
   * POST /api/reviews
   * Create a new review for a product
   */
  createReview(payload: CreateReviewCommand): Observable<ReviewDto> {
    return this.http.post<ReviewDto>(`${this.apiUrl}/reviews`, payload);
  }

  /**
   * POST /api/reviews/{id}/report
   * Report a review for inappropriate content
   */
  reportReview(payload: ReportReviewCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reviews/${payload.reviewId}/report`, payload);
  }

  /**
   * GET /api/products/{id}/can-review
   * Check if current user can review this product (purchased before)
   */
  canUserReviewProduct(productId: number): Observable<{ canReview: boolean }> {
    return this.http.get<{ canReview: boolean }>(`${this.apiUrl}/products/${productId}/can-review`);
  }
}