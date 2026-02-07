import { Component } from '@angular/core';

@Component({
  selector: 'app-features-section',
  standalone: false,
  templateUrl: './features-section.component.html',
  styleUrls: ['./features-section.component.scss']
})
export class FeaturesSectionComponent {
  features = [
    {
      icon: 'shopping_cart',
      title: 'FAST DELIVERY',
      description: 'Order and get your product delivered to your address within 48 hours.'
    },
    {
      icon: 'verified',
      title: 'QUALITY',
      description: 'Made from the highest quality materials on the market.'
    },
    {
      icon: 'local_offer',
      title: 'GREAT OFFERS',
      description: 'Stay updated with discounts and various promotions.'
    },
    {
      icon: 'lock',
      title: 'SECURE PAYMENT',
      description: '100% guaranteed secure payment.'
    }
  ];
}
