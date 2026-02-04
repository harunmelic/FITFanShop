import { Component, OnInit, inject, signal } from '@angular/core';
import { ProductApiService } from '../../../../api-services/catalog/product-api.service';
import { ProductDto } from '../../../../api-services/catalog/product-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';

@Component({
  selector: 'app-products-slider',
  standalone: false,
  templateUrl: './products-slider.component.html',
  styleUrls: ['./products-slider.component.scss']
})
export class ProductsSliderComponent implements OnInit {
  private productService = inject(ProductApiService);
  private cartService = inject(CartService);
  
  currentSlide = 0;
  products = signal<ProductDto[]>([]);

  slides: { title: string; products: ProductDto[] }[] = [];
  
  // Variant selection: Map<productId, variantId>
  selectedVariants = new Map<number, number>();
  showVariantSelector: number | null = null; // productId of product showing variant selector

  ngOnInit(): void {
    this.loadProducts();
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
            title: 'PONUDA DRESOVA',
            products: selectedProducts.slice(0, 4)
          },
          {
            title: 'PONUDA OPREME',
            products: selectedProducts.slice(4, 8)
          },
          {
            title: 'EKSKLUZIVNA KOLEKCIJA',
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
    // Always show variant selector
    this.showVariantSelector = product.id;
  }

  selectVariant(productId: number, variantId: number): void {
    const product = this.slides.flatMap(s => s.products).find(p => p.id === productId);
    if (!product) return;

    const variant = product.variants.find(v => v.id === variantId);
    if (variant && variant.stockQuantity > 0) {
      this.cartService.addItem(variant.id, 1);
      this.showVariantSelector = null; // Close selector after adding
    }
  }

  isVariantSelected(productId: number, variantId: number): boolean {
    return this.selectedVariants.get(productId) === variantId;
  }

  closeVariantSelector(): void {
    this.showVariantSelector = null;
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
}
