import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginCommand } from '../../../../api-services/auth/auth-api.model';

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
      },
      error: (err) => {
        this.stopLoading('Pogrešan email ili šifra.');
        console.error('Login error:', err);
      },
    });
  }

  onRegister(): void {
    // TODO: Implementiraj registraciju
    console.log('Register clicked');
  }

  close(): void {
    this.dialogRef.close();
  }
}
