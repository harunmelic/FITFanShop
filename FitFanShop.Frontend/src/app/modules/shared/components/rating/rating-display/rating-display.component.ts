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

  get stars(): number[] {
    return [1, 2, 3, 4, 5];
  }

  getStarClass(star: number): string {
    const baseClass = `star-${this.size}`;
    
    if (this.rating >= star) {
      return `${baseClass} filled`;
    } else if (this.rating > star - 1) {
      return `${baseClass} half-filled`;
    } else {
      return `${baseClass} empty`;
    }
  }
}