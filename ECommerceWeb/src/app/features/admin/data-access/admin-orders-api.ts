import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import { PagedResultDto } from '../../../shared/models/paged-result.dto';
import { OrderResponseDto, OrderStatus } from '../../orders/data-access/order.dto';

@Injectable({
  providedIn: 'root',
})
export class AdminOrdersApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  list(page = 1, pageSize = 10): Observable<PagedResultDto<OrderResponseDto>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);

    return this.http.get<PagedResultDto<OrderResponseDto>>(`${this.apiBaseUrl}/orders`, { params });
  }

  getById(id: number): Observable<OrderResponseDto> {
    return this.http.get<OrderResponseDto>(`${this.apiBaseUrl}/orders/${id}`);
  }

  changeStatus(id: number, status: OrderStatus): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.apiBaseUrl}/orders/${id}/status`, { status });
  }

  cancel(id: number): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.apiBaseUrl}/orders/${id}/cancel`, {});
  }
}
