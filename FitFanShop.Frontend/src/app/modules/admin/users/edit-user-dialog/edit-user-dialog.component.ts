import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserApiService } from '../../../../api-services/users/user-api.service';
import { UserDto } from '../../../../api-services/users/user-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-edit-user-dialog',
  templateUrl: './edit-user-dialog.component.html',
  styleUrls: ['./edit-user-dialog.component.scss'],
  standalone: false
})
export class EditUserDialogComponent {
  userForm: FormGroup;
  isSubmitting = false;
  roles = ['User', 'Admin'];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditUserDialogComponent>,
    private userApiService: UserApiService,
    private toasterService: ToasterService,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto }
  ) {
    // Map backend isEnabled (1/0) to frontend isActive (boolean)
    const isActive = (data.user as any).isEnabled === 1 || data.user.isActive === true;
    this.userForm = this.fb.group({
      firstName: [data.user.firstName, [Validators.required, Validators.minLength(2)]],
      lastName: [data.user.lastName, [Validators.required, Validators.minLength(2)]],
      email: [data.user.email, [Validators.required, Validators.email]],
      role: [data.user.role, [Validators.required]],
      isActive: [isActive]
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = {
      firstName: this.userForm.value.firstName,
      lastName: this.userForm.value.lastName,
      email: this.userForm.value.email,
      role: this.userForm.value.role,
      isEnabled: this.userForm.value.isActive ? 1 : 0
    };

    console.log('Updating user ID:', this.data.user.id);
    console.log('Update payload:', formValue);

    this.userApiService.update(this.data.user.id, formValue).subscribe({
      next: (user) => {
        this.toasterService.success('User updated successfully!');
        this.dialogRef.close(user);
      },
      error: (error) => {
        console.error('Error updating user:', error);
        console.error('Error status:', error.status);
        console.error('Error response:', error.error);
        if (error.status === 400) {
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Session expired. Please login again.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else if (error.status === 405) {
          this.toasterService.error('Method not allowed. Backend may not support user updates.');
        } else if (error.status === 404) {
          this.toasterService.error('User not found.');
        } else if (error.status === 409) {
          this.toasterService.error('User with this email already exists.');
        } else {
          this.toasterService.error('Error updating user.');
        }
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  get f() {
    return this.userForm.controls;
  }
}
