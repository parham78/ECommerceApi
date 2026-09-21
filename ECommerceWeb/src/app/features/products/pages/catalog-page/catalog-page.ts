import { Component, inject, OnInit, signal } from '@angular/core';
import { finalize } from 'rxjs';

import { ProductResponseDto } from '../../data-access/product.dto';
import { ProductsApi } from '../../data-access/products-api';
import { ProductCard } from '../../ui/product-card/product-card';

@Component({
  selector: 'app-catalog-page',
  imports: [ProductCard],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.scss',
})
export class CatalogPage implements OnInit {
  private readonly productsApi = inject(ProductsApi);

  readonly products = signal<ProductResponseDto[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productsApi
      .list({
        page: 1,
        pageSize: 12,
      })
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        }),
      )
      .subscribe({
        next: (result) => {
          this.products.set(result.items);
        },
        error: () => {
          this.products.set([]);
          this.errorMessage.set('We could not load products. Please try again.');
        },
      });
  }
}
