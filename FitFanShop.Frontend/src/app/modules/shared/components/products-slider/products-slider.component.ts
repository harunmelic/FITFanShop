import { Component, OnInit, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ProductApiService } from '../../../../api-services/catalog/product-api.service';
import { ProductDto } from '../../../../api-services/catalog/product-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';
import { DiscountApiService } from '../../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { ProductVariantSelectorComponent } from '../product-variant-selector/product-variant-selector.component';

@Component({
  selector: 'app-products-slider',
  standalone: false,
  templateUrl: './products-slider.component.html',
  styleUrls: ['./products-slider.component.scss']
})
export class ProductsSliderComponent implements OnInit {
  private productService = inject(ProductApiService);
  private cartService = inject(CartService);
  private discountService = inject(DiscountApiService);
  private currentUserService = inject(CurrentUserService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  
  currentSlide = 0;
  products = signal<ProductDto[]>([]);
  activeDiscounts = signal<DiscountDto[]>([]);

  slides: { title: string; products: ProductDto[] }[] = [
    {
      title: 'PONUDA',
      products: []
    }
  ];

  ngOnInit(): void {
    this.loadProducts();
    this.loadActiveDiscounts();
  }

  loadProducts(): void {
    this.productService.getAll({ page: 1, pageSize: 12, isEnabled: true }).subscribe({
      next: (products) => {
        // Shuffle products for random order
        const shuffled = this.shuffleArray([...products]);
        const selectedProducts = shuffled.slice(0, 12);
        
        // Split into 3 slides of 4 products each
        this.slides = [
          {
            title: 'PONUDA',
            products: selectedProducts.slice(0, 4)
          },
          {
            title: 'PONUDA',
            products: selectedProducts.slice(4, 8)
          },
          {
            title: 'PONUDA',
            products: selectedProducts.slice(8, 12)
          }
        ];
      },
      error: (error) => {
        console.error('Error loading products:', error);
        // Keep default slides if error
        this.slides = this.getDefaultSlides();
      }
    });
  }

  private shuffleArray<T>(array: T[]): T[] {
    for (let i = array.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  }

  private getDefaultSlides() {
    return [
      {
        title: 'PONUDA DRESOVA',
        products: []
      },
      {
        title: 'PONUDA OPREME',
        products: []
      },
      {
        title: 'EKSKLUZIVNA KOLEKCIJA',
        products: []
      }
    ];
  }

  addToCart(product: ProductDto): void {
    // Check if product has more than one variant or if it has one variant but not ONE SIZE
    const hasMultipleVariants = product.variants.length > 1;
    const singleVariantNotOneSize = product.variants.length === 1 && 
                                    product.variants[0].size.toUpperCase() !== 'ONE SIZE';
    
    if (hasMultipleVariants || singleVariantNotOneSize) {
      // Open dialog for variant selection
      const dialogRef = this.dialog.open(ProductVariantSelectorComponent, {
        width: '500px',
        maxWidth: '90vw',
        data: { product }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.cartService.addItem(result.variantId, result.quantity);
        }
      });
    } else {
      // Ako je ONE SIZE, dodaj direktno
      const variant = product.variants.find(v => v.stockQuantity > 0);
      if (variant) {
        this.cartService.addItem(variant.id, 1);
      }
    }
  }

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }

  prevSlide() {
    this.currentSlide = this.currentSlide === 0 ? this.slides.length - 1 : this.currentSlide - 1;
  }

  goToSlide(index: number) {
    this.currentSlide = index;
  }

  goToProduct(productId: number): void {
    this.router.navigate(['/catalog/product', productId]);
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

  getProductDiscount(product: ProductDto): { hasDiscount: boolean; discountPercent: number; originalPrice: number; discountedPrice: number } {
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
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0 };
    }
    
    const maxDiscountPercent = Math.max(...applicableDiscounts.map(d => d.percentage));
    const originalPrice = product.price;
    const discountedPrice = originalPrice * (1 - maxDiscountPercent / 100);
    
    return {
      hasDiscount: true,
      discountPercent: maxDiscountPercent,
      originalPrice,
      discountedPrice
    };
  }

  navigateToCatalog(): void {
    this.router.navigate(['/catalog']).then(() => {
      setTimeout(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }, 100);
    });
  }
}
