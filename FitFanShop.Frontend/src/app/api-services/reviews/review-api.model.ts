export interface ReviewDto {
  id: number;
  productId: number;
  userId: number;
  userName: string;
  rating: number;
  comment: string;
  createdAt: Date;
  isVerifiedPurchase: boolean;
}

export interface CreateReviewCommand {
  productId: number;
  orderItemId: number;
  rating: number;
  comment: string;
}

export interface ProductReviewSummaryDto {
  averageRating: number;
  totalReviews: number;
  reviews: ReviewDto[];
}

export interface ReportReviewCommand {
  reviewId: number;
  reason: string;
  description?: string;
}

// Order-related models for review verification
export interface OrderItemDto {
  id: number;
  orderId: number;
  productId: number;
  productName: string;
  productVariantId?: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  canReview: boolean;
  hasReviewed: boolean;
}

export interface UserOrderDto {
  id: number;
  orderDate: Date;
  status: string;
  items: OrderItemDto[];
}

export interface CanReviewResponse {
  canReview: boolean;
  orderItemId?: number;
  reason?: string;
  hasAlreadyReviewed?: boolean;
}