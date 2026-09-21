import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';

import { BasketApi } from '../../data-access/basket-api';
import { BasketResponseDto } from '../../data-access/basket.dto';

@Component({
  selector: 'app-basket-page',
  imports: [CurrencyPipe],
  templateUrl: './basket-page.html',
  styleUrl: './basket-page.scss',
})
export class BasketPage {
  private readonly basketApi = inject(BasketApi);

  readonly basket = signal<BasketResponseDto | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.loadBasket();
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
