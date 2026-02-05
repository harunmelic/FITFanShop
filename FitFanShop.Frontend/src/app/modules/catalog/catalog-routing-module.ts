import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CatalogLayoutComponent } from './catalog-layout/catalog-layout.component';
import { CatalogPageComponent } from './catalog-page/catalog-page.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';

const routes: Routes = [
  {
    path: '',
    component: CatalogLayoutComponent,
    children: [
      {
        path: '',
        component: CatalogPageComponent
      },
      {
        path: 'product/:id',
        component: ProductDetailComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CatalogRoutingModule {}
