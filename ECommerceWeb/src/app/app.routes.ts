import { Routes } from '@angular/router';

import { guestGuard } from './features/auth/guards/guest-guard';
import { customerGuard } from './features/auth/guards/customer-guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/storefront-shell/storefront-shell').then((m) => m.StorefrontShell),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/home/pages/home-page/home-page').then((m) => m.HomePage),
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/products/pages/catalog-page/catalog-page').then((m) => m.CatalogPage),
      },
      {
        path: 'products/:id',
        loadComponent: () =>
          import('./features/products/pages/product-detail-page/product-detail-page').then(
            (m) => m.ProductDetailPage,
          ),
      },
      {
        path: 'login',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./features/auth/pages/login-page/login-page').then((m) => m.LoginPage),
      },
      {
        path: 'register',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./features/auth/pages/register-page/register-page').then((m) => m.RegisterPage),
      },
      {
        path: 'basket',
        canActivate: [customerGuard],
        loadComponent: () =>
          import('./features/basket/pages/basket-page/basket-page').then((m) => m.BasketPage),
      },
      {
        path: '**',
        loadComponent: () =>
          import('./system/not-found-page/not-found-page').then((m) => m.NotFoundPage),
      },
    ],
  },
];
