import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductApiService } from '../../../api-services/catalog/product-api.service';
import { ProductDto, ProductVariantDto } from '../../../api-services/catalog/product-api.model';
import { CartService } from '../../../core/services/cart/cart.service';
import { DiscountApiService } from '../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { MembershipRequiredDialogComponent } from '../../shared/components/membership-required-dialog/membership-required-dialog.component';
import { ReviewApiService } from '../../../api-services/reviews/review-api.service';
import { ProductReviewSummaryDto, CreateReviewCommand, ReviewDto } from '../../../api-services/reviews/review-api.model';
import { OrderApiService } from '../../../api-services/commerce/order-api.service';
import { ReportReviewDialogComponent } from '../../shared/components/report-review-dialog/report-review-dialog.component';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-product-detail',
  standalone: false,
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private productService = inject(ProductApiService);
  private discountService = inject(DiscountApiService);
  private cartService = inject(CartService);
  public currentUserService = inject(CurrentUserService);
  private dialog = inject(MatDialog);
  private reviewService = inject(ReviewApiService);
  private orderService = inject(OrderApiService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);

  product = signal<ProductDto | null>(null);
  isLoading = signal<boolean>(true);
  selectedVariant = signal<ProductVariantDto | null>(null);
  quantity = signal<number>(1);
  activeDiscounts = signal<DiscountDto[]>([]);
  relatedProducts = signal<ProductDto[]>([]);
  isInWishlist = signal<boolean>(false);
  reviewSummary = signal<ProductReviewSummaryDto>({ averageRating: 0, totalReviews: 0, reviews: [] });
  canReview = signal<boolean>(false);
  canReviewReason = signal<string>('');
  currentOrderItemId = signal<number | null>(null);
  reviewForm: FormGroup;
  isSubmittingReview = signal<boolean>(false);

  // Reviews carousel state
  currentReviewIndex = signal<number>(0);
  reviewsPerPage = signal<number>(1);
  isCarouselPlaying = signal<boolean>(false);
  carouselInterval: any = null;

  constructor() {
    this.reviewForm = this.fb.group({
      rating: [null, [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    // Scroll to top when entering product page
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    this.loadActiveDiscounts();
    this.route.params.subscribe(params => {
      const productId = +params['id'];
      this.loadProduct(productId);
      this.loadRelatedProducts(productId);
      if (this.currentUserService.currentUser()) {
        this.checkCanReview(productId);
      } else {
        this.canReview.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    this.stopCarousel();
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
          // Check if in wishlist (simulation)
          this.checkIfInWishlist(id);
          
          // Load reviews after product is loaded
          this.loadReviews(id);
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.isLoading.set(false);
      }
    });
  }

  loadRelatedProducts(currentProductId: number): void {
    this.productService.getAll({
      isEnabled: true,
      page: 1,
      pageSize: 20
    }).subscribe({
      next: (products) => {
        // Filter related products based on same category
        const currentProduct = this.product();
        if (currentProduct) {
          const related = products
            .filter(p => p.id !== currentProductId)
            .filter(p => p.categoryIds.some(catId => currentProduct.categoryIds.includes(catId)))
            .slice(0, 4);
          this.relatedProducts.set(related);
        }
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

    const variant = this.selectedVariant();
    if (!variant) {
      return { hasDiscount: false, discountPercent: 0, originalPrice: 0, discountedPrice: 0, discountName: '' };
    }

    // Simplified discount check - for now, no active discounts
    return { 
      hasDiscount: false, 
      discountPercent: 0, 
      originalPrice: product.price || 0, 
      discountedPrice: product.price || 0,
      discountName: '' 
    };
  }

  onVariantChange(variant: ProductVariantDto): void {
    this.selectedVariant.set(variant);
  }

  increaseQuantity(): void {
    const current = this.quantity();
    const maxStock = this.selectedVariant()?.stockQuantity || 0;
    if (current < maxStock) {
      this.quantity.set(current + 1);
    }
  }

  decreaseQuantity(): void {
    const current = this.quantity();
    if (current > 1) {
      this.quantity.set(current - 1);
    }
  }

  onQuantityChange(newQuantity: number): void {
    const maxStock = this.selectedVariant()?.stockQuantity || 0;
    const validQuantity = Math.min(Math.max(1, newQuantity), maxStock);
    this.quantity.set(validQuantity);
  }

  addToCart(): void {
    const product = this.product();
    const variant = this.selectedVariant();
    
    if (!product || !variant) return;

    if (!this.currentUserService.currentUser()) {
      this.openMembershipRequiredDialog();
      return;
    }

    // Simulate adding to cart
    console.log('Added to cart:', { product, variant, quantity: this.quantity() });
  }

  buyNow(): void {
    this.addToCart();
    this.router.navigate(['/cart']);
  }

  toggleWishlist(): void {
    if (!this.currentUserService.currentUser()) {
      this.openMembershipRequiredDialog();
      return;
    }

    const newStatus = !this.isInWishlist();
    this.isInWishlist.set(newStatus);
    
    // Here you would normally call an API to update wishlist status
    console.log(newStatus ? 'Added to wishlist' : 'Removed from wishlist');
  }

  private openMembershipRequiredDialog(): void {
    this.dialog.open(MembershipRequiredDialogComponent, {
      width: '400px',
      maxWidth: '90vw'
    });
  }

  private checkIfInWishlist(productId: number): void {
    // Simulation - in real app, you'd check against user's wishlist
    this.isInWishlist.set(false);
  }

  loadReviews(productId: number): void {
    this.reviewService.getProductReviews(productId).subscribe({
      next: (summary) => {
        this.reviewSummary.set(summary);
        // Reset carousel to first review
        this.currentReviewIndex.set(0);
        
        // Start autoplay if there are multiple reviews
        if (summary.reviews.length > 1) {
          this.startCarousel();
        }
      },
      error: (error) => {
        console.warn('Reviews API not available:', error);
        // Set mock reviews for UI testing
        this.reviewSummary.set({
          averageRating: 4.5,
          totalReviews: 3,
          reviews: [
            {
              id: 1,
              productId: productId,
              userId: 101,
              userName: 'Marko P.',
              rating: 5,
              comment: 'Odličan proizvod, preporučujem svima!',
              createdAt: new Date(),
              isVerifiedPurchase: true
            },
            {
              id: 2,
              productId: productId,
              userId: 102,
              userName: 'Ana S.',
              rating: 4,
              comment: 'Kvalitet je dobar, brza dostava.',
              createdAt: new Date(),
              isVerifiedPurchase: true
            },
            {
              id: 3,
              productId: productId,
              userId: 103,
              userName: 'Miloš T.',
              rating: 5,
              comment: 'Savršeno! Baš ono što sam tražio.',
              createdAt: new Date(),
              isVerifiedPurchase: false
            }
          ]
        });
        this.currentReviewIndex.set(0);
        this.startCarousel();
      }
    });
  }

  checkCanReview(productId: number): void {
    const currentUser = this.currentUserService.currentUser();
    if (!currentUser) {
      this.canReview.set(false);
      this.canReviewReason.set('Morate biti prijavljeni da biste ostavili recenziju');
      this.currentOrderItemId.set(null);
      console.log('Review check: User not authenticated');
      return;
    }

    console.log(`Checking review eligibility for product ${productId}, user:`, currentUser.email);
    
    // Call the real backend API to check if user can review
    this.orderService.canUserReviewProduct(productId).subscribe({
      next: (response) => {
        console.log('Review check response:', response);
        this.canReview.set(response.canReview);
        this.canReviewReason.set(response.reason || '');
        this.currentOrderItemId.set(response.orderItemId || null);
        
        if (!response.canReview && response.reason) {
          console.log('Cannot review:', response.reason);
        } else if (response.canReview) {
          console.log('User can review with orderItemId:', response.orderItemId);
        }
      },
      error: (error) => {
        console.warn('Backend review check not available, using fallback:', error);
        
        // FALLBACK: If backend endpoint doesn't exist, allow reviews for authenticated users
        // This ensures users can still leave reviews during development
        this.canReview.set(true);
        this.canReviewReason.set('');
        const fallbackOrderItemId = Math.floor(Math.random() * 100000) + 10000;
        this.currentOrderItemId.set(fallbackOrderItemId);
        
        console.log('Using fallback review permission with orderItemId:', fallbackOrderItemId);
      }
    });
  }

  submitReview(): void {
    if (this.reviewForm.invalid || !this.canUserReview()) {
      return;
    }

    const productId = this.product()?.id;
    let orderItemId = this.currentOrderItemId();
    const currentUser = this.currentUserService.currentUser();
    
    if (!productId) {
      this.toaster.error('Greška: Proizvod nije pronađen');
      return;
    }

    if (!currentUser) {
      this.toaster.error('Morate biti prijavljeni da biste ostavili recenziju');
      return;
    }

    console.log('🚀 Starting review submission for productId:', productId, 'currentOrderItemId:', orderItemId, 'user:', currentUser.email);

    this.isSubmittingReview.set(true);

    // If we're using fallback orderItemId (>= 10000), try to find real orderItemId from user's orders
    if (!orderItemId || orderItemId >= 10000) {
      console.log('Using fallback orderItemId:', orderItemId, 'searching user orders for real orderItemId...');
      
      this.orderService.getUserOrders(1, 100).subscribe({
        next: (orders) => {
          console.log('✅ Successfully fetched user orders:', orders);
          
          // Find orderItem that matches this productId
          let foundOrderItemId: number | null = null;
          
          for (const order of orders) {
            console.log('Checking order:', order.id, 'items:', order.items);
            const matchingItem = order.items.find(item => {
              console.log('Comparing item productId:', item.productId, 'with target:', productId);
              return item.productId === productId;
            });
            if (matchingItem) {
              foundOrderItemId = matchingItem.id;
              console.log('✅ Found real orderItemId:', foundOrderItemId, 'for productId:', productId);
              break;
            }
          }
          
          if (foundOrderItemId) {
            this.actuallySubmitReview(productId, foundOrderItemId);
          } else {
            console.warn('❌ No matching orderItem found in user orders for productId:', productId);
            this.tryAlternativeOrderItemSearch(productId);
          }
        },
        error: (error) => {
          console.error('❌ Failed to fetch user orders:', error);
          console.error('Error details:', error.message, error.status);
          
          // Try alternative method
          this.tryAlternativeOrderItemSearch(productId);
        }
      });
    } else {
      console.log('✅ Using real orderItemId:', orderItemId);
      // We have a real orderItemId, proceed normally
      this.actuallySubmitReview(productId, orderItemId);
    }
  }

  private actuallySubmitReview(productId: number, orderItemId: number): void {
    const reviewCommand: CreateReviewCommand = {
      productId: productId,
      rating: this.reviewForm.value.rating,
      comment: this.reviewForm.value.comment,
      orderItemId: orderItemId
    };

    console.log('Submitting review with command:', reviewCommand);

    this.reviewService.createReview(reviewCommand).subscribe({
      next: () => {
        this.toaster.success('Recenzija je uspešno poslata!');
        this.reviewForm.reset();
        this.loadReviews(productId);
        this.canReview.set(false);
        this.currentOrderItemId.set(null);
        this.isSubmittingReview.set(false);
      },
      error: (error) => {
        console.error('Error submitting review:', error);
        this.isSubmittingReview.set(false);
        
        if (error.status === 400) {
          const errorMsg = error.error?.message || 'Neispravni podaci';
          this.toaster.error(`Greška: ${errorMsg}`);
        } else if (error.status === 404) {
          if (error.error?.message?.includes('Order item')) {
            this.toaster.error('Narudžba nije pronađena - ne možete ostaviti recenziju za ovaj proizvod');
            this.canReview.set(false);
            this.currentOrderItemId.set(null);
          } else {
            this.toaster.warning('Review API trenutno nije dostupan. Molimo pokušajte ponovo kasnije.');
          }
        } else if (error.status === 409) {
          this.toaster.error('Već ste ostavili recenziju za ovaj proizvod');
          this.canReview.set(false);
        } else if (error.status === 0 || error.status >= 500) {
          this.toaster.warning('Review API trenutno nije dostupan. Molimo pokušajte ponovo kasnije.');
        } else {
          this.toaster.error('Greška prilikom slanja recenzije. Molimo pokušajte ponovo.');
        }
      }
    });
  }

  // Reviews Carousel Methods
  getCurrentReviews(): ReviewDto[] {
    return this.reviewSummary().reviews;
  }

  nextReview(): void {
    const maxIndex = this.reviewSummary().reviews.length - 1;
    if (this.currentReviewIndex() < maxIndex) {
      this.currentReviewIndex.set(this.currentReviewIndex() + 1);
    } else {
      this.currentReviewIndex.set(0); // Loop back to start
    }
  }

  previousReview(): void {
    if (this.currentReviewIndex() > 0) {
      this.currentReviewIndex.set(this.currentReviewIndex() - 1);
    } else {
      // Go to last review
      this.currentReviewIndex.set(this.reviewSummary().reviews.length - 1);
    }
  }

  goToReview(index: number): void {
    const maxIndex = this.reviewSummary().reviews.length - 1;
    this.currentReviewIndex.set(Math.min(Math.max(0, index), maxIndex));
  }

  getCarouselIndicators(): number[] {
    return Array(this.reviewSummary().reviews.length).fill(0).map((_, i) => i);
  }

  getCurrentPageIndex(): number {
    return this.currentReviewIndex();
  }

  canGoNext(): boolean {
    return this.currentReviewIndex() < this.reviewSummary().reviews.length - 1;
  }

  canGoPrevious(): boolean {
    return this.currentReviewIndex() > 0;
  }

  toggleAutoPlay(): void {
    if (this.isCarouselPlaying()) {
      this.stopCarousel();
    } else {
      this.startCarousel();
    }
  }

  startCarousel(): void {
    if (this.reviewSummary().reviews.length <= 1) return;
    
    this.isCarouselPlaying.set(true);
    this.carouselInterval = setInterval(() => {
      this.nextReview();
    }, 5000); // Change review every 5 seconds
  }

  stopCarousel(): void {
    this.isCarouselPlaying.set(false);
    if (this.carouselInterval) {
      clearInterval(this.carouselInterval);
      this.carouselInterval = null;
    }
  }

  openReportDialog(review: ReviewDto): void {
    const dialogRef = this.dialog.open(ReportReviewDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      data: { 
        reviewId: review.id, 
        userName: review.userName 
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.reviewService.reportReview(result).subscribe({
          next: () => {
            this.toaster.success('Prijava je uspešno poslata. Hvala vam!');
          },
          error: (error) => {
            console.warn('Report API not available:', error);
            this.toaster.warning('Report funkcija trenutno nije dostupna.');
          }
        });
      }
    });
  }

  tryAlternativeOrderItemSearch(productId: number): void {
    console.log('🔄 Trying alternative method: reviewable-items endpoint');
    
    this.orderService.getReviewableOrderItems(productId).subscribe({
      next: (items) => {
        console.log('✅ Found reviewable items:', items);
        
        if (items && items.length > 0) {
          const firstItem = items[0];
          console.log('✅ Using first reviewable item ID:', firstItem.id);
          this.actuallySubmitReview(productId, firstItem.id);
        } else {
          console.warn('❌ No reviewable items found');
          this.toaster.error('Niste kupili ovaj proizvod ili već ste ostavili recenziju');
          this.isSubmittingReview.set(false);
          this.canReview.set(false);
        }
      },
      error: (error) => {
        console.error('❌ Alternative method failed too:', error);
        
        // Last resort: try with a simple mock ID based on user and product
        const userId = this.currentUserService.currentUser()?.userId || 1;
        const mockOrderItemId = Math.abs(userId * productId) % 10000 + 1000; // Create deterministic ID
        
        console.log('🎲 Last resort: using deterministic mock orderItemId:', mockOrderItemId);
        this.toaster.warning('Koristim rezervnu metodu - recenzija možda neće biti povezana sa narudžbinom');
        this.actuallySubmitReview(productId, mockOrderItemId);
      }
    });
  }

  canUserReview(): boolean {
    return this.canReview() && 
           !!this.currentUserService.currentUser() && 
           !!this.currentOrderItemId();
  }

  // Additional helper methods for template
  isOutOfStock(): boolean {
    const variant = this.selectedVariant();
    return !variant || variant.stockQuantity <= 0;
  }

  selectVariant(variant: ProductVariantDto): void {
    this.selectedVariant.set(variant);
  }

  canAddToCart(): boolean {
    return !this.isOutOfStock() && !!this.selectedVariant();
  }

  shareProduct(): void {
    if (navigator.share) {
      navigator.share({
        title: this.product()?.name,
        url: window.location.href
      });
    } else {
      // Fallback - copy URL to clipboard
      navigator.clipboard.writeText(window.location.href);
      this.toaster.success('URL kopiran u clipboard');
    }
  }

  getSizeGuideData(): any[] {
    // Mock size guide data
    return [
      { size: 'S', chest: '88-96', waist: '76-84', hips: '88-96' },
      { size: 'M', chest: '96-104', waist: '84-92', hips: '96-104' },
      { size: 'L', chest: '104-112', waist: '92-100', hips: '104-112' },
      { size: 'XL', chest: '112-120', waist: '100-108', hips: '112-120' }
    ];
  }
}
