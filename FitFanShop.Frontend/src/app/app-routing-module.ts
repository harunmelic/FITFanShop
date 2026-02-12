import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutComponent } from './modules/shared/components/checkout/checkout.component';
import { CheckoutSuccessComponent } from './modules/shared/components/checkout-success/checkout-success.component';
import { myAuthGuard } from './core/guards/my-auth-guard';

const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./modules/home/home-module').then(m => m.HomeModule)
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./modules/auth/auth-module').then(m => m.AuthModule)
  },
  {
    path: 'catalog',
    loadChildren: () =>
      import('./modules/catalog/catalog-module').then(m => m.CatalogModule)
  },
  {
    path: 'event',
    loadChildren: () =>
      import('./modules/events/events-module').then(m => m.EventsModule)
  },
  {
    path: 'profile',
    loadChildren: () =>
      import('./modules/profile/profile.module').then(m => m.ProfileModule)
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./modules/admin/admin.module').then(m => m.AdminModule)
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true }
  },
  {
    path: 'checkout/success',
    component: CheckoutSuccessComponent,
    canActivate: [myAuthGuard],
    data: { requireAuth: true }
  },
  // fallback 404
  { path: '**', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
