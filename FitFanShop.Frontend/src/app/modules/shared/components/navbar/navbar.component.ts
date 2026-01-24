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
  isProfileDropdownOpen = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.nav-link-dropdown')) {
      this.isKatalogDropdownOpen = false;
    }
    if (!target.closest('.profile-dropdown')) {
      this.isProfileDropdownOpen = false;
    }
  }

  openLoginDialog(): void {
    this.dialog.open(LoginDialogComponent, {
      width: '400px',
      disableClose: false,
    });
  }

  toggleProfileDropdown(): void {
    this.isProfileDropdownOpen = !this.isProfileDropdownOpen;
  }

  closeProfileDropdown(): void {
    this.isProfileDropdownOpen = false;
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => {
        this.closeProfileDropdown();
        this.router.navigate(['/']);
        setTimeout(() => {
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }, 100);
      }
    });
  }

  toggleKatalogDropdown(): void {
    this.isKatalogDropdownOpen = !this.isKatalogDropdownOpen;
  }

  closeKatalogDropdown(): void {
    this.isKatalogDropdownOpen = false;
  }
}
