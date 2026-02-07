import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'FitFanShop Frontend';
  isAdminRoute = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Praćenje promene rute
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.isAdminRoute = event.url.startsWith('/admin');
      });

    // Provera inicijalne rute
    this.isAdminRoute = this.router.url.startsWith('/admin');
  }
}
