import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventLayoutComponent } from './event-layout/event-layout.component';
import { EventPageComponent } from './event-page/event-page.component';
import { EventDetailComponent } from './event-detail/event-detail.component';

const routes: Routes = [
  {
    path: '',
    component: EventLayoutComponent,
    children: [
      {
        path: '',
        component: EventPageComponent
      },
      {
        path: ':id',
        component: EventDetailComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventsRoutingModule {}
