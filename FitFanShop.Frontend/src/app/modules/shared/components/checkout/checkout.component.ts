import { Component, OnInit, inject, signal, effect } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CartService } from '../../../../core/services/cart/cart.service';
import { OrderApiService, OrderDetailsDto } from '../../../../api-services/commerce/order-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { CartItemDto } from '../../../../api-services/commerce/cart-api.model';
import { DiscountApiService } from '../../../../api-services/commerce/discount-api.service';
import { DiscountDto } from '../../../../api-services/commerce/discount-api.model';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';

@Component({
  selector: 'app-checkout',
  standalone: false,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderApiService);
  private toaster = inject(ToasterService);
  private discountApi = inject(DiscountApiService);
  private currentUser = inject(CurrentUserService);
  
  cartService = inject(CartService);
  
  shippingForm!: FormGroup;
  paymentForm!: FormGroup;
  
  isProcessing = signal<boolean>(false);
  isLoadingOrder = signal<boolean>(true);
  currentStep = signal<number>(1); // 1 = Shipping, 2 = Payment, 3 = Review
  
  orderId: number = 0;
  order = signal<OrderDetailsDto | null>(null);
  savedCartItems = signal<CartItemDto[]>([]);
  activeDiscounts = signal<DiscountDto[]>([]);

  constructor() {
    // Load active discounts
    this.loadActiveDiscounts();
    
    // Save cart items before they might get cleared
    effect(() => {
      const cartItems = this.cartService.cartItems();
      if (cartItems.length > 0 && this.savedCartItems().length === 0) {
        console.log('Saving cart items:', cartItems);
        console.log('First cart item structure:', cartItems[0]);
        console.log('Cart total amount:', this.cartService.totalAmount());
        this.savedCartItems.set([...cartItems]);
      }
    });
  }

  loadActiveDiscounts(): void {
    this.discountApi.getActive().subscribe({
      next: (discounts) => {
        this.activeDiscounts.set(discounts);
        console.log('Loaded discounts:', discounts);
      },
      error: (error) => {
        console.error('Error loading discounts:', error);
      }
    });
  }

  getSavedCartTotal(): number {
    return this.savedCartItems().reduce((total, item) => {
      return total + ((item.quantity || 0) * (item.price || 0));
    }, 0);
  }

  getItemPrice(item: CartItemDto): number {
    const basePrice = item.unitPrice || item.price || 0;
    const itemTotal = basePrice * item.quantity;
    
    // Apply discounts like in cart service
    const user = this.currentUser.currentUser();
    const isMember = user?.isMember || false;
    const discounts = this.activeDiscounts();
    
    // Calculate product discounts for this item
    const applicableDiscounts = discounts.filter(d => {
      const isMemberDiscount = d.membersOnly;
      if (isMemberDiscount && !isMember) return false;
      
      const now = new Date();
      const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
      if (!isActiveByDate) return false;
      
      if (d.productIds && d.productIds.length > 0) {
        return item.productId && d.productIds.includes(item.productId);
      }
      return true;
    });
    
    let discountAmount = 0;
    if (applicableDiscounts.length > 0) {
      const maxDiscount = Math.max(...applicableDiscounts.map(d => d.percentage));
      discountAmount = itemTotal * (maxDiscount / 100);
    }
    
    // Member discount (10%)
    const memberDiscountAmount = isMember ? itemTotal * 0.10 : 0;
    
    const finalPrice = itemTotal - discountAmount - memberDiscountAmount;
    return finalPrice / item.quantity; // Return per unit price
  }

  getCalculatedTotal(): number {
    const items = this.savedCartItems();
    if (items.length === 0) {
      return this.order()?.totalAmount || 0;
    }
    
    const subtotal = items.reduce((sum, item) => {
      const basePrice = item.unitPrice || item.price || 0;
      return sum + (basePrice * item.quantity);
    }, 0);
    
    const user = this.currentUser.currentUser();
    const isMember = user?.isMember || false;
    const memberDiscount = isMember ? subtotal * 0.10 : 0;

    // Calculate product discounts
    let productDiscounts = 0;
    const discounts = this.activeDiscounts();
    
    items.forEach(item => {
      const applicableDiscounts = discounts.filter(d => {
        const isMemberDiscount = d.membersOnly;
        if (isMemberDiscount && !isMember) return false;
        
        const now = new Date();
        const isActiveByDate = new Date(d.startDate) <= now && new Date(d.endDate) >= now;
        if (!isActiveByDate) return false;
        
        if (d.productIds && d.productIds.length > 0) {
          return item.productId && d.productIds.includes(item.productId);
        }
        return true;
      });

      if (applicableDiscounts.length > 0) {
        const maxDiscount = Math.max(...applicableDiscounts.map(d => d.percentage));
        const itemTotal = (item.unitPrice || item.price || 0) * item.quantity;
        productDiscounts += itemTotal * (maxDiscount / 100);
      }
    });

    const totalDiscount = memberDiscount + productDiscounts;
    const shippingCost = subtotal >= 100 ? 0 : 15;
    
    return subtotal - totalDiscount + shippingCost;
  }
  
  ngOnInit(): void {
    // Get orderId from query params
    this.route.queryParams.subscribe(params => {
      this.orderId = +params['orderId'];
      
      if (!this.orderId) {
        this.toaster.error('Invalid order');
        this.router.navigate(['/catalog']);
        return;
      }

      console.log('Checkout ngOnInit - orderId:', this.orderId);
      console.log('Cart state:', { 
        items: this.cartService.cartItems(), 
        count: this.cartService.itemCount(),
        total: this.cartService.totalAmount()
      });

      this.loadOrder();
    });

    this.initForms();
  }

  loadOrder(): void {
    this.isLoadingOrder.set(true);
    this.orderService.getOrderById(this.orderId).subscribe({
      next: (order) => {
        console.log('order loaded:', order); // Debug log
        this.order.set(order);
        this.isLoadingOrder.set(false);
      },
      error: (error) => {
        console.error('Error loading order:', error);
        this.toaster.error('Failed to load order details');
        this.router.navigate(['/catalog']);
        this.isLoadingOrder.set(false);
      }
    });
  }

  initForms(): void {
    this.shippingForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      address: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required]],
      postalCode: ['', [Validators.required, Validators.pattern(/^\d{5}$/)]],
      phone: ['', [Validators.required, Validators.pattern(/^[\d\s\+\-\(\)]+$/)]],
      notes: ['']
    });

    this.paymentForm = this.fb.group({
      cardNumber: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],
      cardHolder: ['', [Validators.required, Validators.minLength(3)]],
      expiryDate: ['', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]],
      cvv: ['', [Validators.required, Validators.pattern(/^\d{3,4}$/)]]
    });
  }

  nextStep(): void {
    if (this.currentStep() === 1) {
      if (this.shippingForm.invalid) {
        this.shippingForm.markAllAsTouched();
        this.toaster.error('Please fill in all required shipping fields');
        return;
      }
      this.currentStep.set(2);
    } else if (this.currentStep() === 2) {
      if (this.paymentForm.invalid) {
        this.paymentForm.markAllAsTouched();
        this.toaster.error('Please fill in all required payment fields');
        return;
      }
      this.currentStep.set(3);
    }
  }

  previousStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.set(this.currentStep() - 1);
    }
  }

  confirmOrder(): void {
    if (this.shippingForm.invalid || this.paymentForm.invalid) {
      this.toaster.error('Please complete all required fields');
      return;
    }

    this.isProcessing.set(true);

    // Mock payment processing delay (simulating payment gateway)
    setTimeout(() => {
      this.orderService.updateOrderStatus(this.orderId, 'Confirmed').subscribe({
        next: () => {
          const orderNumber = `ORD-${this.orderId.toString().padStart(6, '0')}`;
          this.toaster.success(`Payment successful! Order #${orderNumber} confirmed.`);
          
          // Clear cart after successful payment
          this.cartService.clearCart();
          
          // Navigate to success page
          this.router.navigate(['/checkout/success'], {
            queryParams: {
              orderNumber: orderNumber,
              total: this.order()?.totalAmount || 0
            }
          });
          
          this.isProcessing.set(false);
        },
        error: (error) => {
          console.error('Error confirming order:', error);
          this.toaster.error('Payment failed. Please try again.');
          this.isProcessing.set(false);
        }
      });
    }, 1500); // Mock payment delay
  }

  cancelOrder(): void {
    if (confirm('Are you sure you want to cancel this order?')) {
      this.isProcessing.set(true);
      
      this.orderService.updateOrderStatus(this.orderId, 'Cancelled').subscribe({
        next: () => {
          this.toaster.info('Order cancelled');
          this.router.navigate(['/catalog']);
          this.isProcessing.set(false);
        },
        error: (error) => {
          console.error('Error cancelling order:', error);
          this.toaster.error('Failed to cancel order');
          this.isProcessing.set(false);
        }
      });
    }
  }

  getErrorMessage(formGroup: FormGroup, fieldName: string): string {
    const field = formGroup.get(fieldName);
    if (!field?.errors || !field.touched) return '';

    if (field.errors['required']) return `${fieldName} is required`;
    if (field.errors['minlength']) return `${fieldName} is too short`;
    if (field.errors['pattern']) return `Invalid ${fieldName} format`;
    
    return '';
  }

  formatCardNumber(event: any): void {
    let value = event.target.value.replace(/\s/g, '');
    if (value.length > 16) {
      value = value.substring(0, 16);
    }
    // Add space every 4 digits
    const formatted = value.match(/.{1,4}/g)?.join(' ') || value;
    this.paymentForm.patchValue({ cardNumber: value }, { emitEvent: false });
    event.target.value = formatted;
  }

  formatExpiryDate(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    if (value.length >= 2) {
      value = value.substring(0, 2) + '/' + value.substring(2, 4);
    }
    this.paymentForm.patchValue({ expiryDate: value }, { emitEvent: false });
    event.target.value = value;
  }
}

