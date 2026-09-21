import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ProductResponseDto } from '../../../products/data-access/product.dto';
import { ProductsApi } from '../../../products/data-access/products-api';
import { ProductCard } from '../../../products/ui/product-card/product-card';

@Component({
  selector: 'app-home-page',
  imports: [RouterLink, ProductCard],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage {
  private readonly productsApi = inject(ProductsApi);

  readonly featuredProducts = signal<ProductResponseDto[]>([]);
  readonly isFeaturedLoading = signal(true);
  readonly featuredError = signal<string | null>(null);

  constructor() {
    this.loadFeaturedProducts();
  }

  private loadFeaturedProducts(): void {
    this.isFeaturedLoading.set(true);
    this.featuredError.set(null);

    this.productsApi
      .list({
        page: 1,
        pageSize: 4,
      })
      .subscribe({
        next: (result) => {
          this.featuredProducts.set(result.items);
          this.isFeaturedLoading.set(false);
        },

        error: () => {
          this.isFeaturedLoading.set(false);
          this.featuredError.set('Featured products could not be loaded.');
        },
      });
  }
}
