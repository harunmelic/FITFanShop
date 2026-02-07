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
  productForm!: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      imageUrl: [''],
      categoryId: [null],
      stock: [0, [Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const command: CreateProductCommand = this.productForm.value;

    this.productsApiService.createProduct(command).subscribe({
      next: (product) => {
        this.toasterService.success('Product created successfully!');
        this.dialogRef.close(product);
      },
      error: (error) => {
        console.error('Error creating product:', error);
        this.toasterService.error('Error creating product');
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
