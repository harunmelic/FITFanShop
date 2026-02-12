import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { myAuthGuard } from '../../core/guards/my-auth-guard';
import { ProfileLayoutComponent } from './profile-layout/profile-layout.component';
import { MyProfileComponent } from './my-profile/my-profile.component';

const routes: Routes = [
  {
    path: '',
    component: ProfileLayoutComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true },
    children: [
      { path: '', component: MyProfileComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfileRoutingModule {}
