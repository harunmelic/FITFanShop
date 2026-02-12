import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CartApiService } from '../../../api-services/commerce/cart-api.service';
import { CartDto, AddCartItemCommand, CartItemDto, PriceBreakdown } from '../../../api-services/commerce/cart-api.model';
import { DiscountApiService } from '../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../api-services/commerce/discount-api.model';
import { ToasterService } from '../toaster.service';
import { AuthFacadeService } from '../auth/auth-facade.service';
import { CurrentUserService } from '../auth/current-user.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartApi = inject(CartApiService);
  private discountApi = inject(DiscountApiService);
  private toaster = inject(ToasterService);
  private auth = inject(AuthFacadeService);
  private currentUser = inject(CurrentUserService);

  private cart = signal<CartDto | null>(null);
  private savedForLater = signal<CartItemDto[]>([]);
  private activeDiscounts = signal<DiscountDto[]>([]);
  private readonly savedForLaterStoragePrefix = 'fitfanshop_saved_for_later_user_';
  isLoading = signal<boolean>(false);
  isLoadingCart = signal<boolean>(false);
  isSidebarOpen = signal<boolean>(false);
  isAuthenticated = computed(() => this.auth.isAuthenticated());
  
  cartItems = computed(() => this.cart()?.items || []);
  itemCount = computed(() => this.cart()?.itemCount || 0);
  totalAmount = computed(() => this.cart()?.totalAmount || 0);
  savedItems = computed(() => this.savedForLater());
  priceBreakdown = computed(() => this.calculatePriceBreakdown());

  constructor() {
    // Track authentication state changes
    effect(() => {
      const isAuthenticated = this.auth.isAuthenticated();
      
      if (isAuthenticated) {
        // User logged in - load cart from database
        this.loadCart();
        this.loadActiveDiscounts();
        this.loadSavedForLater();
      } else {
        // User logged out - reset cart
        this.cart.set(null);
        this.activeDiscounts.set([]);
        this.savedForLater.set([]);
      }
    });
  }

  loadActiveDiscounts(): void {
    this.discountApi.getActive().subscribe({
      next: (discounts) => {
        this.activeDiscounts.set(discounts);
      },
      error: (error) => {
        console.error('Error loading discounts:', error);
      }
    });
  }

  loadCart(): void {
    if (!this.auth.isAuthenticated()) {
      return;
    }

    this.isLoadingCart.set(true);
    this.cartApi.getCart().subscribe({
      next: (cart) => {
        this.cart.set(cart);
        this.isLoadingCart.set(false);
      },
      error: (error) => {
        console.error('Error loading cart:', error);
        this.isLoadingCart.set(false);
      }
    });
  }

  addItem(productVariantId: number, quantity: number = 1): void {
    if (!this.auth.isAuthenticated()) {
      this.toaster.warning('You must be logged in to add products to cart');
      return;
    }

    const command: AddCartItemCommand = {
      productVariantId,
      quantity
    };

    this.cartApi.addItem(command).pipe(
      tap((cart) => {
        this.cart.set(cart);
        this.toaster.success('Product added to cart!');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error adding item to cart:', error);
        this.toaster.error('Error adding product to cart');
      }
    });
  }

  addTicket(ticketTypeId: number, quantity: number = 1): void {
    if (!this.auth.isAuthenticated()) {
      this.toaster.warning('You must be logged in to add tickets to cart');
      return;
    }

    const command: AddCartItemCommand = {
      ticketTypeId,
      quantity
    };

    this.cartApi.addItem(command).pipe(
      tap((cart) => {
        this.cart.set(cart);
      })
    ).subscribe({
      error: (error) => {
        console.error('Error adding ticket to cart:', error);
        this.toaster.error('Error adding ticket to cart');
      }
    });
  }

  updateItemQuantity(itemId: number, quantity: number): void {
    // Find the item to check stock
    const item = this.cartItems().find(i => i.id === itemId);
    
    if (item && item.stock && quantity > item.stock) {
      this.toaster.error(`Only ${item.stock} ${item.stock === 1 ? 'item' : 'items'} available!`);
      return;
    }

    this.cartApi.updateItem(itemId, { quantity }).pipe(
      tap((cart) => {
        this.cart.set(cart);
        this.toaster.success('Quantity updated');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error updating cart item:', error);
        this.toaster.error('Error updating quantity');
      }
    });
  }

  removeItem(itemId: number): void {
    this.cartApi.removeItem(itemId).pipe(
      tap(() => {
        this.loadCart(); // Reload cart after removing item
        this.toaster.success('Product removed from cart');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error removing cart item:', error);
        this.toaster.error('Error removing product');
      }
    });
  }

  clearCart(): void {
    this.cartApi.clearCart().pipe(
      tap(() => {
        this.cart.set(null);
        this.toaster.success('Cart cleared');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error clearing cart:', error);
        this.toaster.error('Error clearing cart');
      }
    });
  }

  // Save for Later functionality
  saveForLater(itemId: number): void {
    if (!this.auth.isAuthenticated()) {
      this.toaster.warning('Please login to use save for later');
      return;
    }

    const item = this.cartItems().find(i => i.id === itemId);
    if (!item) return;

    this.isLoading.set(true);
    
    // Move from cart to saved for later
    this.removeItem(itemId);
    this.savedForLater.update(items => [...items, item]);
    this.persistSavedForLater();
    this.isLoading.set(false);
    this.toaster.success('Product saved for later');
  }

  moveToCart(item: CartItemDto): void {
    this.isLoading.set(true);
    
    // Remove from saved for later
    this.savedForLater.update(items => items.filter(i => i.id !== item.id));
    
    // Add back to cart
    if (item.productVariantId) {
      this.addItem(item.productVariantId, item.quantity);
    } else if (item.ticketTypeId) {
      this.addTicket(item.ticketTypeId, item.quantity);
    }

    this.persistSavedForLater();
    
    this.isLoading.set(false);
  }

  removeFromSavedForLater(itemId: number): void {
    this.savedForLater.update(items => items.filter(i => i.id !== itemId));
    this.persistSavedForLater();
    this.toaster.success('Product removed');
  }

  private loadSavedForLater(): void {
    const storageKey = this.getSavedForLaterStorageKey();
    if (!storageKey) {
      this.savedForLater.set([]);
      return;
    }

    try {
      const saved = localStorage.getItem(storageKey);
      if (!saved) {
        this.savedForLater.set([]);
        return;
      }

      const parsed = JSON.parse(saved) as CartItemDto[];
      this.savedForLater.set(Array.isArray(parsed) ? parsed : []);
    } catch {
      this.savedForLater.set([]);
    }
  }

  private persistSavedForLater(): void {
    const storageKey = this.getSavedForLaterStorageKey();
    if (!storageKey) {
      return;
    }

    const savedItems = this.savedForLater();

    if (savedItems.length === 0) {
      localStorage.removeItem(storageKey);
      return;
    }

    localStorage.setItem(storageKey, JSON.stringify(savedItems));
  }

  private getSavedForLaterStorageKey(): string | null {
    const userId = this.currentUser.currentUser()?.userId;
    if (!userId) {
      return null;
    }

    return `${this.savedForLaterStoragePrefix}${userId}`;
  }

  // Sidebar controls
  openSidebar(): void {
    this.isSidebarOpen.set(true);
    document.body.classList.add('cart-sidebar-open');
    // Reload cart when opening sidebar to show skeleton
    if (this.auth.isAuthenticated()) {
      this.loadCart();
    }
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
    document.body.classList.remove('cart-sidebar-open');
  }

  toggleSidebar(): void {
    if (this.isSidebarOpen()) {
      this.closeSidebar();
    } else {
      this.openSidebar();
    }
  }

  // Calculate price breakdown
  private calculatePriceBreakdown(): PriceBreakdown {
    const items = this.cartItems();
    
    // Calculate subtotal
    const subtotal = items.reduce((sum, item) => {
      const itemPrice = item.unitPrice || item.price || 0;
      return sum + (itemPrice * item.quantity);
    }, 0);

    // Check if user is member (10% discount)
    const user = this.currentUser.currentUser();
    const isMember = user?.isMember || false;
    const memberDiscount = isMember ? subtotal * 0.10 : 0;

    // Calculate product discounts
    let productDiscounts = 0;
    const discounts = this.activeDiscounts();
    
    console.log('Active discounts:', discounts);
    console.log('Cart items:', items);
    
    items.forEach(item => {
      console.log(`Processing item ${item.productName}, productId: ${item.productId}`);
      
      // Find applicable discounts (non-member-only or member-only if user is member)
      const applicableDiscounts = discounts.filter(d => {
        const isMemberDiscount = d.membersOnly;
        const userIsMember = isMember;
        
        // If discount is members-only, user must be a member
        if (isMemberDiscount && !userIsMember) {
          return false;
        }
        
        // Check if discount is active by date
        const now = new Date();
        const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
        
        if (!isActiveByDate) {
          return false;
        }
        
        // If productIds list exists, check if product belongs to discount
        // If no productIds or empty array, apply to all products
        if (d.productIds && d.productIds.length > 0) {
          const productBelongsToDiscount = item.productId && d.productIds.includes(item.productId);
          console.log(`Discount ${d.name}: productId ${item.productId} in ${JSON.stringify(d.productIds)}? ${productBelongsToDiscount}`);
          return productBelongsToDiscount;
        }
        
        return true; // No product filter, applies to all
      });

      console.log(`Applicable discounts for ${item.productName}:`, applicableDiscounts);

      if (applicableDiscounts.length > 0) {
        // Use the highest discount for this product
        const maxDiscount = Math.max(...applicableDiscounts.map(d => d.percentage));
        const itemTotal = (item.unitPrice || item.price || 0) * item.quantity;
        const discountAmount = itemTotal * (maxDiscount / 100);
        console.log(`Applying ${maxDiscount}% discount (${discountAmount} KM) to ${item.productName}`);
        productDiscounts += discountAmount;
      }
    });

    const totalDiscount = memberDiscount + productDiscounts;

    // Calculate shipping
    // If cart has only tickets (no physical products), shipping is always 0
    const hasPhysicalProducts = items.some(item => !!item.productVariantId || item.isProduct === true);
    const shippingCost = hasPhysicalProducts ? (subtotal >= 100 ? 0 : 15) : 0;

    // Calculate tax (17% VAT) - prices already include VAT
    // Extract VAT from price: VAT = price - (price / 1.17)
    const subtotalWithDiscounts = subtotal - totalDiscount + shippingCost;
    const tax = subtotalWithDiscounts - (subtotalWithDiscounts / 1.17);

    // Calculate total - total is same as subtotal with discounts and shipping (VAT already in price)
    const total = subtotal - totalDiscount + shippingCost;

    return {
      subtotal,
      memberDiscount,
      productDiscounts,
      totalDiscount,
      shippingCost,
      tax,
      total
    };
  }
}
