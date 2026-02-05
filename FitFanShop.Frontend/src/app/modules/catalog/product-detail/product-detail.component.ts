import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ProductApiService } from '../../../api-services/catalog/product-api.service';
import { ProductDto, ProductVariantDto } from '../../../api-services/catalog/product-api.model';
import { CartService } from '../../../core/services/cart/cart.service';
import { DiscountApiService } from '../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { MembershipRequiredDialogComponent } from '../../shared/components/membership-required-dialog/membership-required-dialog.component';

@Component({
  selector: 'app-product-detail',
  standalone: false,
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productService = inject(ProductApiService);
  private discountService = inject(DiscountApiService);
  private cartService = inject(CartService);
  private currentUserService = inject(CurrentUserService);
  private dialog = inject(MatDialog);

  product = signal<ProductDto | null>(null);
  isLoading = signal<boolean>(true);
  selectedVariant = signal<ProductVariantDto | null>(null);
  quantity = signal<number>(1);
  activeDiscounts = signal<DiscountDto[]>([]);

  ngOnInit(): void {
    this.loadActiveDiscounts();
    this.route.params.subscribe(params => {
      const productId = +params['id'];
      this.loadProduct(productId);
    });
  }

  loadProduct(id: number): void {
    this.isLoading.set(true);
    
    this.productService.getAll({
      isEnabled: true,
      page: 1,
      pageSize: 100
    }).subscribe({
      next: (products) => {
        const product = products.find(p => p.id === id);
        if (product) {
          this.product.set(product);
          // Auto-select first available variant
          const firstAvailable = product.variants.find(v => v.stockQuantity > 0);
          if (firstAvailable) {
            this.selectedVariant.set(firstAvailable);
          }
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.isLoading.set(false);
      }
    });
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

  getProductDiscount(): { hasDiscount: boolean; discountPercent: number; originalPrice: number; discountedPrice: number; discountName: string } {
    const product = this.product();
    if (!product) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0, discountName: '' };
    }

    const discounts = this.activeDiscounts();
    const user = this.currentUserService.currentUser();
    const isMember = user?.isMember || false;
    
    // Filter applicable discounts
    const applicableDiscounts = discounts.filter(d => {
      if (d.membersOnly && !isMember) return false;
      
      const now = new Date();
      const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
      if (!isActiveByDate) return false;
      
      if (d.productIds && d.productIds.length > 0) {
        return d.productIds.includes(product.id);
      }
      
      return true;
    });
    
    if (applicableDiscounts.length === 0) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0, discountName: '' };
    }
    
    const maxDiscountPercent = Math.max(...applicableDiscounts.map(d => d.percentage));
    const bestDiscount = applicableDiscounts.find(d => d.percentage === maxDiscountPercent)!;
    const originalPrice = product.price;
    const discountedPrice = originalPrice * (1 - maxDiscountPercent / 100);
    
    return {
      hasDiscount: true,
      discountPercent: maxDiscountPercent,
      originalPrice,
      discountedPrice,
      discountName: bestDiscount.name
    };
  }

  selectVariant(variant: ProductVariantDto): void {
    if (variant.stockQuantity > 0) {
      this.selectedVariant.set(variant);
    }
  }

  increaseQuantity(): void {
    const variant = this.selectedVariant();
    if (variant && this.quantity() < variant.stockQuantity) {
      this.quantity.set(this.quantity() + 1);
    }
  }

  decreaseQuantity(): void {
    if (this.quantity() > 1) {
      this.quantity.set(this.quantity() - 1);
    }
  }

  addToCart(): void {
    const product = this.product();
    
    // Check if product is exclusive and user is not a member
    if (product?.exclusive && !this.currentUserService.currentUser()?.isMember) {
      this.dialog.open(MembershipRequiredDialogComponent, {
        width: '500px',
        maxWidth: '90vw'
      });
      return;
    }

    const variant = this.selectedVariant();
    if (variant && variant.stockQuantity > 0) {
      this.cartService.addItem(variant.id, this.quantity());
    }
  }

  canAddToCart(): boolean {
    const variant = this.selectedVariant();
    return !!(variant && variant.stockQuantity > 0 && this.quantity() <= variant.stockQuantity);
  }

  goBack(): void {
    this.router.navigate(['/catalog']);
  }

  isOutOfStock(): boolean {
    const product = this.product();
    return product ? product.variants.every(v => v.stockQuantity === 0) : true;
  }

  getAvailableSizes(): string {
    const product = this.product();
    return product ? product.variants.map(v => v.size).join(', ') : '';
  }
}
