import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportReviewCommand } from '../../../../api-services/reviews/review-api.model';

export interface ReportDialogData {
  reviewId: number;
  userName: string;
}

@Component({
  selector: 'app-report-review-dialog',
  standalone: false,
  templateUrl: './report-review-dialog.component.html',
  styleUrl: './report-review-dialog.component.scss'
})
export class ReportReviewDialogComponent {
  form: FormGroup;

  reportReasons = [
    { value: 'spam', label: 'Spam ili reklama' },
    { value: 'inappropriate', label: 'Neprikladan sadržaj' },
    { value: 'fake', label: 'Lažna recenzija' },
    { value: 'offensive', label: 'Uvredljiv jezik' },
    { value: 'irrelevant', label: 'Nerelevantno za proizvod' },
    { value: 'other', label: 'Ostalo' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ReportReviewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReportDialogData
  ) {
    this.form = this.fb.group({
      reason: ['', Validators.required],
      description: ['']
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const payload: ReportReviewCommand = {
        reviewId: this.data.reviewId,
        reason: this.form.value.reason,
        description: this.form.value.description || undefined
      };
      
      this.dialogRef.close(payload);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}