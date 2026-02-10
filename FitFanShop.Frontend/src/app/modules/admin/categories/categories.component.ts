import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CategoryApiService } from '../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../api-services/catalog/category-api.model';
import { AddCategoryDialogComponent } from './add-category-dialog/add-category-dialog.component';
import { EditCategoryDialogComponent } from './edit-category-dialog/edit-category-dialog.component';
import { DeleteConfirmationDialogComponent } from '../products/delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss'],
  standalone: false
})
export class CategoriesComponent implements OnInit {
  categories: CategoryDto[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(
    private categoryApiService: CategoryApiService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.categoryApiService.getAll().subscribe({
      next: (response) => {
        this.categories = Array.isArray(response) ? response : [];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.errorMessage = 'Error loading categories';
        this.isLoading = false;
      }
    });
  }

  openAddCategoryDialog(): void {
    const dialogRef = this.dialog.open(AddCategoryDialogComponent, {
      width: '500px',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategories();
      }
    });
  }

  openEditCategoryDialog(category: CategoryDto): void {
    const dialogRef = this.dialog.open(EditCategoryDialogComponent, {
      width: '500px',
      disableClose: false,
      data: { category }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategories();
      }
    });
  }

  deleteCategory(category: CategoryDto): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '420px',
      panelClass: 'delete-confirmation-dialog-container',
      data: {
        title: 'Obriši kategoriju',
        message: 'Da li ste sigurni da želite da obrišete ovu kategoriju?',
        productName: category.name
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.categoryApiService.delete(category.id).subscribe({
          next: () => {
            this.loadCategories();
          },
          error: (error) => {
            console.error('Error deleting category:', error);
            this.errorMessage = 'Error deleting category';
          }
        });
      }
    });
  }
}
