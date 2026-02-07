import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './products/products.component';
import { PicturesComponent } from './pictures/pictures.component';
import { AddProductDialogComponent } from './products/add-product-dialog/add-product-dialog.component';
import { materialModules } from '../shared/material-modules';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    AdminLayoutComponent,
    ProductsComponent,
    PicturesComponent,
    AddProductDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    ...materialModules
  ]
})
export class AdminModule { }
