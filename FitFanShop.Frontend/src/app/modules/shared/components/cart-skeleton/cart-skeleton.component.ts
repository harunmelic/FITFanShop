import { Component } from '@angular/core';

@Component({
  selector: 'app-cart-skeleton',
  standalone: false,
  templateUrl: './cart-skeleton.component.html',
  styleUrl: './cart-skeleton.component.scss'
})
export class CartSkeletonComponent {
  skeletonItems = [1, 2, 3]; // Prikazuje 3 skeleton items
}
