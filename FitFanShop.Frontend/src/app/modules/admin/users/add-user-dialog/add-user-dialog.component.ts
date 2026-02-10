import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { UserApiService } from '../../../../api-services/users/user-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-add-user-dialog',
  templateUrl: './add-user-dialog.component.html',
  styleUrls: ['./add-user-dialog.component.scss'],
  standalone: false
})
export class AddUserDialogComponent {
  userForm: FormGroup;
  isSubmitting = false;
  roles = ['User', 'Admin'];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddUserDialogComponent>,
    private userApiService: UserApiService,
    private toasterService: ToasterService
  ) {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      role: ['User', [Validators.required]],
      isActive: [true]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = {
      ...this.userForm.value,
      isEnabled: this.userForm.value.isActive ? 1 : 0
    };
    delete (formValue as any).isActive;

    this.userApiService.create(formValue).subscribe({
      next: (user) => {
        this.toasterService.success('User created successfully!');
        this.dialogRef.close(user);
      },
      error: (error) => {
        console.error('Error creating user:', error);
        if (error.status === 400) {
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Session expired. Please login again.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else if (error.status === 409) {
          this.toasterService.error('User with this email already exists.');
        } else {
          this.toasterService.error('Error creating user.');
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
