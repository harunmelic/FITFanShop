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
  isLoadingProduct = false;
  categories: CategoryDto[] = [];
  isLoadingCategories = false;
  hasMultipleSizes = true; // Will be updated based on variants

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditProductDialogComponent>,
    private productsApiService: ProductsApiService,
    private toasterService: ToasterService,
    private categoryApiService: CategoryApiService,
    private authFacadeService: AuthFacadeService,
    @Inject(MAT_DIALOG_DATA) public data: { product: Product }
  ) {
    // Initialize form with existing product data (stock will be refreshed in ngOnInit)
    this.productForm = this.fb.group({
      name: [data.product.name, [Validators.required, Validators.minLength(3)]],
      description: [data.product.description || ''],
      price: [data.product.price, [Validators.required, Validators.min(0)]],
      imageUrl: [data.product.imageUrl || ''],
      categoryId: [data.product.categoryIds?.[0] || data.product.categoryId, [Validators.required]],
      sizeS: [0, [Validators.min(0)]],
      sizeM: [0, [Validators.min(0)]],
      sizeL: [0, [Validators.min(0)]],
      sizeXL: [0, [Validators.min(0)]],
      sizeXXL: [0, [Validators.min(0)]],
      sizeOneSize: [0, [Validators.min(0)]], // For "One Size" products
      stockUnit: ['pcs'],
      isEnabled: [data.product.isEnabled ?? true]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadFullProduct();
  }

  /**
   * Fetch the full product by ID to ensure we have complete variant data.
   * The product passed from the list may not include variants.
   */
  private loadFullProduct(): void {
    this.isLoadingProduct = true;
    console.log('=== LOADING FULL PRODUCT ===');
    console.log('Product ID:', this.data.product.id);
    console.log('Initial product data:', this.data.product);
    console.log('Initial variants:', this.data.product.variants);
    
    this.productsApiService.getProductById(this.data.product.id).subscribe({
      next: (product) => {
        console.log('=== API RESPONSE ===');
        console.log('Fetched full product:', product);
        console.log('Variants array:', product.variants);
        console.log('Variants length:', product.variants?.length);
        if (product.variants) {
          product.variants.forEach((v, i) => {
            console.log(`Variant ${i}:`, {
              id: v.id,
              size: v.size,
              stockQuantity: v.stockQuantity,
              stock: v.stock,
              stockUnit: v.stockUnit
            });
          });
        }
        // Update the data reference so onSubmit uses fresh variant IDs
        this.data.product = product;
        this.populateStockFromVariants(product.variants);
        this.isLoadingProduct = false;
      },
      error: (error) => {
        console.error('Error fetching product details, falling back to list data:', error);
        // Fall back to whatever data we already have
        this.populateStockFromVariants(this.data.product.variants);
        this.isLoadingProduct = false;
      }
    });
  }

  /**
   * Populate the size stock fields from variant data.
   * Uses case-insensitive matching and nullish coalescing for correct 0 handling.
   */
  private populateStockFromVariants(variants?: ProductVariant[]): void {
    console.log('=== POPULATING STOCK FROM VARIANTS ===');
    console.log('Variants received:', variants);
    
    if (!variants || variants.length === 0) {
      console.log('No variants found');
      this.hasMultipleSizes = true;
      return;
    }

    // Check if this product uses standard sizes or "One Size" / "Default"
    const standardSizes = ['S', 'M', 'L', 'XL', 'XXL'];
    const hasStandardSizes = variants.some(v => 
      standardSizes.some(size => v.size?.trim().toUpperCase() === size)
    );
    const hasOneSize = variants.some(v => 
      v.size?.trim().toUpperCase() === 'ONE SIZE' || 
      v.size?.trim().toUpperCase() === 'DEFAULT' ||
      v.size?.trim().toUpperCase() === 'ONESIZE'
    );

    console.log('Has standard sizes:', hasStandardSizes);
    console.log('Has One Size:', hasOneSize);

    this.hasMultipleSizes = hasStandardSizes || variants.length > 1;
    
    const getSizeQuantity = (size: string): number => {
      const variant = variants?.find(v => {
        const match = v.size?.trim().toUpperCase() === size.toUpperCase();
        if (match) {
          console.log(`Found variant for size ${size}:`, v);
        }
        return match;
      });
      const quantity = variant?.stockQuantity ?? variant?.stock ?? 0;
      console.log(`Size ${size} quantity: ${quantity}`);
      return quantity;
    };

    const getOneSizeQuantity = (): number => {
      // Look for "One Size", "Default", or just take first variant
      const variant = variants.find(v => 
        v.size?.trim().toUpperCase() === 'ONE SIZE' || 
        v.size?.trim().toUpperCase() === 'DEFAULT' ||
        v.size?.trim().toUpperCase() === 'ONESIZE'
      ) || variants[0];
      
      const quantity = variant?.stockQuantity ?? variant?.stock ?? 0;
      console.log('One Size quantity:', quantity);
      return quantity;
    };

    const stockUnit = variants?.[0]?.stockUnit || 'pcs';
    console.log('Stock unit:', stockUnit);

    const stockValues = {
      sizeS: getSizeQuantity('S'),
      sizeM: getSizeQuantity('M'),
      sizeL: getSizeQuantity('L'),
      sizeXL: getSizeQuantity('XL'),
      sizeXXL: getSizeQuantity('XXL'),
      sizeOneSize: getOneSizeQuantity(),
      stockUnit: stockUnit,
    };
    
    console.log('Patching form with values:', stockValues);
    this.productForm.patchValue(stockValues);
    console.log('Form values after patch:', this.productForm.value);
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
    
    // Build variants array - preserve existing variants structure
    const variants: any[] = [];
    
    // Check if product has "One Size" variant
    const existingOneSizeVariant = this.data.product.variants?.find(v => 
      v.size?.trim().toUpperCase() === 'ONE SIZE' || 
      v.size?.trim().toUpperCase() === 'DEFAULT' ||
      v.size?.trim().toUpperCase() === 'ONESIZE'
    );

    if (existingOneSizeVariant || (!this.hasMultipleSizes && formValue.sizeOneSize > 0)) {
      // This is a "One Size" product
      if (existingOneSizeVariant) {
        // Update existing One Size variant
        variants.push({
          id: existingOneSizeVariant.id,
          size: existingOneSizeVariant.size, // Preserve original size name
          sku: existingOneSizeVariant.sku,
          price: formValue.price,
          stockQuantity: formValue.sizeOneSize,
          stockUnit: existingOneSizeVariant.stockUnit || formValue.stockUnit || 'pcs'
        });
      } else if (formValue.sizeOneSize > 0) {
        // Create new One Size variant
        variants.push({
          size: 'One Size',
          sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-one`,
          price: formValue.price,
          stockQuantity: formValue.sizeOneSize,
          stockUnit: formValue.stockUnit || 'pcs'
        });
      }
    } else {
      // Standard multi-size product (S/M/L/XL/XXL)
      const sizes = [
        { name: 'S', quantity: formValue.sizeS },
        { name: 'M', quantity: formValue.sizeM },
        { name: 'L', quantity: formValue.sizeL },
        { name: 'XL', quantity: formValue.sizeXL },
        { name: 'XXL', quantity: formValue.sizeXXL }
      ];

      sizes.forEach(size => {
        // Case-insensitive search for existing variant
        const existingVariant = this.data.product.variants?.find(v => 
          v.size?.trim().toUpperCase() === size.name.toUpperCase()
        );
        
        if (existingVariant) {
          // Update existing variant - MUST include id for backend to update
          variants.push({
            id: existingVariant.id,
            size: existingVariant.size, // Preserve original case
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
    }

    // If no variants at all, create a default one
    if (variants.length === 0) {
      variants.push({
        size: 'One Size',
        sku: `${formValue.name.replace(/\s+/g, '-').toLowerCase()}-one`,
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
