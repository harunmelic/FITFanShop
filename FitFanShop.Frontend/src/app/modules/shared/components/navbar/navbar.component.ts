import { Component, inject, HostListener } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginDialogComponent } from '../login-dialog/login-dialog.component';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  private dialog = inject(MatDialog);
  private router = inject(Router);
  auth = inject(AuthFacadeService);

  isKatalogDropdownOpen = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.nav-link-dropdown')) {
      this.isKatalogDropdownOpen = false;
    }
  }

  openLoginDialog(): void {
    this.dialog.open(LoginDialogComponent, {
      width: '400px',
      disableClose: false,
    });
  }

  logout(): void {
    this.auth.logout();
  }

  toggleKatalogDropdown(): void {
    this.isKatalogDropdownOpen = !this.isKatalogDropdownOpen;
  }

  closeKatalogDropdown(): void {
    this.isKatalogDropdownOpen = false;
  }
}
