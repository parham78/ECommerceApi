import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import { PagedResultDto } from '../../../shared/models/paged-result.dto';
import {
  AdminProductResponseDto,
  CreateProductRequestDto,
  UpdateProductRequestDto,
  UpdateStockRequestDto,
} from './admin-product.dto';

@Injectable({
  providedIn: 'root',
})
export class AdminProductsApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  list(
    page = 1,
    pageSize = 10,
    isActive?: boolean,
  ): Observable<PagedResultDto<AdminProductResponseDto>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);

    if (isActive !== undefined) {
      params = params.set('isActive', isActive);
    }

    return this.http.get<PagedResultDto<AdminProductResponseDto>>(
      `${this.apiBaseUrl}/products/admin`,
      { params },
    );
  }

  getById(id: number): Observable<AdminProductResponseDto> {
    return this.http.get<AdminProductResponseDto>(`${this.apiBaseUrl}/products/admin/${id}`);
  }

  create(request: CreateProductRequestDto): Observable<AdminProductResponseDto> {
    return this.http.post<AdminProductResponseDto>(`${this.apiBaseUrl}/products`, request);
  }

  update(id: number, request: UpdateProductRequestDto): Observable<AdminProductResponseDto> {
    return this.http.put<AdminProductResponseDto>(`${this.apiBaseUrl}/products/${id}`, request);
  }

  updateStock(id: number, request: UpdateStockRequestDto): Observable<AdminProductResponseDto> {
    return this.http.put<AdminProductResponseDto>(
      `${this.apiBaseUrl}/products/${id}/stock`,
      request,
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/products/${id}`);
  }
}
