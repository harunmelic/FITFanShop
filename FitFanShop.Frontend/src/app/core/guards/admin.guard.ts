import { Injectable, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthFacadeService } from '../services/auth/auth-facade.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  private router = inject(Router);
  private authService = inject(AuthFacadeService);

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      // Not logged in, redirect to login page
      this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
      return false;
    }

    // Check if user has admin role
    if (!this.authService.isAdmin()) {
      // Logged in but not admin, redirect to home
      this.router.navigate(['/']);
      return false;
    }
    
    // User is authenticated and is admin
    return true;
  }
}
