export interface DiscountDto {
  id: number;
  name: string;
  discountType: DiscountType;
  value: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  productIds?: number[];
  categoryIds?: number[];
}

export enum DiscountType {
  Percentage = 0,
  FixedAmount = 1
}

export interface CreateDiscountCommand {
  name: string;
  discountType: DiscountType;
  value: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  productIds?: number[];
  categoryIds?: number[];
}

export interface UpdateDiscountCommand {
  name: string;
  discountType: DiscountType;
  value: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  productIds?: number[];
  categoryIds?: number[];
}
