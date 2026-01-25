import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginCommand } from '../../../../api-services/auth/auth-api.model';
import { RegisterDialogComponent } from '../register-dialog/register-dialog.component';
import { ForgotPasswordDialogComponent } from '../forgot-password-dialog/forgot-password-dialog.component';

@Component({
  selector: 'app-login-dialog',
  standalone: false,
  templateUrl: './login-dialog.component.html',
  styleUrl: './login-dialog.component.scss',
})
export class LoginDialogComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthFacadeService);
  private dialogRef = inject(MatDialogRef<LoginDialogComponent>);
  private dialog = inject(MatDialog);
  hidePassword = true;

  form = this.fb.group({
    email: ['admin@fitfanshop.com', [Validators.required, Validators.email]],
    password: ['admin123', [Validators.required]],
  });

  onLogin(): void {
    if (this.form.invalid || this.isLoading) return;

    this.startLoading();

    const payload: LoginCommand = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
      fingerprint: null,
    };

    this.auth.login(payload).subscribe({
      next: () => {
        this.stopLoading();
        this.dialogRef.close(true);
        setTimeout(() => {
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }, 100);
      },
      error: (err) => {
        this.stopLoading('Pogrešan email ili šifra.');
        console.error('Login error:', err);
      },
    });
  }

  onRegister(): void {
    this.dialogRef.close();
    this.dialog.open(RegisterDialogComponent, {
      width: '500px',
      disableClose: false,
    });
  }

  onForgotPassword(): void {
    this.dialogRef.close();
    this.dialog.open(ForgotPasswordDialogComponent, {
      width: '500px',
      disableClose: false,
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
