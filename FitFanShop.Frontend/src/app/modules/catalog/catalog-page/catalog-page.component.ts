import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductApiService } from '../../../api-services/catalog/product-api.service';
import { CategoryApiService } from '../../../api-services/catalog/category-api.service';
import { ProductDto } from '../../../api-services/catalog/product-api.model';
import { CategoryDto } from '../../../api-services/catalog/category-api.model';
import { CartService } from '../../../core/services/cart/cart.service';
import { DiscountApiService } from '../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ProductVariantSelectorComponent } from '../../shared/components/product-variant-selector/product-variant-selector.component';
import { MembershipRequiredDialogComponent } from '../../shared/components/membership-required-dialog/membership-required-dialog.component';

@Component({
  selector: 'app-catalog-page',
  standalone: false,
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss'
})
export class CatalogPageComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private productService = inject(ProductApiService);
  private categoryService = inject(CategoryApiService);
  private discountService = inject(DiscountApiService);
  cartService = inject(CartService);
  private currentUserService = inject(CurrentUserService);
  private dialog = inject(MatDialog);

  // Expose Math to template
  Math = Math;

  // Data
  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  filteredProducts = signal<ProductDto[]>([]);
  paginatedProducts = signal<ProductDto[]>([]);
  activeDiscounts = signal<DiscountDto[]>([]);
  
  // Loading states
  isLoading = signal<boolean>(false);
  
  // Filters
  searchControl = new FormControl('');
  selectedCategoryId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  selectedSizes = signal<string[]>([]);
  
  // Sort
  sortBy = signal<string>('name-asc');
  
  // Pagination
  currentPage = signal<number>(1);
  pageSize = signal<number>(9);
  totalPages = signal<number>(0);
  
  // Available filter options
  availableSizes = signal<string[]>([]);
  priceRange = signal<{ min: number; max: number }>({ min: 0, max: 1000 });
  
  // Store query params to apply after categories load
  private pendingCategoryName: string | null = null;

  ngOnInit(): void {
    // Listen to query params for category filter FIRST
    this.route.queryParams.subscribe(params => {
      const categoryId = params['categoryId'];
      const categoryName = params['categoryName'];
      
      console.log('📥 Query params received:', { categoryId, categoryName });
      
      if (categoryId) {
        this.selectedCategoryId.set(+categoryId);
      } else if (categoryName) {
        this.pendingCategoryName = categoryName;
        console.log('💾 Saved pending category name:', categoryName);
      }
    });
    
    // Load data - categories will trigger filter application
    this.loadCategories();
    this.loadProducts();
    this.loadActiveDiscounts();
    this.setupSearchListener();
  }

  setupSearchListener(): void {
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.applyFilters();
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        console.log('📦 Categories loaded:', categories.length);
        this.categories.set(categories);
        // Apply pending category name filter if exists
        if (this.pendingCategoryName) {
          console.log('🔄 Applying pending category filter:', this.pendingCategoryName);
          this.applyCategoryNameFilter();
        } else {
          console.log('⏭️ No pending category, applying normal filters');
          this.applyFilters();
        }
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadProducts(): void {
    this.isLoading.set(true);
    
    this.productService.getAll({
      isEnabled: true,
      page: 1,
      pageSize: 100
    }).subscribe({
      next: (products) => {
        console.log('📦 Products loaded:', products.length);
        this.products.set(products);
        this.extractFilterOptions(products);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading products:', error);
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

  extractFilterOptions(products: ProductDto[]): void {
    // Extract unique sizes
    const sizes = new Set<string>();
    let minPrice = Infinity;
    let maxPrice = 0;

    products.forEach(product => {
      product.variants.forEach(variant => {
        sizes.add(variant.size);
      });
      
      if (product.price < minPrice) minPrice = product.price;
      if (product.price > maxPrice) maxPrice = product.price;
    });

    this.availableSizes.set(Array.from(sizes).sort());
    this.priceRange.set({ min: minPrice, max: maxPrice });
  }

  private applyCategoryNameFilter(): void {
    if (!this.pendingCategoryName) return;
    
    console.log('🔍 Looking for category:', this.pendingCategoryName);
    console.log('📋 Available categories:', this.categories().map(c => c.name));
    
    const category = this.categories().find(cat => 
      cat.name.toLowerCase() === this.pendingCategoryName!.toLowerCase()
    );
    
    if (category) {
      console.log('✅ Found category:', category.name, 'ID:', category.id);
      this.selectedCategoryId.set(category.id);
      this.applyFilters();
    } else {
      console.warn('❌ Category not found:', this.pendingCategoryName);
    }
    
    this.pendingCategoryName = null;
  }

  applyFilters(): void {
    let filtered = [...this.products()];
    
    console.log('🔧 Applying filters...');
    console.log('   Total products:', filtered.length);
    console.log('   Selected category ID:', this.selectedCategoryId());
    
    // Search filter
    const searchTerm = this.searchControl.value?.toLowerCase() || '';
    if (searchTerm) {
      filtered = filtered.filter(p => 
        p.name.toLowerCase().includes(searchTerm) ||
        p.description?.toLowerCase().includes(searchTerm)
      );
    }
    
    // Category filter
    if (this.selectedCategoryId()) {
      const beforeCount = filtered.length;
      filtered = filtered.filter(p => p.categoryIds.includes(this.selectedCategoryId()!));
      console.log(`   Category filter: ${beforeCount} → ${filtered.length} products`);
    }
    
    // Price filter
    if (this.minPrice() !== null) {
      filtered = filtered.filter(p => p.price >= this.minPrice()!);
    }
    if (this.maxPrice() !== null) {
      filtered = filtered.filter(p => p.price <= this.maxPrice()!);
    }
    
    // Size filter
    if (this.selectedSizes().length > 0) {
      filtered = filtered.filter(p => 
        p.variants.some(v => this.selectedSizes().includes(v.size))
      );
    }
    
    // Apply sorting
    this.sortProducts(filtered);
  }

  sortProducts(products: ProductDto[]): void {
    const sortBy = this.sortBy();
    
    switch(sortBy) {
      case 'name-asc':
        products.sort((a, b) => a.name.localeCompare(b.name));
        break;
      case 'name-desc':
        products.sort((a, b) => b.name.localeCompare(a.name));
        break;
      case 'price-asc':
        products.sort((a, b) => a.price - b.price);
        break;
      case 'price-desc':
        products.sort((a, b) => b.price - a.price);
        break;
      case 'rating':
        products.sort((a, b) => (b.averageRating || 0) - (a.averageRating || 0));
        break;
    }
    
    this.filteredProducts.set(products);
    this.updatePagination();
  }

  updatePagination(): void {
    const total = this.filteredProducts().length;
    const pages = Math.ceil(total / this.pageSize());
    this.totalPages.set(pages);
    
    // Reset to page 1 if current page is out of bounds
    if (this.currentPage() > pages && pages > 0) {
      this.currentPage.set(1);
    }
    
    // Calculate paginated products
    const startIndex = (this.currentPage() - 1) * this.pageSize();
    const endIndex = startIndex + this.pageSize();
    const paginated = this.filteredProducts().slice(startIndex, endIndex);
    this.paginatedProducts.set(paginated);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.updatePagination();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  nextPage(): void {
    this.goToPage(this.currentPage() + 1);
  }

  prevPage(): void {
    this.goToPage(this.currentPage() - 1);
  }

  getPageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    
    if (total <= 7) {
      // Show all pages if 7 or less
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      // Show first page
      pages.push(1);
      
      if (current > 3) {
        pages.push(-1); // Ellipsis
      }
      
      // Show pages around current
      const start = Math.max(2, current - 1);
      const end = Math.min(total - 1, current + 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      if (current < total - 2) {
        pages.push(-1); // Ellipsis
      }
      
      // Show last page
      pages.push(total);
    }
    
    return pages;
  }

  onCategorySelect(categoryId: number | null): void {
    this.selectedCategoryId.set(categoryId);
    this.applyFilters();
  }

  onSizeToggle(size: string): void {
    const sizes = [...this.selectedSizes()];
    const index = sizes.indexOf(size);
    
    if (index > -1) {
      sizes.splice(index, 1);
    } else {
      sizes.push(size);
    }
    
    this.selectedSizes.set(sizes);
    this.applyFilters();
  }

  onSortChange(sortBy: string): void {
    this.sortBy.set(sortBy);
    this.applyFilters();
  }

  onPriceRangeChange(min: number | null, max: number | null): void {
    this.minPrice.set(min);
    this.maxPrice.set(max);
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchControl.setValue('');
    this.selectedCategoryId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.selectedSizes.set([]);
    this.sortBy.set('name-asc');
    this.applyFilters();
  }

  goToProduct(productId: number): void {
    this.router.navigate(['/catalog/product', productId]);
  }

  goToCategory(categoryId: number, categoryName: string): void {
    this.router.navigate(['/catalog/category', categoryId], {
      queryParams: { name: categoryName }
    });
  }

  isOutOfStock(product: ProductDto): boolean {
    return product.variants.every(v => v.stockQuantity === 0);
  }

  addToCart(product: ProductDto): void {
    // Check if product is exclusive and user is not a member
    if (product.exclusive && !this.currentUserService.currentUser()?.isMember) {
      this.dialog.open(MembershipRequiredDialogComponent, {
        width: '500px',
        maxWidth: '90vw'
      });
      return;
    }

    const hasMultipleVariants = product.variants.length > 1;
    const singleVariantNotOneSize = product.variants.length === 1 && 
                                    product.variants[0].size.toUpperCase() !== 'ONE SIZE';
    
    if (hasMultipleVariants || singleVariantNotOneSize) {
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
      const firstAvailableVariant = product.variants.find(v => v.stockQuantity > 0);
      if (firstAvailableVariant) {
        this.cartService.addItem(firstAvailableVariant.id, 1);
      }
    }
  }

  getCategoryIcon(categoryName: string): string {
    const name = categoryName.toLowerCase();
    if (name.includes('dres')) return 'checkroom';
    if (name.includes('trenerka')) return 'sports';
    if (name.includes('oprema')) return 'sports_soccer';
    return 'category';
  }

  getProductCountByCategory(categoryId: number): number {
    return this.products().filter(p => p.categoryIds.includes(categoryId)).length;
  }

  getCategoryName(categoryId: number): string {
    return this.categories().find(c => c.id === categoryId)?.name || '';
  }

  hasActiveFilters(): boolean {
    return !!(
      this.selectedCategoryId() ||
      this.searchControl.value ||
      this.selectedSizes().length > 0 ||
      this.minPrice() !== null ||
      this.maxPrice() !== null
    );
  }
}
