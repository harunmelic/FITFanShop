import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { Product, UpdateProductCommand } from '../../../../api-services/products/products-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-edit-product-dialog',
  templateUrl: './edit-product-dialog.component.html',
  styleUrls: ['./edit-product-dialog.component.scss'],
  standalone: false
})
export class EditProductDialogComponent implements OnInit {
  productForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService,
    @Inject(MAT_DIALOG_DATA) public data: { product: Product }
  ) {
    // Initialize form with existing product data
    this.productForm = this.fb.group({
      name: [data.product.name, [Validators.required, Validators.minLength(3)]],
      description: [data.product.description || ''],
      price: [data.product.price, [Validators.required, Validators.min(0)]],
      imageUrl: [data.product.imageUrl || ''],
      categoryId: [data.product.categoryId, [Validators.required]],
      stock: [data.product.stock || 0, [Validators.min(0)]],
      isEnabled: [data.product.isEnabled ?? true]
    });
  }

  ngOnInit(): void {
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.productForm.value;
    
    const command: UpdateProductCommand = {
      id: this.data.product.id,
      name: formValue.name,
      price: formValue.price,
      description: formValue.description || undefined,
      imageUrl: formValue.imageUrl || undefined,
      categoryId: formValue.categoryId,
      stock: formValue.stock || 0,
      isEnabled: formValue.isEnabled
    };

    console.log('Updating product:', command);

    this.productsApiService.updateProduct(this.data.product.id, command).subscribe({
      next: (product) => {
        console.log('Update successful, received product:', product);
        this.toasterService.success('Product updated successfully!');
        this.dialogRef.close(product);
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Full error object:', error);
        console.error('Error status:', error.status);
        console.error('Error body:', error.error);
        
        if (error.status === 400) {
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Not authorized. Please login.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else if (error.status === 404) {
          this.toasterService.error('Product not found.');
        } else {
          this.toasterService.error('Error updating product.');
        }
        
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  get f() {
    return this.productForm.controls;
  }
}
