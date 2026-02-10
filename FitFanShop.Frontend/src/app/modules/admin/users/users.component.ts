import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UserApiService } from '../../../api-services/users/user-api.service';
import { UserDto } from '../../../api-services/users/user-api.model';
import { AddUserDialogComponent } from './add-user-dialog/add-user-dialog.component';
import { EditUserDialogComponent } from './edit-user-dialog/edit-user-dialog.component';
import { DeleteConfirmationDialogComponent } from '../products/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { ToasterService } from '../../../core/services/toaster.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
  standalone: false
})
export class UsersComponent implements OnInit {
  users: UserDto[] = [];
  isLoading = false;
  errorMessage = '';
  private currentUserEmailCache: string | null | undefined = undefined;

  constructor(
    private userApiService: UserApiService,
    private dialog: MatDialog,
    private toasterService: ToasterService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    console.log('Loading users from API...');
    this.userApiService.getAll().subscribe({
      next: (response: any) => {
        console.log('Users API response:', response);
        // Backend returns paginated response with 'items' property
        const users = response?.items || [];
        // Map isEnabled (number 1/0) to isActive (boolean)
        this.users = users.map((user: any) => ({
          ...user,
          isActive: user.isEnabled === 1 || user.isEnabled === true
        }));
        console.log('Loaded users count:', this.users.length);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users - Full error:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        console.error('Error response:', error.error);
        
        if (error.status === 404) {
          this.errorMessage = 'Users endpoint not found (404). Backend may not have /api/users endpoint.';
        } else if (error.status === 401) {
          this.errorMessage = 'Unauthorized (401). Please login again.';
        } else if (error.status === 403) {
          this.errorMessage = 'Forbidden (403). Admin rights required.';
        } else if (error.status === 0) {
          this.errorMessage = 'Cannot connect to backend API. Is the backend running?';
        } else {
          this.errorMessage = `Error loading users (${error.status}): ${error.message}`;
        }
        this.isLoading = false;
      }
    });
  }

  openAddUserDialog(): void {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {
      width: '600px',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  openEditUserDialog(user: UserDto): void {
    const dialogRef = this.dialog.open(EditUserDialogComponent, {
      width: '600px',
      disableClose: false,
      data: { user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  deleteUser(user: UserDto): void {
    // Prevent admin from deleting themselves
    const currentUserEmail = this.getCurrentUserEmail();
    
    if (currentUserEmail && currentUserEmail === user.email) {
      this.toasterService.error('You cannot delete your own account!');
      return;
    }

    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '420px',
      panelClass: 'delete-confirmation-dialog-container',
      data: {
        title: 'Obriši korisnika',
        message: 'Da li ste sigurni da želite da obrišete ovog korisnika?',
        productName: `${user.firstName} ${user.lastName} (${user.email})`
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userApiService.delete(user.id).subscribe({
          next: () => {
            this.loadUsers();
          },
          error: (error) => {
            console.error('Error deleting user:', error);
            this.errorMessage = 'Error deleting user';
          }
        });
      }
    });
  }

  getRoleBadgeClass(role: string): string {
    return role?.toLowerCase() === 'admin' ? 'role-admin' : 'role-user';
  }

  isCurrentUser(user: UserDto): boolean {
    const currentUserEmail = this.getCurrentUserEmail();
    return currentUserEmail === user.email;
  }

  private getCurrentUserEmail(): string | null {
    // Cache to avoid multiple token decodings
    if (this.currentUserEmailCache !== undefined) {
      return this.currentUserEmailCache;
    }

    try {
      const token = localStorage.getItem('accessToken');
      if (!token) {
        this.currentUserEmailCache = null;
        return null;
      }
      
      const decoded: any = jwtDecode(token);
      
      // Microsoft JWT uses long claim names
      const emailClaimName = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
      const email = decoded[emailClaimName] || decoded.email || null;
      
      this.currentUserEmailCache = email;
      return email;
    } catch (error) {
      console.error('Error decoding token:', error);
      this.currentUserEmailCache = null;
      return null;
    }
  }
}
