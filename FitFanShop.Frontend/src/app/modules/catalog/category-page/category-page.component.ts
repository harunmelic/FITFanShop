import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductApiService } from '../../../api-services/catalog/product-api.service';
import { ProductDto } from '../../../api-services/catalog/product-api.model';
import { CartService } from '../../../core/services/cart/cart.service';

@Component({
  selector: 'app-category-page',
  standalone: false,
  templateUrl: './category-page.component.html',
  styleUrl: './category-page.component.scss'
})
export class CategoryPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productService = inject(ProductApiService);
  cartService = inject(CartService);

  categoryId: number = 0;
  categoryName: string = '';
  
  products = signal<ProductDto[]>([]);
  isLoading = signal<boolean>(false);
  
  // Pagination
  currentPage = signal<number>(1);
  pageSize = signal<number>(9); // 3x3 grid
  totalItems = signal<number>(0);
  totalPages = signal<number>(0);

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.categoryId = +params['id'];
      this.loadProducts();
    });

    this.route.queryParams.subscribe(params => {
      this.categoryName = params['name'] || '';
    });
  }

  loadProducts(): void {
    this.isLoading.set(true);
    
    this.productService.getAll({
      categoryId: this.categoryId,
      page: this.currentPage(),
      pageSize: this.pageSize(),
      isEnabled: true
    }).subscribe({
      next: (products) => {
        this.products.set(products);
        this.totalItems.set(products.length);
        this.totalPages.set(Math.ceil(products.length / this.pageSize()));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.isLoading.set(false);
      }
    });
  }

  getProductImage(product: ProductDto): string {
    return product.imageUrl || '';
  }

  isOutOfStock(product: ProductDto): boolean {
    return product.variants.every(v => v.stockQuantity === 0);
  }

  addToCart(product: ProductDto): void {
    const firstAvailableVariant = product.variants.find(v => v.stockQuantity > 0);
    if (firstAvailableVariant) {
      this.cartService.addItem(firstAvailableVariant.id, 1);
    }
  }

  goToProduct(productId: number): void {
    // TODO: Navigate to product detail page when implemented
    console.log('Navigate to product:', productId);
  }

  // Pagination methods
  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadProducts();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  prevPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadProducts();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadProducts();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
