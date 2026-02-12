/**
 * User DTO model
 * Represents a user in the system
 */
export interface UserDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isEnabled: number; // Backend returns 1 (active) or 0 (inactive)
  isActive?: boolean; // Frontend convenience property
  createdAt?: string;
  updatedAt?: string;
}

/**
 * Command for creating a new user
 */
export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
  isEnabled: number; // 1 = active, 0 = inactive
}

/**
 * Command for updating an existing user
 */
export interface UpdateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isEnabled: number; // 1 = active, 0 = inactive
}

/**
 * Command for updating user password
 */
export interface UpdateUserPasswordCommand {
  newPassword: string;
  confirmPassword: string;
}

export interface UserProfileDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  isMember: boolean;
  isAdmin: boolean;
  registrationDate: string;
}

export interface UpdateMyProfileCommand {
  firstName: string;
  lastName: string;
}

export interface ChangeMyPasswordCommand {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
