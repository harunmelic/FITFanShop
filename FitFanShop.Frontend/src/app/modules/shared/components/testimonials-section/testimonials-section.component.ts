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
      text: 'Prezadovoljan sam uslugama FitFanShopa. Preporučujem svakom da poruči neki od proizvoda i sam se uvjeri u kvalitet!',
      author: 'DANIS MAMELEDŽIJA',
      rating: 5
    },
    {
      text: 'Odličan kvalitet dresova i brza dostava. Definitivno ću ponovo naručivati!',
      author: 'ABDULLAH MUSIĆ',
      rating: 5
    },
    {
      text: 'Sve pohvale za FitFanShop! Profesionalna usluga i vrhunski proizvodi.',
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
