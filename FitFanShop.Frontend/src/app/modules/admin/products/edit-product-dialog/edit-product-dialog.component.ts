import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { Product, UpdateProductCommand, ProductVariant } from '../../../../api-services/products/products-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../../api-services/catalog/category-api.model';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

@Component({
  selector: 'app-edit-product-dialog',
  templateUrl: './edit-product-dialog.component.html',
  styleUrls: ['./edit-product-dialog.component.scss'],
  standalone: false
})
export class EditProductDialogComponent implements OnInit {
  productForm: FormGroup;
  isSubmitting = false;
  categories: CategoryDto[] = [];
  isLoadingCategories = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService,
    private categoryApiService: CategoryApiService,
    private authFacadeService: AuthFacadeService,
    @Inject(MAT_DIALOG_DATA) public data: { product: Product }
  ) {
    // Initialize size quantities from variants
    const getSizeQuantity = (size: string): number => {
      const variant = data.product.variants?.find(v => v.size === size);
      return variant?.stockQuantity || variant?.stock || 0;
    };

    const getStockUnit = (): string => {
      return data.product.variants?.[0]?.stockUnit || 'pcs';
    };

    // Initialize form with existing product data
    this.productForm = this.fb.group({
      name: [data.product.name, [Validators.required, Validators.minLength(3)]],
      description: [data.product.description || ''],
      price: [data.product.price, [Validators.required, Validators.min(0)]],
      imageUrl: [data.product.imageUrl || ''],
      categoryId: [data.product.categoryIds?.[0] || data.product.categoryId, [Validators.required]],
      sizeS: [getSizeQuantity('S'), [Validators.min(0)]],
      sizeM: [getSizeQuantity('M'), [Validators.min(0)]],
      sizeL: [getSizeQuantity('L'), [Validators.min(0)]],
      sizeXL: [getSizeQuantity('XL'), [Validators.min(0)]],
      sizeXXL: [getSizeQuantity('XXL'), [Validators.min(0)]],
      stockUnit: [getStockUnit()],
      isEnabled: [data.product.isEnabled ?? true]
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
    
    // Map sizes to their quantities
    const sizes = [
      { name: 'S', quantity: formValue.sizeS },
      { name: 'M', quantity: formValue.sizeM },
      { name: 'L', quantity: formValue.sizeL },
      { name: 'XL', quantity: formValue.sizeXL },
      { name: 'XXL', quantity: formValue.sizeXXL }
    ];

    // Build variants array - include existing variants with their IDs
    const variants: any[] = [];
    
    sizes.forEach(size => {
      const existingVariant = this.data.product.variants?.find(v => v.size === size.name);
      
      if (existingVariant) {
        // Update existing variant - MUST include id for backend to update
        variants.push({
          id: existingVariant.id,
          size: size.name,
          sku: existingVariant.sku,
          price: formValue.price,
          stockQuantity: size.quantity,
          stockUnit: existingVariant.stockUnit || formValue.stockUnit || 'pcs'
        });
      } else if (size.quantity > 0) {
        // Create new variant only if quantity > 0
        variants.push({
          size: size.name,
          sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-${size.name.toLowerCase()}`,
          price: formValue.price,
          stockQuantity: size.quantity,
          stockUnit: formValue.stockUnit || 'pcs'
        });
      }
    });

    // If no variants at all, create a default one
    if (variants.length === 0) {
      variants.push({
        size: 'Default',
        sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-default`,
        price: formValue.price,
        stockQuantity: 0,
        stockUnit: formValue.stockUnit || 'pcs'
      });
    }
    
    const command: UpdateProductCommand = {
      id: this.data.product.id,
      name: formValue.name,
      price: formValue.price,
      description: formValue.description || undefined,
      imageUrl: formValue.imageUrl || undefined,
      categoryIds: formValue.categoryId ? [Number(formValue.categoryId)] : [],
      variants: variants,  // Include variants for updating stock quantities
      isEnabled: formValue.isEnabled
    };

    console.log('=== UPDATE PRODUCT REQUEST ===');
    console.log('Product ID:', this.data.product.id);
    console.log('Command:', JSON.stringify(command, null, 2));
    console.log('Variants count:', variants.length);
    console.log('Variants:', variants);

    // Check auth status before making request
    if (!this.authFacadeService.isAuthenticated()) {
      console.error('User not authenticated!');
      this.toasterService.error('Your session has expired. Please log in again.');
      this.isSubmitting = false;
      return;
    }
    
    const currentUser = this.authFacadeService.currentUser();
    if (!currentUser?.isAdmin) {
      console.error('User is not admin - insufficient permissions!');
      this.toasterService.error('You do not have administrator permissions.');
      this.isSubmitting = false;
      return;
    }

    console.log('User is authenticated admin, proceeding with update...');

    this.productsApiService.updateProduct(this.data.product.id, command).subscribe({
      next: (product) => {
        console.log('=== UPDATE SUCCESS ===');
        console.log('Received product:', product);
        console.log('Received variants:', product.variants);
        this.toasterService.success('Product updated successfully!');
        this.dialogRef.close(product);
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('=== UPDATE ERROR ===');
        console.error('Full error object:', error);
        console.error('Error status:', error.status);
        console.error('Error statusText:', error.statusText);
        console.error('Error body:', error.error);
        console.error('Error headers:', error.headers);
        
        if (error.status === 400) {
          console.error('Validation error details:', error.error);
          this.toasterService.error('Validation error: ' + (error.error?.title || JSON.stringify(error.error) || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Session expired or insufficient permissions. Please log in as admin.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else if (error.status === 404) {
          this.toasterService.error('Product not found.');
        } else {
          this.toasterService.error('Error updating product: ' + (error.error?.message || error.message || 'Unknown error'));
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
