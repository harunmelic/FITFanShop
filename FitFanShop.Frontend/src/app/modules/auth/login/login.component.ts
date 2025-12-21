import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LoginDialogComponent } from '../../shared/components/login-dialog/login-dialog.component';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private dialog = inject(MatDialog);
  private router = inject(Router);

  ngOnInit(): void {
    // Otvori modal i vrati se na početnu
    this.router.navigate(['/']);
    setTimeout(() => {
      this.dialog.open(LoginDialogComponent, {
        width: '400px',
        disableClose: false,
      });
    }, 100);
  }
}
