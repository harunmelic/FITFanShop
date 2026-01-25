import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { RegisterCommand } from '../../../../api-services/auth/auth-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-register-dialog',
  standalone: false,
  templateUrl: './register-dialog.component.html',
  styleUrl: './register-dialog.component.scss',
})
export class RegisterDialogComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthFacadeService);
  private toaster = inject(ToasterService);
  private dialogRef = inject(MatDialogRef<RegisterDialogComponent>);

  hidePassword = true;
  hideConfirmPassword = true;

  // Predefinisana sigurnosna pitanja
  securityQuestions = [
    'Ime vašeg prvog ljubimca?',
    'Grad u kojem ste rođeni?',
    'Omiljeni film?',
    'Prezime majke prije udaje?',
    'Nadimak iz djetinjstva?',
  ];

  // Step 1: Personal Info
  personalInfoForm = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
  });

  // Step 2: Account Info
  accountInfoForm = this.fb.group(
    {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[A-Z]).*$/) // At least one uppercase letter
      ]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: this.passwordMatchValidator }
  );

  // Step 3: Security Question
  securityForm = this.fb.group({
    securityQuestion: ['', [Validators.required]],
    securityAnswer: ['', [Validators.required, Validators.minLength(2)]],
  });

  // Password match validator
  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password && confirmPassword && password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.personalInfoForm.invalid || this.accountInfoForm.invalid || this.securityForm.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const payload: RegisterCommand = {
      firstName: this.personalInfoForm.value.firstName ?? '',
      lastName: this.personalInfoForm.value.lastName ?? '',
      email: this.accountInfoForm.value.email ?? '',
      password: this.accountInfoForm.value.password ?? '',
      confirmPassword: this.accountInfoForm.value.confirmPassword ?? '',
      securityQuestion: this.securityForm.value.securityQuestion ?? '',
      securityAnswer: this.securityForm.value.securityAnswer ?? '',
    };

    this.auth.register(payload).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Uspešna registracija! Dobrodošli!');
        this.dialogRef.close(true);
        setTimeout(() => {
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }, 100);
      },
      error: (err) => {
        console.error('Register error:', err);
        
        let errorMsg = 'Greška pri registraciji. Pokušajte ponovo.';
        
        // Check for specific backend error messages
        if (err.error?.message) {
          errorMsg = err.error.message;
        } else if (err.error?.title) {
          errorMsg = err.error.title;
        } else if (err.status === 400) {
          errorMsg = 'Email je već zauzet.';
        } else if (err.status === 500) {
          errorMsg = 'Serverska greška. Pokušajte kasnije.';
        }
        
        this.stopLoading(errorMsg);
        this.toaster.error(errorMsg);
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
