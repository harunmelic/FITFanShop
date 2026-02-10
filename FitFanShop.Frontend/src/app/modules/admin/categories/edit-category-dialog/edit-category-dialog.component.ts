import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { CategoryDto } from '../../../../api-services/catalog/category-api.model';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-edit-category-dialog',
  templateUrl: './edit-category-dialog.component.html',
  styleUrls: ['./edit-category-dialog.component.scss'],
  standalone: false
})
export class EditCategoryDialogComponent {
  categoryForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EditCategoryDialogComponent>,
    private categoryApiService: CategoryApiService,
    private toasterService: ToasterService,
    @Inject(MAT_DIALOG_DATA) public data: { category: CategoryDto }
  ) {
    this.categoryForm = this.fb.group({
      name: [data.category.name, [Validators.required, Validators.minLength(2)]],
      description: [data.category.description || ''],
      isEnabled: [data.category.isEnabled ?? true]
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.categoryForm.value;

    this.categoryApiService.update(this.data.category.id, formValue).subscribe({
      next: (category) => {
        this.toasterService.success('Category updated successfully!');
        this.dialogRef.close(category);
      },
      error: (error) => {
        console.error('Error updating category:', error);
        if (error.status === 400) {
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Session expired. Please login again.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else if (error.status === 404) {
          this.toasterService.error('Category not found.');
        } else {
          this.toasterService.error('Error updating category.');
        }
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  get f() {
    return this.categoryForm.controls;
  }
}
