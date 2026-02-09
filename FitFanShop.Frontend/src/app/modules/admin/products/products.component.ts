import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ProductsApiService } from '../../../api-services/products/products-api.service';
import { Product } from '../../../api-services/products/products-api.model';
import { AddProductDialogComponent } from './add-product-dialog/add-product-dialog.component';
import { EditProductDialogComponent } from './edit-product-dialog/edit-product-dialog.component';

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

  constructor(
    private productsApiService: ProductsApiService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.productsApiService.getProducts().subscribe({
      next: (response) => {
        // Check if response is directly an array
        if (Array.isArray(response)) {
          this.products = response;
        } else if (response && response.products) {
          this.products = response.products;
        } else {
          this.products = [];
        }
        
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.errorMessage = 'Error loading products';
        this.isLoading = false;
      }
    });
  }

  openAddProductDialog(): void {
    const dialogRef = this.dialog.open(AddProductDialogComponent, {
      width: '600px',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Product created successfully, refresh list
        this.loadProducts();
      }
    });
  }

  openEditProductDialog(product: Product): void {
    const dialogRef = this.dialog.open(EditProductDialogComponent, {
      width: '600px',
      disableClose: false,
      data: { product: product }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Dialog closed with result:', result);
      if (result) {
        // Product updated successfully, refresh list
        console.log('Refreshing products list...');
        this.loadProducts();
      } else {
        console.log('Dialog closed without result, not refreshing');
      }
    });
  }

  deleteProduct(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productsApiService.deleteProduct(id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          this.errorMessage = 'Error deleting product';
        }
      });
    }
  }

  // Calculate total stock from all product variants
  getTotalStock(product: Product): number {
    if (!product.variants || product.variants.length === 0) {
      return product.stock || 0;
    }
    // Backend uses stockQuantity, fallback to stock for backward compatibility
    const total = product.variants.reduce((total, variant) => total + (variant.stockQuantity || variant.stock || 0), 0);
    return total;
  }

  // Get stock unit display (showing all unique units)
  getStockUnitDisplay(product: Product): string {
    if (!product.variants || product.variants.length === 0) {
      return '';
    }
    const units = product.variants
      .map(v => v.stockUnit)
      .filter((unit, index, self) => unit && self.indexOf(unit) === index);
    return units.length > 0 ? ` ${units.join(', ')}` : '';
  }
}
