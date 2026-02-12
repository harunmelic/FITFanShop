import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {FitPaginatorBarComponent} from './components/fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './material-modules';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import { FitConfirmDialogComponent } from './components/fit-confirm-dialog/fit-confirm-dialog.component';
import {DialogHelperService} from './services/dialog-helper.service';
import { FitLoadingBarComponent } from './components/fit-loading-bar/fit-loading-bar.component';
import { FitTableSkeletonComponent } from './components/fit-table-skeleton/fit-table-skeleton.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { LoginDialogComponent } from './components/login-dialog/login-dialog.component';
import { RegisterDialogComponent } from './components/register-dialog/register-dialog.component';
import { MembershipBannerComponent } from './components/membership-banner/membership-banner.component';
import { FooterComponent } from './components/footer/footer.component';
import { HeroSliderComponent } from './components/hero-slider/hero-slider.component';
import { FeaturesSectionComponent } from './components/features-section/features-section.component';
import { ProductsSliderComponent } from './components/products-slider/products-slider.component';
import { TestimonialsSectionComponent } from './components/testimonials-section/testimonials-section.component';
import { NewsletterSectionComponent } from './components/newsletter-section/newsletter-section.component';
import { ScrollToTopComponent } from './components/scroll-to-top/scroll-to-top.component';
import { LoadingScreenComponent } from './components/loading-screen/loading-screen.component';
import { ForgotPasswordDialogComponent } from './components/forgot-password-dialog/forgot-password-dialog.component';
import { CartSidebarComponent } from './components/cart-sidebar/cart-sidebar.component';
import { CartItemComponent } from './components/cart-item/cart-item.component';
import { CartSkeletonComponent } from './components/cart-skeleton/cart-skeleton.component';
import { ProductVariantSelectorComponent } from './components/product-variant-selector/product-variant-selector.component';
import { MembershipRequiredDialogComponent } from './components/membership-required-dialog/membership-required-dialog.component';
import { RatingDisplayComponent } from './components/rating/rating-display/rating-display.component';
import { RatingInputComponent } from './components/rating/rating-input/rating-input.component';
import { ReportReviewDialogComponent } from './components/report-review-dialog/report-review-dialog.component';
import { WishlistSidebarComponent } from './components/wishlist-sidebar/wishlist-sidebar.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { CheckoutSuccessComponent } from './components/checkout-success/checkout-success.component';



@NgModule({
  declarations: [
    FitPaginatorBarComponent,
    FitConfirmDialogComponent,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    NavbarComponent,
    LoginDialogComponent,
    RegisterDialogComponent,
    ForgotPasswordDialogComponent,
    MembershipBannerComponent,
    FooterComponent,
    HeroSliderComponent,
    FeaturesSectionComponent,
    ProductsSliderComponent,
    TestimonialsSectionComponent,
    NewsletterSectionComponent,
    ScrollToTopComponent,
    LoadingScreenComponent,
    CartSidebarComponent,
    CartItemComponent,
    CartSkeletonComponent,
    ProductVariantSelectorComponent,
    MembershipRequiredDialogComponent,
    ReportReviewDialogComponent,
    RatingDisplayComponent,
    RatingInputComponent,
    WishlistSidebarComponent,
    CheckoutComponent,
    CheckoutSuccessComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    ...materialModules
  ],
  providers: [
    DialogHelperService
  ],
  exports:[
    FitPaginatorBarComponent,
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    NavbarComponent,
    MembershipBannerComponent,
    FooterComponent,
    HeroSliderComponent,
    FeaturesSectionComponent,
    ProductsSliderComponent,
    TestimonialsSectionComponent,
    NewsletterSectionComponent,
    ScrollToTopComponent,
    LoadingScreenComponent,
    CartSidebarComponent,
    WishlistSidebarComponent,
    RatingDisplayComponent,
    RatingInputComponent,
    ...materialModules
  ]
})
export class SharedModule { }
