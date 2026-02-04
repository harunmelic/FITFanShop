import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CartApiService } from '../../../api-services/commerce/cart-api.service';
import { CartDto, AddCartItemCommand, CartItemDto } from '../../../api-services/commerce/cart-api.model';
import { ToasterService } from '../toaster.service';
import { AuthFacadeService } from '../auth/auth-facade.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartApi = inject(CartApiService);
  private toaster = inject(ToasterService);
  private auth = inject(AuthFacadeService);

  private cart = signal<CartDto | null>(null);
  private savedForLater = signal<CartItemDto[]>([]);
  isLoading = signal<boolean>(false);
  isLoadingCart = signal<boolean>(false);
  isSidebarOpen = signal<boolean>(false);
  
  cartItems = computed(() => this.cart()?.items || []);
  itemCount = computed(() => this.cart()?.itemCount || 0);
  totalAmount = computed(() => this.cart()?.totalAmount || 0);
  savedItems = computed(() => this.savedForLater());

  constructor() {
    // Praćenje promene stanja autentifikacije
    effect(() => {
      const isAuthenticated = this.auth.isAuthenticated();
      
      if (isAuthenticated) {
        // Korisnik se prijavio - učitaj korpu iz baze
        this.loadCart();
      } else {
        // Korisnik se odjavio - resetuj korpu
        this.cart.set(null);
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
      this.toaster.warning('Morate biti prijavljeni da biste dodali proizvod u korpu');
      return;
    }

    const command: AddCartItemCommand = {
      productVariantId,
      quantity
    };

    this.cartApi.addItem(command).pipe(
      tap((cart) => {
        this.cart.set(cart);
        this.toaster.success('Proizvod je dodat u korpu!');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error adding item to cart:', error);
        this.toaster.error('Greška prilikom dodavanja u korpu');
      }
    });
  }

  updateItemQuantity(itemId: number, quantity: number): void {
    // Find the item to check stock
    const item = this.cartItems().find(i => i.id === itemId);
    
    if (item && item.stock && quantity > item.stock) {
      this.toaster.error(`Dostupno samo ${item.stock} ${item.stock === 1 ? 'komad' : 'komada'}!`);
      return;
    }

    this.cartApi.updateItem(itemId, { quantity }).pipe(
      tap((cart) => {
        this.cart.set(cart);
        this.toaster.success('Količina je ažurirana');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error updating cart item:', error);
        this.toaster.error('Greška prilikom ažuriranja');
      }
    });
  }

  removeItem(itemId: number): void {
    this.cartApi.removeItem(itemId).pipe(
      tap(() => {
        this.loadCart(); // Reload cart after removing item
        this.toaster.success('Proizvod je uklonjen iz korpe');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error removing cart item:', error);
        this.toaster.error('Greška prilikom uklanjanja');
      }
    });
  }

  clearCart(): void {
    this.cartApi.clearCart().pipe(
      tap(() => {
        this.cart.set(null);
        this.toaster.success('Korpa je ispražnjena');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error clearing cart:', error);
        this.toaster.error('Greška prilikom pražnjenja korpe');
      }
    });
  }

  // Save for Later functionality
  saveForLater(itemId: number): void {
    const item = this.cartItems().find(i => i.id === itemId);
    if (!item) return;

    this.isLoading.set(true);
    
    // Move from cart to saved for later
    this.removeItem(itemId);
    this.savedForLater.update(items => [...items, item]);
    this.isLoading.set(false);
    this.toaster.success('Proizvod je sačuvan za kasnije');
  }

  moveToCart(item: CartItemDto): void {
    this.isLoading.set(true);
    
    // Remove from saved for later
    this.savedForLater.update(items => items.filter(i => i.id !== item.id));
    
    // Add back to cart
    if (item.productVariantId) {
      this.addItem(item.productVariantId, item.quantity);
    }
    
    this.isLoading.set(false);
  }

  removeFromSavedForLater(itemId: number): void {
    this.savedForLater.update(items => items.filter(i => i.id !== itemId));
    this.toaster.success('Proizvod je uklonjen');
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
}
