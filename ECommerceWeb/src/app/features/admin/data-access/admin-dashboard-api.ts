import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import {
  AdminDashboardSummaryDto,
  LowStockProductDto,
  OrdersByStatusDto,
  RecentAdminOrderDto,
  RevenuePointDto,
} from './admin-dashboard.dto';

@Injectable({
  providedIn: 'root',
})
export class AdminDashboardApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getSummary(): Observable<AdminDashboardSummaryDto> {
    return this.http.get<AdminDashboardSummaryDto>(`${this.apiBaseUrl}/admin/dashboard/summary`);
  }

  getOrdersByStatus(): Observable<OrdersByStatusDto[]> {
    return this.http.get<OrdersByStatusDto[]>(
      `${this.apiBaseUrl}/admin/dashboard/orders-by-status`,
    );
  }

  getRevenue(months = 6): Observable<RevenuePointDto[]> {
    const params = new HttpParams().set('months', months);

    return this.http.get<RevenuePointDto[]>(`${this.apiBaseUrl}/admin/dashboard/revenue`, {
      params,
    });
  }

  getLowStockProducts(threshold = 5, take = 5): Observable<LowStockProductDto[]> {
    const params = new HttpParams().set('threshold', threshold).set('take', take);

    return this.http.get<LowStockProductDto[]>(`${this.apiBaseUrl}/admin/dashboard/low-stock`, {
      params,
    });
  }

  getRecentOrders(take = 5): Observable<RecentAdminOrderDto[]> {
    const params = new HttpParams().set('take', take);

    return this.http.get<RecentAdminOrderDto[]>(
      `${this.apiBaseUrl}/admin/dashboard/recent-orders`,
      { params },
    );
  }
}
