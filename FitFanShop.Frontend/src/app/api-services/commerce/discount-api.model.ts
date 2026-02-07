export interface DiscountDto {
  id: number;
  name: string;
  percentage: number;
  startDate: string;
  endDate: string;
  membersOnly: boolean;
  isActive: boolean;
  productCount?: number;
  productIds?: number[]; // Lista ID-jeva proizvoda koji imaju ovaj popust
}

export interface CreateDiscountCommand {
  name: string;
  percentage: number;
  startDate: string;
  endDate: string;
  membersOnly: boolean;
  isActive: boolean;
}

export interface UpdateDiscountCommand {
  name: string;
  percentage: number;
  startDate: string;
  endDate: string;
  membersOnly: boolean;
  isActive: boolean;
}
