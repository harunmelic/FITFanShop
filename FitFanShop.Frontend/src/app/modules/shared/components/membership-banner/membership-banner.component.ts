import { Component } from '@angular/core';

@Component({
  selector: 'app-membership-banner',
  standalone: false,
  templateUrl: './membership-banner.component.html',
  styleUrls: ['./membership-banner.component.scss']
})
export class MembershipBannerComponent {
  onJoinClick() {
    // TODO: Implement join membership logic
    console.log('Postani član clicked');
  }
}
