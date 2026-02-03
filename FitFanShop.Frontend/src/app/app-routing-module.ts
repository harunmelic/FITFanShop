import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

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
<<<<<<< HEAD
    path: 'admin',
    loadChildren: () =>
      import('./modules/admin/admin.module').then(m => m.AdminModule)
  },
  // Default redirect to home (empty for now)
  { path: '', redirectTo: '/', pathMatch: 'full' },
=======
    path: 'catalog',
    loadChildren: () =>
      import('./modules/catalog/catalog-module').then(m => m.CatalogModule)
  },
>>>>>>> f8638512d7be740d29067c22058bc58a8eeb9eee
  // fallback 404
  { path: '**', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
