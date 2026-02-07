import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-forgot-password-dialog',
  standalone: false,
  templateUrl: './forgot-password-dialog.component.html',
  styleUrl: './forgot-password-dialog.component.scss',
})
export class ForgotPasswordDialogComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthFacadeService);
  private toaster = inject(ToasterService);
  private dialogRef = inject(MatDialogRef<ForgotPasswordDialogComponent>);

  step: 'verify' | 'reset' = 'verify';
  securityQuestion = '';
  resetToken = '';
  hideNewPassword = true;
  hideConfirmPassword = true;

  // Step 1: Verify Form
  verifyForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    securityAnswer: ['', [Validators.required, Validators.minLength(2)]],
  });

  // Step 2: Reset Password Form
  resetForm = this.fb.group(
    {
      newPassword: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[A-Z]).*$/)
      ]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: ForgotPasswordDialogComponent.passwordMatchValidator }
  );

  private static passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (newPassword && confirmPassword && newPassword !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onGetSecurityQuestion(): void {
    const email = this.verifyForm.get('email')?.value;
    if (!email || this.verifyForm.get('email')?.invalid) {
      this.toaster.error('Unesite validan email');
      return;
    }

    this.startLoading();

    this.auth.getSecurityQuestion(email).subscribe({
      next: (response) => {
        this.securityQuestion = response.securityQuestion;
        this.stopLoading();
        this.toaster.info('Odgovorite na sigurnosno pitanje');
      },
      error: (err) => {
        console.error('Get security question error:', err);
        let errorMsg = 'Greška prilikom dobijanja sigurnosnog pitanja.';
        
        if (err.status === 404) {
          errorMsg = 'Korisnik sa ovim emailom ne postoji.';
        } else if (err.error?.message) {
          errorMsg = err.error.message;
        }
        
        this.stopLoading();
        this.toaster.error(errorMsg);
      },
    });
  }

  onVerify(): void {
    if (this.verifyForm.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const email = this.verifyForm.value.email ?? '';
    const securityAnswer = this.verifyForm.value.securityAnswer ?? '';

    this.auth.verifySecurityAnswer(email, securityAnswer).subscribe({
      next: (response) => {
        this.stopLoading();
        
        if (response.isValid) {
          this.resetToken = response.resetToken || '';
          this.step = 'reset';
          this.toaster.success('Verifikacija uspešna! Unesite novu šifru.');
        } else {
          const errorMsg = response.message || 'Netačan odgovor na sigurnosno pitanje.';
          this.toaster.error(errorMsg);
        }
      },
      error: (err) => {
        console.error('Verify error:', err);
        let errorMsg = 'Netačan odgovor na sigurnosno pitanje.';
        
        if (err.error?.message) {
          errorMsg = err.error.message;
        } else if (err.status === 400) {
          errorMsg = 'Netačan odgovor na sigurnosno pitanje.';
        }
        
        this.stopLoading();
        this.toaster.error(errorMsg);
      },
    });
  }

  onResetPassword(): void {
    if (this.resetForm.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const email = this.verifyForm.value.email ?? '';
    const newPassword = this.resetForm.value.newPassword ?? '';
    const confirmNewPassword = this.resetForm.value.confirmPassword ?? '';

    this.auth.resetPassword(email, this.resetToken, newPassword, confirmNewPassword).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Šifra uspešno promenjena! Prijavite se sa novom šifrom.');
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.error('Reset password error:', err);
        let errorMsg = 'Greška pri promeni šifre. Pokušajte ponovo.';
        
        if (err.error?.message) {
          errorMsg = err.error.message;
        } else if (err.status === 400) {
          errorMsg = 'Nevažeći zahtev. Proverite unete podatke.';
        }
        
        this.stopLoading();
        this.toaster.error(errorMsg);
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
