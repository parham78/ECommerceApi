import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';

import { OrderResponseDto, OrderStatus } from '../../../orders/data-access/order.dto';

import { AdminOrdersApi } from '../../data-access/admin-orders-api';

@Component({
  selector: 'app-admin-orders-page',
  imports: [DatePipe],
  templateUrl: './admin-orders-page.html',
  styleUrl: './admin-orders-page.scss',
})
export class AdminOrdersPage implements OnInit {
  private readonly ordersApi = inject(AdminOrdersApi);

  private readonly destroyRef = inject(DestroyRef);

  readonly orders = signal<OrderResponseDto[]>([]);

  readonly currentPage = signal(1);

  readonly pageSize = signal(10);

  readonly totalCount = signal(0);

  readonly totalPages = signal(0);

  readonly isLoading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly selectedOrder = signal<OrderResponseDto | null>(null);

  readonly isLoadingOrder = signal(false);

  readonly isUpdatingOrder = signal(false);

  readonly orderErrorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(page = this.currentPage()): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.ordersApi
      .list(page, this.pageSize())
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          this.orders.set(result.items);
          this.currentPage.set(result.currentPage);
          this.pageSize.set(result.pageSize);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(result.totalPages);
        },

        error: () => {
          this.errorMessage.set('We could not load the orders.');
        },
      });
  }

  openOrder(id: number): void {
    this.isLoadingOrder.set(true);
    this.orderErrorMessage.set(null);

    this.ordersApi
      .getById(id)
      .pipe(
        finalize(() => {
          this.isLoadingOrder.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (order) => {
          this.selectedOrder.set(order);
        },

        error: () => {
          this.orderErrorMessage.set('We could not load this order.');
        },
      });
  }

  closeOrder(): void {
    if (this.isUpdatingOrder()) {
      return;
    }

    this.selectedOrder.set(null);
    this.orderErrorMessage.set(null);
  }

  nextStatus(status: OrderStatus): OrderStatus | null {
    switch (status) {
      case 'Pending':
        return 'Processing';

      case 'Processing':
        return 'Shipped';

      case 'Shipped':
        return 'Completed';

      default:
        return null;
    }
  }

  updateStatus(status: OrderStatus): void {
    const order = this.selectedOrder();

    if (!order) {
      return;
    }

    this.isUpdatingOrder.set(true);
    this.orderErrorMessage.set(null);

    this.ordersApi
      .changeStatus(order.id, status)
      .pipe(
        finalize(() => {
          this.isUpdatingOrder.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (updatedOrder) => {
          this.updateOrderLocally(updatedOrder);
        },

        error: (error: HttpErrorResponse) => {
          this.handleOrderActionError(error);
        },
      });
  }

  cancelOrder(): void {
    const order = this.selectedOrder();

    if (!order) {
      return;
    }

    this.isUpdatingOrder.set(true);
    this.orderErrorMessage.set(null);

    this.ordersApi
      .cancel(order.id)
      .pipe(
        finalize(() => {
          this.isUpdatingOrder.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (updatedOrder) => {
          this.updateOrderLocally(updatedOrder);
        },

        error: (error: HttpErrorResponse) => {
          this.handleOrderActionError(error);
        },
      });
  }

  previousPage(): void {
    if (this.currentPage() <= 1) {
      return;
    }

    this.loadOrders(this.currentPage() - 1);
  }

  nextPage(): void {
    if (this.currentPage() >= this.totalPages()) {
      return;
    }

    this.loadOrders(this.currentPage() + 1);
  }

  private updateOrderLocally(updatedOrder: OrderResponseDto): void {
    this.selectedOrder.set(updatedOrder);

    this.orders.update((orders) =>
      orders.map((order) => (order.id === updatedOrder.id ? updatedOrder : order)),
    );
  }

  private handleOrderActionError(error: HttpErrorResponse): void {
    if (error.status === 400) {
      this.orderErrorMessage.set('That status change is not allowed for this order.');

      return;
    }

    this.orderErrorMessage.set('We could not update this order. Please try again.');
  }
}
