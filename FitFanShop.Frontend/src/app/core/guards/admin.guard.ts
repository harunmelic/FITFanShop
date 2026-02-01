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
    
    // TODO: Implementirati proveru da li je korisnik admin
    // Primer:
    // const currentUser = this.authService.getCurrentUser();
    // if (currentUser && currentUser.role === 'ADMIN') {
    //   return true;
    // }
    
    // Za sada dozvoljavamo pristup svima (placeholder)
    // Kasnije ćete ovde dodati logiku za proveru admin role
    const isAdmin = this.checkIfUserIsAdmin();
    
    if (!isAdmin) {
      // Preusmeravanje na login ili home stranicu
      this.router.navigate(['/']);
      return false;
    }
    
    return true;
  }

  private checkIfUserIsAdmin(): boolean {
    // TODO: Implementirati pravu proveru
    // Primer:
    // const user = this.currentUserService.getCurrentUser();
    // return user?.role === 'ADMIN';
    
    // Za sada vraćamo true kako bi admin panel bio dostupan
    // Promenite ovo kasnije kada implementirate autentifikaciju
    return true;
  }
}
