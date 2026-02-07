import { Component, inject, HostListener, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginDialogComponent } from '../login-dialog/login-dialog.component';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../../api-services/catalog/category-api.model';
import { ProductApiService } from '../../../../api-services/catalog/product-api.service';
import { ProductDto } from '../../../../api-services/catalog/product-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';
import { FitConfirmDialogComponent } from '../fit-confirm-dialog/fit-confirm-dialog.component';
import { DialogType, DialogButton, DialogConfig } from '../../models/dialog-config.model';

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
  private productService = inject(ProductApiService);
  auth = inject(AuthFacadeService);
  cartService = inject(CartService);

  categories = signal<CategoryDto[]>([]);
  isKatalogDropdownOpen = false;
  isProfileDropdownOpen = false;
  
  // Search functionality
  isSearchOpen = signal<boolean>(false);
  searchControl = new FormControl('');
  searchResults = signal<ProductDto[]>([]);
  isSearching = signal<boolean>(false);

  ngOnInit(): void {
    this.loadCategories();
    this.setupSearchListener();
    // Cart is automatically loaded in CartService when user logs in
  }

  setupSearchListener(): void {
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe((searchTerm) => {
      if (searchTerm && searchTerm.trim().length >= 2) {
        this.performSearch(searchTerm.trim());
      } else {
        this.searchResults.set([]);
      }
    });
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
    const dialogConfig: DialogConfig = {
      type: DialogType.QUESTION,
      title: 'Logout',
      message: 'Are you sure you want to logout?',
      buttons: [
        { type: DialogButton.CANCEL, label: 'Cancel' },
        { type: DialogButton.YES, label: 'Confirm' }
      ]
    };

    const dialogRef = this.dialog.open(FitConfirmDialogComponent, {
      width: '400px',
      data: dialogConfig
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.button === DialogButton.YES) {
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
    });
  }

  toggleKatalogDropdown(): void {
    this.isKatalogDropdownOpen = !this.isKatalogDropdownOpen;
  }

  closeKatalogDropdown(): void {
    this.isKatalogDropdownOpen = false;
  }

  navigateToCatalog(categoryId?: number): void {
    this.closeKatalogDropdown();
    
    if (categoryId) {
      this.router.navigate(['/catalog'], { queryParams: { categoryId } });
    } else {
      this.router.navigate(['/catalog']);
    }
    
    // Scroll to top after navigation
    setTimeout(() => {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }, 100);
  }

  scrollToFooter(): void {
    const footer = document.querySelector('.footer');
    if (footer) {
      footer.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  toggleSearch(): void {
    this.isSearchOpen.set(!this.isSearchOpen());
    if (this.isSearchOpen()) {
      // Focus search input after a short delay
      setTimeout(() => {
        const searchInput = document.querySelector('.search-input') as HTMLInputElement;
        if (searchInput) {
          searchInput.focus();
        }
      }, 100);
    } else {
      // Clear search when closing
      this.searchControl.setValue('');
      this.searchResults.set([]);
    }
  }

  performSearch(searchTerm: string): void {
    this.isSearching.set(true);
    
    this.productService.getAll({
      isEnabled: true,
      page: 1,
      pageSize: 50
    }).subscribe({
      next: (products) => {
        const results = products.filter(p => 
          p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.description?.toLowerCase().includes(searchTerm.toLowerCase())
        ).slice(0, 4); // Limit to 4 results
        
        this.searchResults.set(results);
        this.isSearching.set(false);
      },
      error: (error) => {
        console.error('Search error:', error);
        this.isSearching.set(false);
      }
    });
  }

  navigateToProduct(productId: number): void {
    this.toggleSearch();
    this.router.navigate(['/catalog/product', productId]);
  }

  viewAllResults(): void {
    const searchTerm = this.searchControl.value;
    this.toggleSearch();
    this.router.navigate(['/catalog'], { queryParams: { search: searchTerm } });
  }
}
