import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ProductResponseDto } from '../../data-access/product.dto';
import { ProductsApi } from '../../data-access/products-api';
import { ProductImage } from '../../ui/product-image/product-image';

@Component({
  selector: 'app-product-detail-page',
  imports: [CurrencyPipe, ProductImage, RouterLink],
  templateUrl: './product-detail-page.html',
  styleUrl: './product-detail-page.scss',
})
export class ProductDetailPage {
  private readonly route = inject(ActivatedRoute);
  private readonly productsApi = inject(ProductsApi);

  private readonly productId = Number(this.route.snapshot.paramMap.get('id'));

  readonly product = signal<ProductResponseDto | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly canRetry = signal(false);

  constructor() {
    if (!Number.isInteger(this.productId) || this.productId <= 0) {
      this.isLoading.set(false);
      this.errorMessage.set('Invalid product.');
      return;
    }

    this.loadProduct();
  }

  retry(): void {
    this.loadProduct();
  }

  private loadProduct(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.canRetry.set(false);

    this.productsApi.getById(this.productId).subscribe({
      next: (product) => {
        this.product.set(product);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);

        if (error.status === 404) {
          this.errorMessage.set('Product could not be found.');
          return;
        }

        this.errorMessage.set('We could not load this product. Please try again.');

        this.canRetry.set(true);
      },
    });
  }
}
