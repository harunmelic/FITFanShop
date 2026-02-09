import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { FitConfirmDialogComponent } from '../../shared/components/fit-confirm-dialog/fit-confirm-dialog.component';
import { DialogType, DialogButton, DialogConfig } from '../../shared/models/dialog-config.model';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  standalone: false
})
export class AdminLayoutComponent implements OnInit {
  private dialog = inject(MatDialog);
  private auth = inject(AuthFacadeService);

  sidebarOpen = false;

  constructor(private router: Router) { }

  ngOnInit(): void {
    // Close sidebar on route change (mobile)
    this.router.events.subscribe(() => {
      this.sidebarOpen = false;
    });
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
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
            this.router.navigate(['/']);
          }
        });
      }
    });
  }

}
