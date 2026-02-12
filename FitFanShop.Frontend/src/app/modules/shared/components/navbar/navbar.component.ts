import { Component, inject, HostListener, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { LoginDialogComponent } from '../login-dialog/login-dialog.component';
import { ProductApiService } from '../../../../api-services/catalog/product-api.service';
import { ProductDto } from '../../../../api-services/catalog/product-api.model';
import { CartService } from '../../../../core/services/cart/cart.service';
import { WishlistService } from '../../../../core/services/wishlist/wishlist.service';
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
  private productService = inject(ProductApiService);
  auth = inject(AuthFacadeService);
  cartService = inject(CartService);
  wishlistService = inject(WishlistService);

  isProfileDropdownOpen = false;
  
  // Search functionality
  isSearchOpen = signal<boolean>(false);
  searchControl = new FormControl('');
  searchResults = signal<ProductDto[]>([]);
  isSearching = signal<boolean>(false);

  ngOnInit(): void {
    this.setupSearchListener();
    // Cart and Wishlist are automatically loaded in their services when user logs in
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

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.isClickInsideProfileDropdown(event)) {
      this.isProfileDropdownOpen = false;
    }
  }

  private isClickInsideProfileDropdown(event: MouseEvent): boolean {
    const path = (event.composedPath?.() ?? []) as Array<EventTarget>;
    for (const node of path) {
      if (!(node instanceof HTMLElement)) {
        continue;
      }

      if (node.classList.contains('profile-dropdown')) {
        return true;
      }
    }

    const target = event.target as HTMLElement | null;
    return !!target?.closest('.profile-dropdown');
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
