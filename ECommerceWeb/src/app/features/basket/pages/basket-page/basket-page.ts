import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ProductImage } from '../../../products/ui/product-image/product-image';
import { BasketApi } from '../../data-access/basket-api';
import { BasketItemResponseDto, BasketResponseDto } from '../../data-access/basket.dto';

@Component({
  selector: 'app-basket-page',
  imports: [CurrencyPipe, RouterLink, ProductImage],
  templateUrl: './basket-page.html',
  styleUrl: './basket-page.scss',
})
export class BasketPage {
  private readonly basketApi = inject(BasketApi);

  readonly basket = signal<BasketResponseDto | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly pendingItemId = signal<number | null>(null);
  readonly isClearing = signal(false);
  readonly actionError = signal<string | null>(null);

  constructor() {
    this.loadBasket();
  }

  increaseQuantity(item: BasketItemResponseDto): void {
    if (item.quantity >= item.availableStock) {
      return;
    }

    this.updateQuantity(item, item.quantity + 1);
  }

  decreaseQuantity(item: BasketItemResponseDto): void {
    if (item.quantity <= 1) {
      return;
    }

    this.updateQuantity(item, item.quantity - 1);
  }

  removeItem(itemId: number): void {
    this.pendingItemId.set(itemId);
    this.actionError.set(null);

    this.basketApi.removeItem(itemId).subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.pendingItemId.set(null);
      },

      error: () => {
        this.pendingItemId.set(null);
        this.actionError.set('We could not remove this item. Please try again.');
      },
    });
  }

  clearBasket(): void {
    this.isClearing.set(true);
    this.actionError.set(null);

    this.basketApi.clearBasket().subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.isClearing.set(false);
      },

      error: () => {
        this.isClearing.set(false);
        this.actionError.set('We could not clear your basket. Please try again.');
      },
    });
  }

  private updateQuantity(item: BasketItemResponseDto, quantity: number): void {
    this.pendingItemId.set(item.id);
    this.actionError.set(null);

    this.basketApi.updateQuantity(item.id, { quantity }).subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.pendingItemId.set(null);
      },

      error: () => {
        this.pendingItemId.set(null);
        this.actionError.set('We could not update the quantity. Please try again.');
      },
    });
  }

  private loadBasket(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.basketApi.getMyBasket().subscribe({
      next: (basket) => {
        this.basket.set(basket);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set('You are not authorized to view this basket.');
          return;
        }

        this.errorMessage.set('We could not load your basket. Please try again.');
      },
    });
  }
}
