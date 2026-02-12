import { Component, inject, HostListener, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../../../core/services/cart/cart.service';
import { CartItemDto } from '../../../../api-services/commerce/cart-api.model';
import { OrderApiService } from '../../../../api-services/commerce/order-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-cart-sidebar',
  standalone: false,
  templateUrl: './cart-sidebar.component.html',
  styleUrl: './cart-sidebar.component.scss'
})
export class CartSidebarComponent implements OnInit, OnDestroy {
  cartService = inject(CartService);
  private router = inject(Router);
  private orderService = inject(OrderApiService);
  private toaster = inject(ToasterService);
  showSavedItems = false;
  isCreatingOrder = false;

  ngOnInit(): void {
    // Prevent body scroll when sidebar is open
    const body = document.body;
    if (this.cartService.isSidebarOpen()) {
      body.classList.add('cart-sidebar-open');
    }
  }

  ngOnDestroy(): void {
    // Remove body scroll lock
    document.body.classList.remove('cart-sidebar-open');
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    this.cartService.closeSidebar();
    document.body.classList.remove('cart-sidebar-open');
  }

  onOverlayClick(): void {
    this.cartService.closeSidebar();
    document.body.classList.remove('cart-sidebar-open');
  }

  onQuantityChange(event: { itemId: number; quantity: number }): void {
    this.cartService.updateItemQuantity(event.itemId, event.quantity);
  }

  onRemoveItem(itemId: number): void {
    this.cartService.removeItem(itemId);
  }

  onSaveForLater(itemId: number): void {
    this.cartService.saveForLater(itemId);
  }

  onMoveToCart(item: CartItemDto): void {
    this.cartService.moveToCart(item);
  }

  onRemoveSavedItem(itemId: number): void {
    this.cartService.removeFromSavedForLater(itemId);
  }

  toggleSavedItems(): void {
    this.showSavedItems = !this.showSavedItems;
  }

  onClearCart(): void {
    if (confirm('Are you sure you want to remove all items from the cart?')) {
      this.cartService.clearCart();
    }
  }

  proceedToCheckout(): void {
    if (this.cartService.itemCount() === 0) {
      this.toaster.error('Your cart is empty');
      return;
    }

    this.isCreatingOrder = true;

    // Build products list from cart items
    const products = this.cartService.cartItems()
      .filter(item => item.productVariantId)
      .map(item => ({
        productVariantId: item.productVariantId!,
        quantity: item.quantity
      }));

    const tickets = this.cartService.cartItems()
      .filter(item => item.ticketTypeId)
      .map(item => ({
        ticketTypeId: item.ticketTypeId!,
        quantity: item.quantity
      }));

    // Create Pending order
    this.orderService.createOrder({ products, tickets }).subscribe({
      next: (response) => {
        this.toaster.success('Order created! Please complete payment details.');
        this.cartService.closeSidebar();
        // Navigate to checkout with orderId
        this.router.navigate(['/checkout'], {
          queryParams: { orderId: response.orderId }
        });
        this.isCreatingOrder = false;
      },
      error: (error) => {
        console.error('Order creation error:', error);
        this.toaster.error('Failed to create order. Please try again.');
        this.isCreatingOrder = false;
      }
    });
  }
}
