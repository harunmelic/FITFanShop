export interface CurrentUserDto {
  userId: number;
  email: string;
  firstName?: string;
  lastName?: string;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  isMember: boolean; // Premium membership status for 10% discount
  tokenVersion: number;
}
