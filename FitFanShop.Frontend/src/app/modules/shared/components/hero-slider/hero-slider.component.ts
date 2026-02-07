import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-hero-slider',
  standalone: false,
  templateUrl: './hero-slider.component.html',
  styleUrls: ['./hero-slider.component.scss']
})
export class HeroSliderComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  currentSlide = 0;
  private autoSlideInterval: any;

  slides = [
    {
      title: 'COMPLETE',
      subtitle: 'YOUR COLLECTION',
      buttonText: 'BUY JERSEY',
      bgColor: '#f5f5f5',
      image: '/images/pictures/image-removebg-preview (1).png',
      actionType: 'catalog',
      categoryName: 'Jerseys'
    },
    {
      title: 'BECOME',
      subtitle: 'A MEMBER',
      buttonText: 'BECOME A MEMBER',
      bgColor: '#e8e8e8',
      actionType: 'membership'
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

  onSlideButtonClick(slide: any) {
    console.log('👆 Slide button clicked:', slide.actionType);
    
    if (slide.actionType === 'catalog' && slide.categoryName) {
      this.navigateToCatalog(slide.categoryName);
    } else if (slide.actionType === 'membership') {
      this.navigateToMembership();
    }
  }

  private navigateToCatalog(categoryName: string) {
    console.log('📦 Navigating to catalog with category:', categoryName);
    this.router.navigate(['/catalog'], { queryParams: { categoryName } });
  }

  private navigateToMembership() {
    // TODO: Navigate to membership page when implemented
    console.log('Navigate to membership');
  }
}
