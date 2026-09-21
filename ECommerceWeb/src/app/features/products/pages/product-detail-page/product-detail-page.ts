import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { AuthSession } from '../../../auth/data-access/auth-session';
import { BasketApi } from '../../../basket/data-access/basket-api';
import { BasketItemResponseDto, BasketResponseDto } from '../../../basket/data-access/basket.dto';
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
  private readonly basketApi = inject(BasketApi);

  readonly authSession = inject(AuthSession);

  private readonly productId = Number(this.route.snapshot.paramMap.get('id'));

  readonly product = signal<ProductResponseDto | null>(null);
  readonly basket = signal<BasketResponseDto | null>(null);

  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly canRetry = signal(false);

  readonly isBasketLoading = signal(false);
  readonly isBasketBusy = signal(false);

  readonly basketMessage = signal<string | null>(null);
  readonly basketError = signal<string | null>(null);

  readonly basketItem = computed<BasketItemResponseDto | null>(() => {
    const product = this.product();
    const basket = this.basket();

    if (!product || !basket) {
      return null;
    }

    return basket.items.find((item) => item.productId === product.id) ?? null;
  });

  constructor() {
    if (!Number.isInteger(this.productId) || this.productId <= 0) {
      this.isLoading.set(false);
      this.errorMessage.set('Invalid product.');
      return;
    }

    this.loadProduct();

    if (this.authSession.isAuthenticated() && this.authSession.roles().includes('Customer')) {
      this.loadBasket();
    }
  }

  retry(): void {
    this.loadProduct();
  }

  addToBasket(): void {
    const product = this.product();

    if (!product || product.stock <= 0 || this.isBasketBusy()) {
      return;
    }

    this.startBasketAction();

    this.basketApi
      .addItem({
        productId: product.id,
        quantity: 1,
      })
      .subscribe({
        next: (basket) => {
          this.basket.set(basket);
          this.isBasketBusy.set(false);

          this.basketMessage.set('Added to your basket.');
        },

        error: () => {
          this.isBasketBusy.set(false);

          this.basketError.set('We could not add this product to your basket.');
        },
      });
  }

  increaseQuantity(): void {
    const item = this.basketItem();

    if (!item || item.quantity >= item.availableStock || this.isBasketBusy()) {
      return;
    }

    this.updateQuantity(item, item.quantity + 1);
  }

  decreaseQuantity(): void {
    const item = this.basketItem();

    if (!item || item.quantity <= 1 || this.isBasketBusy()) {
      return;
    }

    this.updateQuantity(item, item.quantity - 1);
  }

  removeFromBasket(): void {
    const item = this.basketItem();

    if (!item || this.isBasketBusy()) {
      return;
    }

    this.startBasketAction();

    this.basketApi.removeItem(item.id).subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.isBasketBusy.set(false);

        this.basketMessage.set('Removed from your basket.');
      },

      error: () => {
        this.isBasketBusy.set(false);

        this.basketError.set('We could not remove this product from your basket.');
      },
    });
  }

  private updateQuantity(item: BasketItemResponseDto, quantity: number): void {
    this.startBasketAction();

    this.basketApi.updateQuantity(item.id, { quantity }).subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.isBasketBusy.set(false);

        this.basketMessage.set('Basket updated.');
      },

      error: () => {
        this.isBasketBusy.set(false);

        this.basketError.set('We could not update your basket.');
      },
    });
  }

  private loadBasket(): void {
    this.isBasketLoading.set(true);

    this.basketApi.getMyBasket().subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.isBasketLoading.set(false);
      },

      error: () => {
        this.isBasketLoading.set(false);

        this.basketError.set('We could not check your basket.');
      },
    });
  }

  private startBasketAction(): void {
    this.isBasketBusy.set(true);
    this.basketMessage.set(null);
    this.basketError.set(null);
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
