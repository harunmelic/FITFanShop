import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { CategoryApiService } from '../../../../api-services/catalog/category-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-add-category-dialog',
  templateUrl: './add-category-dialog.component.html',
  styleUrls: ['./add-category-dialog.component.scss'],
  standalone: false
})
export class AddCategoryDialogComponent {
  categoryForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddCategoryDialogComponent>,
    private categoryApiService: CategoryApiService,
    private toasterService: ToasterService
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      isEnabled: [true]
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.categoryForm.value;

    this.categoryApiService.create(formValue).subscribe({
      next: (category) => {
        this.toasterService.success('Category created successfully!');
        this.dialogRef.close(category);
      },
      error: (error) => {
        console.error('Error creating category:', error);
        if (error.status === 400) {
          this.toasterService.error('Validation error: ' + (error.error?.title || 'Invalid data'));
        } else if (error.status === 401) {
          this.toasterService.error('Not authorized. Please login.');
        } else if (error.status === 403) {
          this.toasterService.error('Access denied. Admin rights required.');
        } else {
          this.toasterService.error('Error creating category.');
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
