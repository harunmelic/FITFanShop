import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { CreateProductCommand } from '../../../../api-services/products/products-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../../api-services/catalog/category-api.model';

@Component({
  selector: 'app-add-product-dialog',
  templateUrl: './add-product-dialog.component.html',
  styleUrls: ['./add-product-dialog.component.scss'],
  standalone: false
})
export class AddProductDialogComponent implements OnInit {
  productForm: FormGroup;
  isSubmitting = false;
  categories: CategoryDto[] = [];
  isLoadingCategories = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService,
    private categoryApiService: CategoryApiService
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      imageUrl: [''],
      categoryId: [null, [Validators.required]],
      sizeS: [0, [Validators.min(0)]],
      sizeM: [0, [Validators.min(0)]],
      sizeL: [0, [Validators.min(0)]],
      sizeXL: [0, [Validators.min(0)]],
      sizeXXL: [0, [Validators.min(0)]],
      stockUnit: ['pcs']  // Default to 'pcs' (pieces)
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoadingCategories = true;
    this.categoryApiService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isLoadingCategories = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.toasterService.error('Error loading categories');
        this.isLoadingCategories = false;
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.productForm.value;
    
    // Create variants for each size with stock quantity
    const variants: any[] = [];
    const sizes = [
      { name: 'S', quantity: formValue.sizeS },
      { name: 'M', quantity: formValue.sizeM },
      { name: 'L', quantity: formValue.sizeL },
      { name: 'XL', quantity: formValue.sizeXL },
      { name: 'XXL', quantity: formValue.sizeXXL }
    ];

    // Add variants only for sizes with stock > 0
    sizes.forEach(size => {
      if (size.quantity > 0) {
        variants.push({
          size: size.name,
          sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-${size.name.toLowerCase()}`,
          price: formValue.price,
          stockQuantity: size.quantity,
          stockUnit: formValue.stockUnit || 'pcs'
        });
      }
    });

    // If no variants, create a default one
    if (variants.length === 0) {
      variants.push({
        size: 'Default',
        sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-default`,
        price: formValue.price,
        stockQuantity: 0,
        stockUnit: formValue.stockUnit || 'pcs'
      });
    }
    
    // Prepare command with categoryIds as array and variants
    const command: CreateProductCommand = {
      name: formValue.name,
      price: formValue.price,
      description: formValue.description || undefined,
      imageUrl: formValue.imageUrl || undefined,
      categoryIds: formValue.categoryId ? [Number(formValue.categoryId)] : [],
      variants: variants
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
