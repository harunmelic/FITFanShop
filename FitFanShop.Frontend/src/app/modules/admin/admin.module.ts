import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './products/products.component';
import { PicturesHybridComponent } from './pictures/pictures-hybrid.component';
import { AddProductDialogComponent } from './products/add-product-dialog/add-product-dialog.component';
import { EditProductDialogComponent } from './products/edit-product-dialog/edit-product-dialog.component';
import { DeleteConfirmationDialogComponent } from './products/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { CategoriesComponent } from './categories/categories.component';
import { AddCategoryDialogComponent } from './categories/add-category-dialog/add-category-dialog.component';
import { EditCategoryDialogComponent } from './categories/edit-category-dialog/edit-category-dialog.component';
import { materialModules } from '../shared/material-modules';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    AdminLayoutComponent,
    ProductsComponent,
    PicturesHybridComponent,
    AddProductDialogComponent,
    EditProductDialogComponent,
    DeleteConfirmationDialogComponent,
    CategoriesComponent,
    AddCategoryDialogComponent,
    EditCategoryDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    ...materialModules
  ]
})
export class AdminModule { }
