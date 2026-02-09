import { Component, inject, HostListener, OnInit, OnDestroy } from '@angular/core';
import { CartService } from '../../../../core/services/cart/cart.service';
import { CartItemDto } from '../../../../api-services/commerce/cart-api.model';

@Component({
  selector: 'app-cart-sidebar',
  standalone: false,
  templateUrl: './cart-sidebar.component.html',
  styleUrl: './cart-sidebar.component.scss'
})
export class CartSidebarComponent implements OnInit, OnDestroy {
  cartService = inject(CartService);
  showSavedItems = false;

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
    // TODO: Navigate to checkout
    console.log('Proceeding to checkout...');
  }
}
