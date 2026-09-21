import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import {
  AddBasketItemRequestDto,
  BasketResponseDto,
  UpdateBasketItemRequestDto,
} from './basket.dto';

@Injectable({
  providedIn: 'root',
})
export class BasketApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyBasket(): Observable<BasketResponseDto> {
    return this.http.get<BasketResponseDto>(`${this.apiBaseUrl}/me/basket`);
  }

  addItem(request: AddBasketItemRequestDto): Observable<BasketResponseDto> {
    return this.http.post<BasketResponseDto>(`${this.apiBaseUrl}/me/basket/items`, request);
  }

  updateQuantity(
    itemId: number,
    request: UpdateBasketItemRequestDto,
  ): Observable<BasketResponseDto> {
    return this.http.patch<BasketResponseDto>(
      `${this.apiBaseUrl}/me/basket/items/${itemId}`,
      request,
    );
  }

  removeItem(itemId: number): Observable<BasketResponseDto> {
    return this.http.delete<BasketResponseDto>(`${this.apiBaseUrl}/me/basket/items/${itemId}`);
  }

  clearBasket(): Observable<BasketResponseDto> {
    return this.http.delete<BasketResponseDto>(`${this.apiBaseUrl}/me/basket/items`);
  }
}
