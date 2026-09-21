import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import { PagedResultDto } from '../../../shared/models/paged-result.dto';
import { OrderResponseDto } from './order.dto';

@Injectable({
  providedIn: 'root',
})
export class OrdersApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyOrders(page: number, pageSize: number): Observable<PagedResultDto<OrderResponseDto>> {
    return this.http.get<PagedResultDto<OrderResponseDto>>(`${this.apiBaseUrl}/me/orders`, {
      params: {
        page,
        pageSize,
      },
    });
  }

  getMyOrderById(orderId: number): Observable<OrderResponseDto> {
    return this.http.get<OrderResponseDto>(`${this.apiBaseUrl}/me/orders/${orderId}`);
  }

  cancelMyOrder(orderId: number): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.apiBaseUrl}/me/orders/${orderId}/cancel`, {});
  }
}
