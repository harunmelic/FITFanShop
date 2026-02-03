export interface CartItemDto {
  id: number;
  productVariantId?: number;
  ticketTypeId?: number;
  quantity: number;
  price: number;
  productName: string;
  variantDetails?: string;
}

export interface CartDto {
  id: number;
  userId: number;
  items: CartItemDto[];
  totalAmount: number;
  itemCount: number;
}

export interface AddCartItemCommand {
  productVariantId?: number;
  ticketTypeId?: number;
  quantity: number;
}

export interface UpdateCartItemDto {
  quantity: number;
}
