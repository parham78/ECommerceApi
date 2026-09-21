import { HttpErrorResponse } from '@angular/common/http';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { OrderResponseDto } from '../../data-access/order.dto';
import { OrdersApi } from '../../data-access/orders-api';

@Component({
  selector: 'app-orders-page',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './orders-page.html',
  styleUrl: './orders-page.scss',
})
export class OrdersPage {
  private readonly ordersApi = inject(OrdersApi);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly orders = signal<OrderResponseDto[]>([]);

  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly currentPage = signal(1);
  readonly pageSize = 10;

  readonly totalCount = signal(0);
  readonly totalPages = signal(0);

  constructor() {
    this.route.queryParamMap.subscribe((params) => {
      const rawPage = Number(params.get('page') ?? '1');

      const page = Number.isInteger(rawPage) && rawPage > 0 ? rawPage : 1;

      this.currentPage.set(page);
      this.loadOrders();
    });
  }

  previousPage(): void {
    if (this.currentPage() <= 1) {
      return;
    }

    this.goToPage(this.currentPage() - 1);
  }

  nextPage(): void {
    if (this.currentPage() >= this.totalPages()) {
      return;
    }

    this.goToPage(this.currentPage() + 1);
  }

  retry(): void {
    this.loadOrders();
  }

  itemCount(order: OrderResponseDto): number {
    return order.items.reduce((total, item) => total + item.quantity, 0);
  }

  private goToPage(page: number): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        page,
      },
      queryParamsHandling: 'merge',
    });
  }

  private loadOrders(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.ordersApi.getMyOrders(this.currentPage(), this.pageSize).subscribe({
      next: (result) => {
        this.orders.set(result.items);

        this.currentPage.set(result.currentPage);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);

        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set('You are not authorized to view these orders.');

          return;
        }

        const detail = error.error?.detail;

        if (typeof detail === 'string' && detail.trim().length > 0) {
          this.errorMessage.set(detail);

          return;
        }

        this.errorMessage.set('We could not load your orders. Please try again.');
      },
    });
  }
}
