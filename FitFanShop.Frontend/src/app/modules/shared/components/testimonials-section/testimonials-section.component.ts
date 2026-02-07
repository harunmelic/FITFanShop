import { Component } from '@angular/core';

@Component({
  selector: 'app-testimonials-section',
  standalone: false,
  templateUrl: './testimonials-section.component.html',
  styleUrls: ['./testimonials-section.component.scss']
})
export class TestimonialsSectionComponent {
  currentTestimonial = 0;

  testimonials = [
    {
      text: 'I\'m very satisfied with FitFanShop\'s services. I recommend everyone to order one of their products and see the quality yourself!',
      author: 'DANIS MAMELEDŽIJA',
      rating: 5
    },
    {
      text: 'Excellent jersey quality and fast delivery. I will definitely order again!',
      author: 'ABDULLAH MUSIĆ',
      rating: 5
    },
    {
      text: 'All praises for FitFanShop! Professional service and top-quality products.',
      author: 'HARUN MELIĆ',
      rating: 5
    }
  ];

  nextTestimonial() {
    this.currentTestimonial = (this.currentTestimonial + 1) % this.testimonials.length;
  }

  prevTestimonial() {
    this.currentTestimonial = this.currentTestimonial === 0 ? this.testimonials.length - 1 : this.currentTestimonial - 1;
  }

  getStars(rating: number): number[] {
    return Array(rating).fill(0);
  }
}
