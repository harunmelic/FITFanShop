import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-rating-display',
  standalone: false,
  templateUrl: './rating-display.component.html',
  styleUrl: './rating-display.component.scss'
})
export class RatingDisplayComponent {
  @Input() rating: number = 0;
  @Input() readonly: boolean = true;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';

  private get normalizedRating(): number {
    const safeRating = Math.max(0, Math.min(5, this.rating || 0));
    return Math.round(safeRating * 2) / 2;
  }

  get stars(): number[] {
    return [1, 2, 3, 4, 5];
  }

  getStarClass(star: number): string {
    const baseClass = `star-${this.size}`;

    if (this.normalizedRating >= star) {
      return `${baseClass} filled`;
    } else if (this.normalizedRating === star - 0.5) {
      return `${baseClass} half-filled`;
    } else {
      return `${baseClass} empty`;
    }
  }
}