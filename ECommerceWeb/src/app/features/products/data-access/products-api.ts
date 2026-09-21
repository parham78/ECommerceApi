import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import { PagedResultDto } from '../../../shared/models/paged-result.dto';
import { ProductResponseDto } from './product.dto';
import { ProductQuery } from './product-query';

@Injectable({
  providedIn: 'root',
})
export class ProductsApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  list(query: ProductQuery): Observable<PagedResultDto<ProductResponseDto>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);

    if (query.search) {
      params = params.set('search', query.search);
    }

    if (query.minPrice !== undefined) {
      params = params.set('minPrice', query.minPrice);
    }

    if (query.maxPrice !== undefined) {
      params = params.set('maxPrice', query.maxPrice);
    }

    if (query.inStock !== undefined) {
      params = params.set('inStock', query.inStock);
    }

    if (query.sortBy) {
      params = params.set('sortBy', query.sortBy);
    }

    if (query.sortDirection) {
      params = params.set('sortDirection', query.sortDirection);
    }

    return this.http.get<PagedResultDto<ProductResponseDto>>(`${this.apiBaseUrl}/products`, {
      params,
    });
  }
}
