import { CurrencyPipe } from '@angular/common';
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

  readonly product = signal<ProductResponseDto | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    const productId = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(productId) || productId <= 0) {
      this.isLoading.set(false);
      this.errorMessage.set('Invalid product.');
      return;
    }

    this.productsApi.getById(productId).subscribe({
      next: (product) => {
        this.product.set(product);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Product could not be found.');
        this.isLoading.set(false);
      },
    });
  }
}
