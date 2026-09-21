import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api-base-url';
import {
  AddressResponseDto,
  CreateAddressRequestDto,
  UpdateAddressRequestDto,
} from './address.dto';

@Injectable({
  providedIn: 'root',
})
export class AddressesApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyAddresses(): Observable<AddressResponseDto[]> {
    return this.http.get<AddressResponseDto[]>(`${this.apiBaseUrl}/me/addresses`);
  }

  createAddress(request: CreateAddressRequestDto): Observable<AddressResponseDto> {
    return this.http.post<AddressResponseDto>(`${this.apiBaseUrl}/me/addresses`, request);
  }

  updateAddress(
    addressId: number,
    request: UpdateAddressRequestDto,
  ): Observable<AddressResponseDto> {
    return this.http.put<AddressResponseDto>(
      `${this.apiBaseUrl}/me/addresses/${addressId}`,
      request,
    );
  }

  deleteAddress(addressId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/me/addresses/${addressId}`);
  }
}
