import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './products/products.component';
import { PicturesComponent } from './pictures/pictures.component';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    AdminLayoutComponent,
    ProductsComponent,
    PicturesComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule
  ]
})
export class AdminModule { }
