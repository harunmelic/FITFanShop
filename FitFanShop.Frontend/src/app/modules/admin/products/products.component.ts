import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ProductsApiService } from '../../../api-services/products/products-api.service';
import { Product } from '../../../api-services/products/products-api.model';
import { AddProductDialogComponent } from './add-product-dialog/add-product-dialog.component';
import { EditProductDialogComponent } from './edit-product-dialog/edit-product-dialog.component';
import { DeleteConfirmationDialogComponent } from './delete-confirmation-dialog/delete-confirmation-dialog.component';

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
  sortColumn: string = 'id';
  sortDirection: 'asc' | 'desc' = 'asc';

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
    
    // Ensure variants are included in the response
    this.productsApiService.getProducts(1000, true).subscribe({
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

  deleteProduct(product: Product): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '420px',
      panelClass: 'delete-confirmation-dialog-container',
      data: {
        title: 'Obriši proizvod',
        message: 'Da li ste sigurni da želite da obrišete ovaj proizvod?',
        productName: product.name
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.productsApiService.deleteProduct(product.id).subscribe({
          next: () => {
            this.loadProducts();
          },
          error: (error) => {
            console.error('Error deleting product:', error);
            this.errorMessage = 'Error deleting product';
          }
        });
      }
    });
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

  // Sort products by column
  sortBy(column: string): void {
    if (this.sortColumn === column) {
      // Toggle direction if same column
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Set new column and default to ascending
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.products.sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (column) {
        case 'id':
          valueA = a.id;
          valueB = b.id;
          break;
        case 'name':
          valueA = a.name?.toLowerCase() || '';
          valueB = b.name?.toLowerCase() || '';
          break;
        case 'price':
          valueA = a.price || 0;
          valueB = b.price || 0;
          break;
        case 'category':
          valueA = a.categoryName?.toLowerCase() || '';
          valueB = b.categoryName?.toLowerCase() || '';
          break;
        case 'stock':
          valueA = this.getTotalStock(a);
          valueB = this.getTotalStock(b);
          break;
        case 'status':
          valueA = a.isEnabled ? 1 : 0;
          valueB = b.isEnabled ? 1 : 0;
          break;
        default:
          return 0;
      }

      if (valueA < valueB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (valueA > valueB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });
  }

  // Get sort icon for column header
  getSortIcon(column: string): string {
    if (this.sortColumn !== column) {
      return '↕️';
    }
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }
}
