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
    // TODO: Implement logout logic
    // Example:
    // this.authService.logout();
    
    // Redirect to home page
    this.router.navigate(['/']);
  }

}
