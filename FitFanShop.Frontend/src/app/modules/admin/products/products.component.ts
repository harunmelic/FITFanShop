import { Component, OnInit } from '@angular/core';
import { ProductsApiService } from '../../../api-services/products/products-api.service';
import { Product } from '../../../api-services/products/products-api.model';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss'],
  standalone: false
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private productsApiService: ProductsApiService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.productsApiService.getProducts().subscribe({
      next: (response) => {
        this.products = response.products;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.errorMessage = 'Greška pri učitavanju proizvoda';
        this.isLoading = false;
      }
    });
  }

  deleteProduct(id: number): void {
    if (confirm('Da li ste sigurni da želite obrisati ovaj proizvod?')) {
      this.productsApiService.deleteProduct(id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          this.errorMessage = 'Greška pri brisanju proizvoda';
        }
      });
    }
  }
}
