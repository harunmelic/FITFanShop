import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: false
})
export class AdminLayoutComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  logout(): void {
    // TODO: Implementirati logout logiku
    // Primer:
    // this.authService.logout();
    
    // Preusmeravanje na home stranicu
    this.router.navigate(['/']);
  }

}
