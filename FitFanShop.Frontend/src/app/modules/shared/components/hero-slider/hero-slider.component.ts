import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-hero-slider',
  standalone: false,
  templateUrl: './hero-slider.component.html',
  styleUrls: ['./hero-slider.component.scss']
})
export class HeroSliderComponent implements OnInit, OnDestroy {
  currentSlide = 0;
  private autoSlideInterval: any;

  slides = [
    {
      title: 'UPOTPUNITE',
      subtitle: 'VAŠU KOLEKCIJU',
      buttonText: 'KUPI DRES',
      bgColor: '#f5f5f5',
      image: '/images/pictures/image-removebg-preview (1).png'
    },
    {
      title: 'POSTANI',
      subtitle: 'ČLAN',
      buttonText: 'POSTANI ČLAN',
      bgColor: '#e8e8e8'
    }
  ];

  ngOnInit() {
    this.startAutoSlide();
  }

  ngOnDestroy() {
    this.stopAutoSlide();
  }

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }

  prevSlide() {
    this.currentSlide = this.currentSlide === 0 ? this.slides.length - 1 : this.currentSlide - 1;
  }

  goToSlide(index: number) {
    this.currentSlide = index;
  }

  private startAutoSlide() {
    this.autoSlideInterval = setInterval(() => {
      this.nextSlide();
    }, 5000);
  }

  private stopAutoSlide() {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }
}
