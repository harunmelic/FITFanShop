import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { WishlistApiService } from '../../../api-services/commerce/wishlist-api.service';
import { WishlistDto, WishlistItemDto, AddWishlistItemCommand } from '../../../api-services/commerce/wishlist-api.model';
import { ToasterService } from '../toaster.service';
import { AuthFacadeService } from '../auth/auth-facade.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private wishlistApi = inject(WishlistApiService);
  private toaster = inject(ToasterService);
  private auth = inject(AuthFacadeService);

  private wishlist = signal<WishlistDto | null>(null);
  isLoading = signal<boolean>(false);
  isLoadingWishlist = signal<boolean>(false);
  isSidebarOpen = signal<boolean>(false);
  
  wishlistItems = computed(() => this.wishlist()?.items || []);
  itemCount = computed(() => this.wishlist()?.itemCount || 0);
  wishlistProductIds = computed(() => {
    const items = this.wishlistItems();
    return new Set(items.map(item => item.productId));
  });

  constructor() {
    // Track authentication state changes
    effect(() => {
      const isAuthenticated = this.auth.isAuthenticated();
      
      if (isAuthenticated) {
        // User logged in - load wishlist from database
        this.loadWishlist();
      } else {
        // User logged out - reset wishlist
        this.wishlist.set(null);
      }
    });
  }

  loadWishlist(): void {
    if (!this.auth.isAuthenticated()) {
      return;
    }

    this.isLoadingWishlist.set(true);
    this.wishlistApi.getWishlist().subscribe({
      next: (wishlist) => {
        this.wishlist.set(wishlist);
        this.isLoadingWishlist.set(false);
      },
      error: (error) => {
        console.error('Error loading wishlist:', error);
        this.isLoadingWishlist.set(false);
      }
    });
  }

  addItem(productId: number): void {
    if (!this.auth.isAuthenticated()) {
      this.toaster.warning('You must be logged in to add products to wishlist');
      return;
    }

    const command: AddWishlistItemCommand = {
      productId
    };

    this.wishlistApi.addItem(command).pipe(
      tap((wishlist) => {
        this.wishlist.set(wishlist);
        this.toaster.success('Product added to wishlist!');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error adding item to wishlist:', error);
        this.toaster.error('Error adding product to wishlist');
      }
    });
  }

  removeItem(itemId: number): void {
    this.wishlistApi.removeItem(itemId).pipe(
      tap(() => {
        // Optimistically update UI by removing item from current wishlist
        const currentWishlist = this.wishlist();
        if (currentWishlist) {
          const updatedItems = currentWishlist.items.filter(item => item.id !== itemId);
          this.wishlist.set({
            ...currentWishlist,
            items: updatedItems,
            itemCount: updatedItems.length
          });
        }
        this.toaster.success('Product removed from wishlist');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error removing wishlist item:', error);
        this.toaster.error('Error removing product');
        // Reload on error to sync with server
        this.loadWishlist();
      }
    });
  }

  clearWishlist(): void {
    this.wishlistApi.clearWishlist().pipe(
      tap(() => {
        this.wishlist.set(null);
        this.toaster.success('Wishlist cleared');
      })
    ).subscribe({
      error: (error) => {
        console.error('Error clearing wishlist:', error);
        this.toaster.error('Error clearing wishlist');
      }
    });
  }

  isInWishlist(productId: number): boolean {
    return this.wishlistProductIds().has(productId);
  }

  getWishlistItemByProductId(productId: number): WishlistItemDto | undefined {
    return this.wishlistItems().find(item => item.productId === productId);
  }

  toggleWishlist(productId: number): void {
    const item = this.getWishlistItemByProductId(productId);
    
    if (item) {
      this.removeItem(item.id);
    } else {
      this.addItem(productId);
    }
  }

  openSidebar(): void {
    this.isSidebarOpen.set(true);
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
  }
}
