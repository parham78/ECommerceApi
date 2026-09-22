import { Routes } from '@angular/router';

import { adminGuard } from './features/auth/guards/admin-guard';
import { customerGuard } from './features/auth/guards/customer-guard';
import { guestGuard } from './features/auth/guards/guest-guard';

export const routes: Routes = [
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./layout/admin-shell/admin-shell').then((m) => m.AdminShell),

    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/admin/pages/admin-overview-page/admin-overview-page').then(
            (m) => m.AdminOverviewPage,
          ),
      },

      {
        path: 'products',
        loadComponent: () =>
          import('./features/admin/pages/admin-products-page/admin-products-page').then(
            (m) => m.AdminProductsPage,
          ),
      },

      {
        path: 'orders',
        loadComponent: () =>
          import('./features/admin/pages/admin-orders-page/admin-orders-page').then(
            (m) => m.AdminOrdersPage,
          ),
      },
    ],
  },

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
        path: 'checkout',
        canActivate: [customerGuard],
        loadComponent: () =>
          import('./features/checkout/pages/checkout-page/checkout-page').then(
            (m) => m.CheckoutPage,
          ),
      },

      {
        path: 'addresses',
        canActivate: [customerGuard],
        loadComponent: () =>
          import('./features/addresses/pages/addresses-page/addresses-page').then(
            (m) => m.AddressesPage,
          ),
      },

      {
        path: 'orders',
        canActivate: [customerGuard],
        loadComponent: () =>
          import('./features/orders/pages/orders-page/orders-page').then((m) => m.OrdersPage),
      },

      {
        path: 'orders/:id',
        canActivate: [customerGuard],
        loadComponent: () =>
          import('./features/orders/pages/order-detail-page/order-detail-page').then(
            (m) => m.OrderDetailPage,
          ),
      },

      {
        path: '**',
        loadComponent: () =>
          import('./system/not-found-page/not-found-page').then((m) => m.NotFoundPage),
      },
    ],
  },
];
