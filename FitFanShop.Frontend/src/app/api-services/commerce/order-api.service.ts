import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  UserOrderDto, 
  CanReviewResponse, 
  OrderItemDto 
} from '../reviews/review-api.model';

export interface CreateOrderProductItemDto {
  productVariantId: number;
  quantity: number;
}

export interface CreateOrderTicketItemDto {
  ticketTypeId: number;
  quantity: number;
}

export interface CreateOrderCommand {
  products: CreateOrderProductItemDto[];
  tickets: CreateOrderTicketItemDto[];
}

export interface CreateOrderResponse {
  orderId: number;
  orderNumber: string;
  total: number;
  status: string;
}

export interface OrderDetailsDto {
  id: number;
  userId: number;
  status: string;
  totalAmount: number;
  createdAt: string;
  items: OrderItemDetailsDto[];
  tickets?: OrderTicketDetailsDto[];
}

export interface OrderItemDetailsDto {
  id: number;
  productVariantId: number;
  quantity: number;
  pricePaid: number;
  productName: string;
  variantName?: string;
  size?: string;
  color?: string;
}

export interface OrderTicketDetailsDto {
  id: number;
  ticketTypeId: number;
  eventName: string;
  ticketTypeName: string;
  quantity: number;
  pricePaid: number;
  qrCode: string;
  seatNumber?: string;
}

export interface UpdateOrderStatusCommand {
  status: string; // "Pending", "Confirmed", "Cancelled", "Delivered"
}

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

  /**
   * Create a new order from current cart with shipping information
   */
  createOrder(command: CreateOrderCommand): Observable<CreateOrderResponse> {
    return this.http.post<CreateOrderResponse>(`${this.apiUrl}`, command);
  }

  /**
   * Get order details by ID
   */
  getOrderById(orderId: number): Observable<OrderDetailsDto> {
    return this.http.get<OrderDetailsDto>(`${this.apiUrl}/${orderId}`);
  }

  /**
   * Update order status (Pending → Confirmed/Canceled)
   */
  updateOrderStatus(orderId: number, status: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${orderId}/status`, { status });
  }
}