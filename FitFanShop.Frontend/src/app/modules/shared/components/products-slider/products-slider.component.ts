import { Component } from '@angular/core';

@Component({
  selector: 'app-products-slider',
  standalone: false,
  templateUrl: './products-slider.component.html',
  styleUrls: ['./products-slider.component.scss']
})
export class ProductsSliderComponent {
  currentSlide = 0;

  slides = [
    {
      title: 'PONUDA DRESOVA',
      products: [
        { name: 'DRES FKS 23/24', price: '120KM', image: null },
        { name: 'DRES FKŽ 19/20', price: '50KM', image: null },
        { name: 'DRES FK VELEŽ', price: '100KM', image: null },
        { name: 'DRES NK TRAVNIK', price: '60KM', image: null }
      ]
    },
    {
      title: 'PONUDA TRENERKI',
      products: [
        { name: 'TRENERKA FKS 23/24', price: '150KM', image: null },
        { name: 'TRENERKA FKŽ 19/20', price: '130KM', image: null },
        { name: 'TRENERKA FK VELEŽ', price: '140KM', image: null },
        { name: 'TRENERKA NK TRAVNIK', price: '120KM', image: null }
      ]
    },
    {
      title: 'PONUDA OPREME',
      products: [
        { name: 'LOPTA NIKE', price: '80KM', image: null },
        { name: 'KOPAČKE ADIDAS', price: '200KM', image: null },
        { name: 'TORBA PUMA', price: '60KM', image: null },
        { name: 'RUKAVICE NIKE', price: '40KM', image: null }
      ]
    }
  ];

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }

  prevSlide() {
    this.currentSlide = this.currentSlide === 0 ? this.slides.length - 1 : this.currentSlide - 1;
  }

  goToSlide(index: number) {
    this.currentSlide = index;
  }
}
