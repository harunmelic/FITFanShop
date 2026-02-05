import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-membership-required-dialog',
  standalone: false,
  templateUrl: './membership-required-dialog.component.html',
  styleUrl: './membership-required-dialog.component.scss'
})
export class MembershipRequiredDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<MembershipRequiredDialogComponent>,
    private router: Router
  ) {}

  becomeMember(): void {
    this.dialogRef.close();
    this.router.navigate(['/auth/register']);
  }

  close(): void {
    this.dialogRef.close();
  }
}
