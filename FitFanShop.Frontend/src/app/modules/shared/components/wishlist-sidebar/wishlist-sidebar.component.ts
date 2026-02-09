import { Component, inject, HostListener, OnInit, OnDestroy, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { WishlistService } from '../../../../core/services/wishlist/wishlist.service';
import { WishlistItemDto } from '../../../../api-services/commerce/wishlist-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';
import { ProductApiService } from '../../../../api-services/catalog/product-api.service';
import { ProductVariantSelectorComponent } from '../product-variant-selector/product-variant-selector.component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DiscountApiService } from '../../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';

@Component({
  selector: 'app-wishlist-sidebar',
  standalone: false,
  templateUrl: './wishlist-sidebar.component.html',
  styleUrl: './wishlist-sidebar.component.scss'
})
export class WishlistSidebarComponent implements OnInit, OnDestroy {
  wishlistService = inject(WishlistService);
  private cartService = inject(CartService);
  private router = inject(Router);
  private productApi = inject(ProductApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private discountService = inject(DiscountApiService);
  private currentUserService = inject(CurrentUserService);
  
  activeDiscounts = signal<DiscountDto[]>([]);

  ngOnInit(): void {
    // Prevent body scroll when sidebar is open
    const body = document.body;
    if (this.wishlistService.isSidebarOpen()) {
      body.classList.add('wishlist-sidebar-open');
    }
    
    // Load active discounts
    this.loadActiveDiscounts();
  }
  
  loadActiveDiscounts(): void {
    this.discountService.getActive().subscribe({
      next: (discounts) => {
        this.activeDiscounts.set(discounts);
      },
      error: (error) => {
        console.error('Error loading discounts:', error);
      }
    });
  }
  
  getDiscountInfo(item: WishlistItemDto): { hasDiscount: boolean; discountPercent: number; originalPrice: number; discountedPrice: number } {
    const discounts = this.activeDiscounts();
    const user = this.currentUserService.currentUser();
    const isMember = user?.isMember || false;
    
    if (!item.productId) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0 };
    }
    
    // Filter applicable discounts
    const applicableDiscounts = discounts.filter(d => {
      if (d.membersOnly && !isMember) return false;
      
      const now = new Date();
      const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
      if (!isActiveByDate) return false;
      
      if (d.productIds && d.productIds.length > 0) {
        return d.productIds.includes(item.productId);
      }
      
      return true;
    });
    
    if (applicableDiscounts.length === 0) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0 };
    }
    
    const maxDiscountPercent = Math.max(...applicableDiscounts.map(d => d.percentage));
    const originalPrice = item.price || 0;
    const discountedPrice = originalPrice * (1 - maxDiscountPercent / 100);
    
    return {
      hasDiscount: true,
      discountPercent: maxDiscountPercent,
      originalPrice,
      discountedPrice
    };
  }

  ngOnDestroy(): void {
    // Remove body scroll lock
    document.body.classList.remove('wishlist-sidebar-open');
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    this.wishlistService.closeSidebar();
    document.body.classList.remove('wishlist-sidebar-open');
  }

  onOverlayClick(): void {
    this.wishlistService.closeSidebar();
    document.body.classList.remove('wishlist-sidebar-open');
  }

  onRemoveItem(itemId: number): void {
    this.wishlistService.removeItem(itemId);
  }

  onClearWishlist(): void {
    if (confirm('Are you sure you want to remove all products from the wishlist?')) {
      this.wishlistService.clearWishlist();
    }
  }

  addToCart(item: WishlistItemDto): void {
    if (!item.isAvailable) {
      this.toaster.warning('This product is currently out of stock');
      return;
    }

    // Fetch product details to open variant selector
    this.productApi.getById(item.productId).subscribe({
      next: (product) => {
        const availableVariants = product.variants.filter(v => v.stockQuantity > 0);
        
        if (availableVariants.length === 0) {
          this.toaster.error('No available variants for this product');
          return;
        }

        // Open variant selector dialog
        const dialogRef = this.dialog.open(ProductVariantSelectorComponent, {
          width: '600px',
          data: { product }
        });

        dialogRef.afterClosed().subscribe(result => {
          if (result && result.variantId) {
            // Add to cart
            this.cartService.addItem(result.variantId, result.quantity);
            
            // Remove from wishlist
            this.wishlistService.removeItem(item.id);
            
            // User stays in wishlist - don't open cart sidebar
          }
        });
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.toaster.error('Error loading product details');
      }
    });
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/catalog/products', productId]);
    this.wishlistService.closeSidebar();
  }
}
