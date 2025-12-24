import { Component } from '@angular/core';

@Component({
  selector: 'app-newsletter-section',
  standalone: false,
  templateUrl: './newsletter-section.component.html',
  styleUrls: ['./newsletter-section.component.scss']
})
export class NewsletterSectionComponent {
  email: string = '';

  onSubmit() {
    if (this.email) {
      console.log('Newsletter subscription:', this.email);
      // TODO: Implement newsletter subscription logic
      this.email = '';
    }
  }
}
