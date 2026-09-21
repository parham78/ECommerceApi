import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import { OrderResponseDto } from '../../orders/data-access/order.dto';
import { CheckoutRequestDto } from './checkout.dto';

@Injectable({
  providedIn: 'root',
})
export class CheckoutApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  checkout(request: CheckoutRequestDto): Observable<OrderResponseDto> {
    return this.http.post<OrderResponseDto>(`${this.apiBaseUrl}/me/checkout`, request);
  }
}
