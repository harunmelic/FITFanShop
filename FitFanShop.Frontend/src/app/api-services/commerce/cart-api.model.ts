export interface CartItemDto {
  id: number;
  productVariantId?: number;
  ticketTypeId?: number;
  quantity: number;
  price: number;
  productName: string;
  variantDetails?: string;
  size?: string; // Veličina varijante
  imageUrl?: string;
  stock?: number;
  unitPrice?: number; // Cena jednog komada
  totalPrice?: number; // Ukupna cena (unitPrice * quantity)
  productId?: number; // ID proizvoda za provjeru popusta
  discount?: number; // Popust na proizvod
}

export interface PriceBreakdown {
  subtotal: number; // Suma svih proizvoda
  memberDiscount: number; // 10% popust za članove
  productDiscounts: number; // Popusti na proizvode
  totalDiscount: number; // Ukupan popust
  shippingCost: number; // Troškovi dostave
  tax: number; // Porez (PDV)
  total: number; // Konačna cijena
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
