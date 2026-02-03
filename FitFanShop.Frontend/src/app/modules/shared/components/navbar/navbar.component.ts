import { Component, inject, HostListener, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginDialogComponent } from '../login-dialog/login-dialog.component';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../../api-services/catalog/category-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit {
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private categoryService = inject(CategoryApiService);
  auth = inject(AuthFacadeService);
  cartService = inject(CartService);

  categories = signal<CategoryDto[]>([]);
  isKatalogDropdownOpen = false;
  isProfileDropdownOpen = false;

  ngOnInit(): void {
    this.loadCategories();
    // Korpa se automatski učitava u CartService-u kada se korisnik prijavi
  }

  loadCategories(): void {
    this.categoryService.getAll(1, 100).subscribe({
      next: (categories) => {
        this.categories.set(categories.filter(c => c.isEnabled));
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

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

  scrollToFooter(): void {
    const footer = document.querySelector('.footer');
    if (footer) {
      footer.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
