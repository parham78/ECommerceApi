import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { catchError, finalize, forkJoin, of } from 'rxjs';

import {
  AdminDashboardSummaryDto,
  LowStockProductDto,
  OrdersByStatusDto,
  RecentAdminOrderDto,
  RevenuePointDto,
} from '../../data-access/admin-dashboard.dto';

import { AdminDashboardApi } from '../../data-access/admin-dashboard-api';

@Component({
  selector: 'app-admin-overview-page',
  templateUrl: './admin-overview-page.html',
  styleUrl: './admin-overview-page.scss',
})
export class AdminOverviewPage implements OnInit {
  private readonly dashboardApi = inject(AdminDashboardApi);

  private readonly destroyRef = inject(DestroyRef);

  readonly summary = signal<AdminDashboardSummaryDto | null>(null);

  readonly ordersByStatus = signal<OrdersByStatusDto[]>([]);

  readonly revenue = signal<RevenuePointDto[]>([]);

  readonly lowStockProducts = signal<LowStockProductDto[]>([]);

  readonly recentOrders = signal<RecentAdminOrderDto[]>([]);

  readonly isLoading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly revenueMax = computed(() => {
    const points = this.revenue();

    if (points.length === 0) {
      return 0;
    }

    return Math.max(...points.map((point) => point.revenue));
  });

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    forkJoin({
      summary: this.dashboardApi.getSummary(),

      ordersByStatus: this.dashboardApi.getOrdersByStatus(),

      revenue: this.dashboardApi.getRevenue(6),

      lowStockProducts: this.dashboardApi.getLowStockProducts(5, 5),

      recentOrders: this.dashboardApi.getRecentOrders(5),
    })
      .pipe(
        catchError(() => {
          this.errorMessage.set('We could not load the dashboard data.');

          return of(null);
        }),

        finalize(() => {
          this.isLoading.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((result) => {
        if (!result) {
          return;
        }

        this.summary.set(result.summary);

        this.ordersByStatus.set(result.ordersByStatus);

        this.revenue.set(result.revenue);

        this.lowStockProducts.set(result.lowStockProducts);

        this.recentOrders.set(result.recentOrders);
      });
  }

  revenueBarHeight(revenue: number): number {
    const max = this.revenueMax();

    if (max <= 0) {
      return 0;
    }

    return Math.max((revenue / max) * 100, revenue > 0 ? 4 : 0);
  }

  monthLabel(year: number, month: number): string {
    return new Intl.DateTimeFormat('en-CA', {
      month: 'short',
    }).format(new Date(year, month - 1, 1));
  }
}
