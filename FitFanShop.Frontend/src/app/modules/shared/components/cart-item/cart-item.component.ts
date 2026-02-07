import { Component, Input, Output, EventEmitter, inject, computed } from '@angular/core';
import { CartItemDto } from '../../../../api-services/commerce/cart-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';

@Component({
  selector: 'app-cart-item',
  standalone: false,
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {
  @Input() item!: CartItemDto;
  @Input() isLoading = false;
  @Input() mode: 'cart' | 'saved' = 'cart';
  
  private cartService = inject(CartService);
  
  // Computed values for discount display
  discountInfo = computed(() => {
    const discounts = this.cartService['activeDiscounts']();
    const user = this.cartService['currentUser'].currentUser();
    const isMember = user?.isMember || false;
    
    if (!this.item.productId) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0 };
    }
    
    // Filtriraj primjenjive popuste
    const applicableDiscounts = discounts.filter(d => {
      if (d.membersOnly && !isMember) return false;
      
      const now = new Date();
      const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
      if (!isActiveByDate) return false;
      
      if (d.productIds && d.productIds.length > 0) {
        return d.productIds.includes(this.item.productId!);
      }
      
      return true;
    });
    
    if (applicableDiscounts.length === 0) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0 };
    }
    
    const maxDiscountPercent = Math.max(...applicableDiscounts.map(d => d.percentage));
    const originalPrice = this.item.unitPrice || this.item.price || 0;
    const discountedPrice = originalPrice * (1 - maxDiscountPercent / 100);
    
    return {
      hasDiscount: true,
      discountPercent: maxDiscountPercent,
      originalPrice,
      discountedPrice
    };
  });
  
  @Output() quantityChange = new EventEmitter<{ itemId: number; quantity: number }>();
  @Output() remove = new EventEmitter<number>();
  @Output() saveForLater = new EventEmitter<number>();
  @Output() moveToCart = new EventEmitter<CartItemDto>();

  decreaseQuantity(): void {
    if (this.item.quantity > 1) {
      this.quantityChange.emit({ itemId: this.item.id, quantity: this.item.quantity - 1 });
    }
  }

  increaseQuantity(): void {
    // Check if we have stock info and prevent going over stock
    if (this.item.stock && this.item.quantity >= this.item.stock) {
      // Show error via parent component (will trigger in cart.service)
      this.quantityChange.emit({ itemId: this.item.id, quantity: this.item.quantity + 1 });
      return;
    }
    this.quantityChange.emit({ itemId: this.item.id, quantity: this.item.quantity + 1 });
  }

  onQuantityInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = parseInt(input.value, 10);
    
    if (isNaN(value) || value <= 0) {
      input.value = this.item.quantity.toString();
      return;
    }

    // Check stock limit
    if (this.item.stock && value > this.item.stock) {
      input.value = this.item.stock.toString();
      this.quantityChange.emit({ itemId: this.item.id, quantity: this.item.stock });
      return;
    }
    
    this.quantityChange.emit({ itemId: this.item.id, quantity: value });
  }

  onRemove(): void {
    this.remove.emit(this.item.id);
  }

  onSaveForLater(): void {
    this.saveForLater.emit(this.item.id);
  }

  onMoveToCart(): void {
    this.moveToCart.emit(this.item);
  }
}
