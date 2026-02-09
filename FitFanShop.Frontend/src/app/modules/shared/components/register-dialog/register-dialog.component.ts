import { Component, inject, computed, signal } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { RegisterCommand } from '../../../../api-services/auth/auth-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

export interface PasswordStrength {
  score: number; // 0-4
  label: string;
  color: string;
  feedback: string[];
}

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

  // Password strength tracking
  private passwordValue = signal<string>('');
  passwordStrength = computed(() => this.calculatePasswordStrength(this.passwordValue()));

  // Predefined security questions
  securityQuestions = [
    'What is your first pet\'s name?',
    'What city were you born in?',
    'What is your favorite movie?',
    'What is your mother\'s maiden name?',
    'What is your childhood nickname?',
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

  constructor() {
    super();
    
    // Track password changes for strength meter
    this.accountInfoForm.get('password')?.valueChanges.subscribe(value => {
      this.passwordValue.set(value || '');
    });
  }

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
        this.toaster.success('Registration successful! Welcome!');
        this.dialogRef.close(true);
        setTimeout(() => {
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }, 100);
      },
      error: (err) => {
        console.error('Register error:', err);
        
        let errorMsg = 'Registration error. Please try again.';
        
        // Check for specific backend error messages
        if (err.error?.message) {
          errorMsg = err.error.message;
        } else if (err.error?.title) {
          errorMsg = err.error.title;
        } else if (err.status === 400) {
          errorMsg = 'Email is already taken.';
        } else if (err.status === 500) {
          errorMsg = 'Server error. Please try later.';
        }
        
        this.stopLoading(errorMsg);
        this.toaster.error(errorMsg);
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  // Password strength calculation
  private calculatePasswordStrength(password: string): PasswordStrength {
    if (!password) {
      return {
        score: 0,
        label: '',
        color: '#e0e0e0',
        feedback: []
      };
    }

    let score = 0;
    const feedback: string[] = [];

    // Length check
    if (password.length >= 8) {
      score += 1;
    } else {
      feedback.push('Use at least 8 characters');
    }

    // Uppercase letter
    if (/[A-Z]/.test(password)) {
      score += 1;
    } else {
      feedback.push('Add an uppercase letter');
    }

    // Lowercase letter
    if (/[a-z]/.test(password)) {
      score += 1;
    } else {
      feedback.push('Add a lowercase letter');
    }

    // Numbers
    if (/[0-9]/.test(password)) {
      score += 1;
    } else {
      feedback.push('Add a number');
    }

    // Special characters
    if (/[^A-Za-z0-9]/.test(password)) {
      score += 1;
    } else {
      feedback.push('Add a special character (!@#$%^&*)');
    }

    // Determine strength level
    let label: string;
    let color: string;

    if (score <= 1) {
      label = 'Very weak';
      color = '#f44336'; // Red
    } else if (score === 2) {
      label = 'Weak';
      color = '#ff9800'; // Orange
    } else if (score === 3) {
      label = 'Medium';
      color = '#ffc107'; // Yellow
    } else if (score === 4) {
      label = 'Strong';
      color = '#8bc34a'; // Light Green
    } else {
      label = 'Very strong';
      color = '#4caf50'; // Green
    }

    return {
      score,
      label,
      color,
      feedback: feedback.slice(0, 2) // Show max 2 suggestions
    };
  }
}
