import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { CreateProductCommand } from '../../../../api-services/products/products-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-add-product-dialog',
  templateUrl: './add-product-dialog.component.html',
  styleUrls: ['./add-product-dialog.component.scss'],
  standalone: false
})
export class AddProductDialogComponent implements OnInit {
  productForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      imageUrl: [''],
      categoryId: [null, [Validators.required]],  // Now required
      stock: [0, [Validators.min(0)]]
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
    
    // Prepare command with categoryIds as array and default variant
    const command: CreateProductCommand = {
      name: formValue.name,
      price: formValue.price,
      description: formValue.description || undefined,
      imageUrl: formValue.imageUrl || undefined,
      categoryIds: formValue.categoryId ? [formValue.categoryId] : [],  // Convert to array
      stock: formValue.stock || 0,
      variants: [
        {
          size: 'Default',
          sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-default`,
          price: formValue.price,
          stock: formValue.stock || 0
        }
      ]
    };

    console.log('Sending product data:', command);

    this.productsApiService.createProduct(command).subscribe({
      next: (product) => {
        this.toasterService.success('Product created successfully!');
        this.dialogRef.close(product);
      },
      error: (error) => {
        console.error('Full error object:', error);
        console.error('Error status:', error.status);
        console.error('Error statusText:', error.statusText);
        console.error('Error body:', error.error);
        
        if (error.status === 400) {
          if (error.error && error.error.errors) {
            console.error('Validation errors:', error.error.errors);
          }
          if (error.error && error.error.title) {
            console.error('Error title:', error.error.title);
          }
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Not authorized. Please login.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else {
          this.toasterService.error('Error creating product. Check console for details.');
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
