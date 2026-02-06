export interface CartItemDto {
  id: number;
  productVariantId?: number;
  ticketTypeId?: number;
  quantity: number;
  price: number;
  productName: string;
  variantDetails?: string;
  size?: string; // Variant size
  imageUrl?: string;
  stock?: number;
  unitPrice?: number; // Price per unit
  totalPrice?: number; // Total price (unitPrice * quantity)
  productId?: number; // Product ID for discount check
  discount?: number; // Product discount
}

export interface PriceBreakdown {
  subtotal: number; // Sum of all products
  memberDiscount: number; // 10% discount for members
  productDiscounts: number; // Product discounts
  totalDiscount: number; // Total discount
  shippingCost: number; // Shipping cost
  tax: number; // Tax (VAT)
  total: number; // Final price
}

export interface CartDto {
  id: number;
  userId: number;
  items: CartItemDto[];
  totalAmount: number;
  itemCount: number;
  priceBreakdown?: PriceBreakdown;
}

export interface AddCartItemCommand {
  productVariantId?: number;
  ticketTypeId?: number;
  quantity: number;
}

export interface UpdateCartItemDto {
  quantity: number;
}
