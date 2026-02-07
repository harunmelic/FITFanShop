import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    // TODO: Implement admin user verification
    // Example:
    // const currentUser = this.authService.getCurrentUser();
    // if (currentUser && currentUser.role === 'ADMIN') {
    //   return true;
    // }
    
    // For now, allow access to everyone (placeholder)
    // Later add logic to verify admin role
    const isAdmin = this.checkIfUserIsAdmin();
    
    if (!isAdmin) {
      // Redirect to login or home page
      this.router.navigate(['/']);
      return false;
    }
    
    return true;
  }

  private checkIfUserIsAdmin(): boolean {
    // TODO: Implement proper verification
    // Example:
    // const user = this.currentUserService.getCurrentUser();
    // return user?.role === 'ADMIN';
    
    // For now return true to make admin panel accessible
    // Change this later when implementing authentication
    return true;
  }
}
