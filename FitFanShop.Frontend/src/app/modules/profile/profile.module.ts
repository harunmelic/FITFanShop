import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileLayoutComponent } from './profile-layout/profile-layout.component';
import { MyProfileComponent } from './my-profile/my-profile.component';

@NgModule({
  declarations: [
    ProfileLayoutComponent,
    MyProfileComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule,
    ProfileRoutingModule
  ]
})
export class ProfileModule {}
