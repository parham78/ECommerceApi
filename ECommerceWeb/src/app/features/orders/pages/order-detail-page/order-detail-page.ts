import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ProductImage } from '../../../products/ui/product-image/product-image';
import { OrderResponseDto } from '../../data-access/order.dto';
import { OrdersApi } from '../../data-access/orders-api';

@Component({
  selector: 'app-order-detail-page',
  imports: [CurrencyPipe, DatePipe, RouterLink, ProductImage],
  templateUrl: './order-detail-page.html',
  styleUrl: './order-detail-page.scss',
})
export class OrderDetailPage {
  private readonly route = inject(ActivatedRoute);
  private readonly ordersApi = inject(OrdersApi);

  readonly order = signal<OrderResponseDto | null>(null);

  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly showCancelConfirmation = signal(false);
  readonly isCancelling = signal(false);
  readonly cancelError = signal<string | null>(null);

  constructor() {
    const rawId = this.route.snapshot.paramMap.get('id');
    const orderId = Number(rawId);

    if (!Number.isInteger(orderId) || orderId <= 0) {
      this.isLoading.set(false);
      this.errorMessage.set('This order number is invalid.');

      return;
    }

    this.loadOrder(orderId);
  }

  itemCount(order: OrderResponseDto): number {
    return order.items.reduce((total, item) => total + item.quantity, 0);
  }

  requestCancellation(): void {
    this.cancelError.set(null);
    this.showCancelConfirmation.set(true);
  }

  closeCancellation(): void {
    if (this.isCancelling()) {
      return;
    }

    this.showCancelConfirmation.set(false);
    this.cancelError.set(null);
  }

  cancelOrder(): void {
    const currentOrder = this.order();

    if (!currentOrder || currentOrder.status !== 'Pending' || this.isCancelling()) {
      return;
    }

    this.isCancelling.set(true);
    this.cancelError.set(null);

    this.ordersApi.cancelMyOrder(currentOrder.id).subscribe({
      next: (updatedOrder) => {
        this.order.set(updatedOrder);

        this.isCancelling.set(false);
        this.showCancelConfirmation.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isCancelling.set(false);

        const detail = error.error?.detail;

        if (typeof detail === 'string' && detail.trim().length > 0) {
          this.cancelError.set(detail);

          return;
        }

        this.cancelError.set('We could not cancel this order. Please try again.');
      },
    });
  }

  private loadOrder(orderId: number): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.ordersApi.getMyOrderById(orderId).subscribe({
      next: (order) => {
        this.order.set(order);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);

        if (error.status === 404) {
          this.errorMessage.set('This order could not be found.');

          return;
        }

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set('You are not authorized to view this order.');

          return;
        }

        const detail = error.error?.detail;

        if (typeof detail === 'string' && detail.trim().length > 0) {
          this.errorMessage.set(detail);

          return;
        }

        this.errorMessage.set('We could not load this order. Please try again.');
      },
    });
  }
}
