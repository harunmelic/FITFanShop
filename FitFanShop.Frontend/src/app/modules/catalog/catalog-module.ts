import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogRoutingModule } from './catalog-routing-module';
import { CategoryPageComponent } from './category-page/category-page.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  declarations: [
    CategoryPageComponent
  ],
  imports: [
    CommonModule,
    CatalogRoutingModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ]
})
export class CatalogModule {}
