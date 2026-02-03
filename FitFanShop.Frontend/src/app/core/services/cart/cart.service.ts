import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CartApiService } from '../../../api-services/commerce/cart-api.service';
import { CartDto, AddCartItemCommand } from '../../../api-services/commerce/cart-api.model';
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
  
  cartItems = computed(() => this.cart()?.items || []);
  itemCount = computed(() => this.cart()?.itemCount || 0);
  totalAmount = computed(() => this.cart()?.totalAmount || 0);

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

    this.cartApi.getCart().subscribe({
      next: (cart) => {
        this.cart.set(cart);
      },
      error: (error) => {
        console.error('Error loading cart:', error);
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
}
