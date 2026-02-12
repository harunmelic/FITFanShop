import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';
import { EventsRoutingModule } from './events-routing-module';
import { EventLayoutComponent } from './event-layout/event-layout.component';
import { EventPageComponent } from './event-page/event-page.component';
import { EventDetailComponent } from './event-detail/event-detail.component';

@NgModule({
  declarations: [
    EventLayoutComponent,
    EventPageComponent,
    EventDetailComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule,
    EventsRoutingModule
  ]
})
export class EventsModule {}
