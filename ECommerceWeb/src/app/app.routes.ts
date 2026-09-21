import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/storefront-shell/storefront-shell').then((m) => m.StorefrontShell),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'products',
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/products/pages/catalog-page/catalog-page').then((m) => m.CatalogPage),
      },
      {
        path: '**',
        loadComponent: () =>
          import('./system/not-found-page/not-found-page').then((m) => m.NotFoundPage),
      },
    ],
  },
];
