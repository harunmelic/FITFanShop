export interface WishlistItemDto {
  id: number;
  productId: number;
  productName: string;
  price: number;
  imageUrl?: string;
  isAvailable: boolean;
  addedAt: string;
  categoryName?: string;
  discount?: number;
}

export interface WishlistDto {
  id: number;
  userId: number;
  items: WishlistItemDto[];
  itemCount: number;
}

export interface AddWishlistItemCommand {
  productId: number;
}
