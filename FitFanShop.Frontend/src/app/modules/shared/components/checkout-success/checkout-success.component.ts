import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-checkout-success',
  standalone: false,
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.scss'
})
export class CheckoutSuccessComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  orderNumber = signal<string>('');
  orderTotal = signal<number>(0);

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.orderNumber.set(params['orderNumber'] || '');
      this.orderTotal.set(parseFloat(params['total']) || 0);

      // If no order data, redirect to home
      if (!this.orderNumber()) {
        this.router.navigate(['/']);
      }
    });
  }

  continueShopping(): void {
    this.router.navigate(['/catalog']);
  }

  viewOrders(): void {
    // TODO: Navigate to orders page when implemented
    this.router.navigate(['/']);
  }
}
