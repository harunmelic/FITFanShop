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
      title: 'BRZA DOSTAVA',
      description: 'Poručite i kroz 48h proizvod je na vašoj adresi.'
    },
    {
      icon: 'verified',
      title: 'KVALITET',
      description: 'Izrađeno od najkvalitetnijih materijala na tržištu.'
    },
    {
      icon: 'local_offer',
      title: 'SUPER PONUDA',
      description: 'Budite u toku sa popustima i raznim akcijama.'
    },
    {
      icon: 'lock',
      title: 'SIGURNO PLAĆANJE',
      description: '100% zagarantovano sigurno plaćanje.'
    }
  ];
}
