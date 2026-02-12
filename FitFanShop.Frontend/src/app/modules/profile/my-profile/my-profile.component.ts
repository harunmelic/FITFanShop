import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { UserApiService } from '../../../api-services/users/user-api.service';
import { UserProfileDto } from '../../../api-services/users/user-api.model';
import { ToasterService } from '../../../core/services/toaster.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { MyOrderHistoryDto, OrderApiService } from '../../../api-services/commerce/order-api.service';

@Component({
  selector: 'app-my-profile',
  standalone: false,
  templateUrl: './my-profile.component.html',
  styleUrl: './my-profile.component.scss'
})
export class MyProfileComponent implements OnInit {
  private userApi = inject(UserApiService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);
  private auth = inject(AuthFacadeService);
  private orderApi = inject(OrderApiService);

  profile = signal<UserProfileDto | null>(null);
  isProfileLoading = signal<boolean>(true);
  isProfileSaving = signal<boolean>(false);
  isPasswordSaving = signal<boolean>(false);
  activeSection = signal<'info' | 'security' | 'orders'>('info');
  orders = signal<MyOrderHistoryDto[]>([]);
  isOrdersLoading = signal<boolean>(false);
  ordersPage = signal<number>(1);
  ordersTotalPages = signal<number>(1);
  readonly ordersPageSize = 5;

  profileForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: [{ value: '', disabled: true }]
  });

  passwordForm = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/[A-Z]/),
        Validators.pattern(/[a-z]/),
        Validators.pattern(/\d/)
      ]
    ],
    confirmNewPassword: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isProfileLoading.set(true);

    this.userApi.getMyProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.profileForm.patchValue({
          firstName: profile.firstName,
          lastName: profile.lastName,
          email: profile.email
        });
        this.isProfileLoading.set(false);
      },
      error: () => {
        this.toaster.error('Failed to load profile. Please try again.');
        this.isProfileLoading.set(false);
      }
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid || this.isProfileSaving()) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const firstName = this.profileForm.controls.firstName.value.trim();
    const lastName = this.profileForm.controls.lastName.value.trim();

    this.isProfileSaving.set(true);
    this.userApi.updateMyProfile({ firstName, lastName }).subscribe({
      next: (updatedProfile) => {
        this.profile.set(updatedProfile);
        this.profileForm.patchValue({
          firstName: updatedProfile.firstName,
          lastName: updatedProfile.lastName,
          email: updatedProfile.email
        });
        this.auth.updateCurrentUserName(updatedProfile.firstName, updatedProfile.lastName);

        this.toaster.success('Profile updated successfully.');
        this.isProfileSaving.set(false);
      },
      error: () => {
        this.toaster.error('Failed to update profile. Please try again.');
        this.isProfileSaving.set(false);
      }
    });
  }

  savePassword(): void {
    if (this.passwordForm.invalid || this.isPasswordSaving()) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const { currentPassword, newPassword, confirmNewPassword } = this.passwordForm.getRawValue();

    if (newPassword !== confirmNewPassword) {
      this.toaster.error('New password and confirmation do not match.');
      return;
    }

    this.isPasswordSaving.set(true);
    this.userApi.changeMyPassword({ currentPassword, newPassword, confirmNewPassword }).subscribe({
      next: () => {
        this.passwordForm.reset();
        this.toaster.success('Password changed successfully.');
        this.isPasswordSaving.set(false);
      },
      error: () => {
        this.toaster.error('Failed to change password. Please check your current password.');
        this.isPasswordSaving.set(false);
      }
    });
  }

  get firstNameControl() {
    return this.profileForm.controls.firstName;
  }

  get lastNameControl() {
    return this.profileForm.controls.lastName;
  }

  get currentPasswordControl() {
    return this.passwordForm.controls.currentPassword;
  }

  get newPasswordControl() {
    return this.passwordForm.controls.newPassword;
  }

  get confirmNewPasswordControl() {
    return this.passwordForm.controls.confirmNewPassword;
  }

  showInfoSection(): void {
    this.activeSection.set('info');
  }

  showSecuritySection(): void {
    this.activeSection.set('security');
  }

  showOrdersSection(): void {
    this.activeSection.set('orders');
    if (this.orders().length === 0) {
      this.loadOrders(1);
    }
  }

  loadOrders(page: number): void {
    this.isOrdersLoading.set(true);

    this.orderApi.getMyOrderHistory(page, this.ordersPageSize).subscribe({
      next: (result) => {
        this.orders.set(result.items);
        this.ordersPage.set(result.currentPage || page);
        this.ordersTotalPages.set(result.totalPages || 1);
        this.isOrdersLoading.set(false);
      },
      error: () => {
        this.orders.set([]);
        this.toaster.error('Failed to load order history. Please try again.');
        this.isOrdersLoading.set(false);
      }
    });
  }

  nextOrdersPage(): void {
    const current = this.ordersPage();
    const total = this.ordersTotalPages();
    if (current >= total) {
      return;
    }
    this.loadOrders(current + 1);
  }

  prevOrdersPage(): void {
    const current = this.ordersPage();
    if (current <= 1) {
      return;
    }
    this.loadOrders(current - 1);
  }
}
