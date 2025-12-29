import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-loading-screen',
  standalone: false,
  templateUrl: './loading-screen.component.html',
  styleUrls: ['./loading-screen.component.scss']
})
export class LoadingScreenComponent implements OnInit {
  isLoading = true;

  ngOnInit() {
    setTimeout(() => {
      this.isLoading = false;
    }, 3500); // 3.5 sekundi
  }
}
