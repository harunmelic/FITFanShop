import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ProductDto, ProductVariantDto } from '../../../../api-services/catalog/product-api.model';

export interface ProductVariantSelectorData {
  product: ProductDto;
}

@Component({
  selector: 'app-product-variant-selector',
  standalone: false,
  templateUrl: './product-variant-selector.component.html',
  styleUrls: ['./product-variant-selector.component.scss']
})
export class ProductVariantSelectorComponent {
  selectedVariant: ProductVariantDto | null = null;
  quantity: number = 1;

  constructor(
    public dialogRef: MatDialogRef<ProductVariantSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductVariantSelectorData
  ) {}

  get product(): ProductDto {
    return this.data.product;
  }

  get availableVariants(): ProductVariantDto[] {
    return this.product.variants.filter(v => v.stockQuantity > 0);
  }

  selectVariant(variant: ProductVariantDto): void {
    this.selectedVariant = variant;
  }

  isVariantSelected(variant: ProductVariantDto): boolean {
    return this.selectedVariant?.id === variant.id;
  }

  getMaxQuantity(): number {
    return this.selectedVariant?.stockQuantity || 1;
  }

  increaseQuantity(): void {
    if (this.quantity < this.getMaxQuantity()) {
      this.quantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart(): void {
    if (this.selectedVariant) {
      this.dialogRef.close({
        variantId: this.selectedVariant.id,
        quantity: this.quantity
      });
    }
  }

  close(): void {
    this.dialogRef.close();
  }
}
