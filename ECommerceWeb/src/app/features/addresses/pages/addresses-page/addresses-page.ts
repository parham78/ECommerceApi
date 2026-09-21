import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  AddressResponseDto,
  CreateAddressRequestDto,
  UpdateAddressRequestDto,
} from '../../data-access/address.dto';
import { AddressesApi } from '../../data-access/addresses-api';

@Component({
  selector: 'app-addresses-page',
  imports: [ReactiveFormsModule],
  templateUrl: './addresses-page.html',
  styleUrl: './addresses-page.scss',
})
export class AddressesPage {
  private readonly addressesApi = inject(AddressesApi);
  private readonly formBuilder = inject(FormBuilder);

  readonly addresses = signal<AddressResponseDto[]>([]);

  readonly isLoading = signal(true);
  readonly loadError = signal<string | null>(null);

  readonly isFormOpen = signal(false);
  readonly editingAddress = signal<AddressResponseDto | null>(null);
  readonly isSaving = signal(false);
  readonly saveError = signal<string | null>(null);

  readonly deleteConfirmId = signal<number | null>(null);
  readonly deletingAddressId = signal<number | null>(null);
  readonly deleteError = signal<string | null>(null);

  readonly addressForm = this.formBuilder.group({
    label: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(50),
    ]),

    recipientName: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(100),
    ]),

    addressLine1: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(200),
    ]),

    addressLine2: this.formBuilder.control<string | null>(null, [Validators.maxLength(200)]),

    city: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(100),
    ]),

    province: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(100),
    ]),

    postalCode: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(20),
    ]),

    country: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(100),
    ]),

    phoneNumber: this.formBuilder.control<string | null>(null, [Validators.maxLength(30)]),

    isDefault: this.formBuilder.nonNullable.control(false),
  });

  constructor() {
    this.loadAddresses();
  }

  openCreateForm(): void {
    this.editingAddress.set(null);
    this.saveError.set(null);

    this.addressForm.reset({
      label: '',
      recipientName: '',
      addressLine1: '',
      addressLine2: null,
      city: '',
      province: '',
      postalCode: '',
      country: '',
      phoneNumber: null,
      isDefault: false,
    });

    this.isFormOpen.set(true);
  }

  openEditForm(address: AddressResponseDto): void {
    this.editingAddress.set(address);
    this.saveError.set(null);

    this.addressForm.reset({
      label: address.label,
      recipientName: address.recipientName,
      addressLine1: address.addressLine1,
      addressLine2: address.addressLine2,
      city: address.city,
      province: address.province,
      postalCode: address.postalCode,
      country: address.country,
      phoneNumber: address.phoneNumber,
      isDefault: address.isDefault,
    });

    this.isFormOpen.set(true);
  }

  closeForm(): void {
    if (this.isSaving()) {
      return;
    }

    this.isFormOpen.set(false);
    this.editingAddress.set(null);
    this.saveError.set(null);
  }

  saveAddress(): void {
    if (this.addressForm.invalid || this.isSaving()) {
      this.addressForm.markAllAsTouched();
      return;
    }

    const values = this.addressForm.getRawValue();
    const editing = this.editingAddress();

    const request = {
      label: values.label.trim(),
      recipientName: values.recipientName.trim(),
      addressLine1: values.addressLine1.trim(),
      addressLine2: this.optionalValue(values.addressLine2),
      city: values.city.trim(),
      province: values.province.trim(),
      postalCode: values.postalCode.trim(),
      country: values.country.trim(),
      phoneNumber: this.optionalValue(values.phoneNumber),

      // The backend keeps the current default until another
      // address is selected as default.
      isDefault: editing?.isDefault ? true : values.isDefault,
    };

    this.isSaving.set(true);
    this.saveError.set(null);

    const request$ = editing
      ? this.addressesApi.updateAddress(editing.id, request as UpdateAddressRequestDto)
      : this.addressesApi.createAddress(request as CreateAddressRequestDto);

    request$.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isFormOpen.set(false);
        this.editingAddress.set(null);

        this.loadAddresses();
      },

      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);

        this.saveError.set(
          this.getErrorMessage(error, 'We could not save this address. Please try again.'),
        );
      },
    });
  }

  requestDelete(addressId: number): void {
    this.deleteError.set(null);
    this.deleteConfirmId.set(addressId);
  }

  cancelDelete(): void {
    this.deleteConfirmId.set(null);
  }

  deleteAddress(addressId: number): void {
    if (this.deletingAddressId() !== null) {
      return;
    }

    this.deletingAddressId.set(addressId);
    this.deleteError.set(null);

    this.addressesApi.deleteAddress(addressId).subscribe({
      next: () => {
        this.deletingAddressId.set(null);
        this.deleteConfirmId.set(null);

        this.loadAddresses();
      },

      error: (error: HttpErrorResponse) => {
        this.deletingAddressId.set(null);

        this.deleteError.set(
          this.getErrorMessage(error, 'We could not delete this address. Please try again.'),
        );
      },
    });
  }

  private loadAddresses(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.addressesApi.getMyAddresses().subscribe({
      next: (addresses) => {
        this.addresses.set(addresses);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.loadError.set('You are not authorized to view these addresses.');

          return;
        }

        this.loadError.set('We could not load your addresses. Please try again.');
      },
    });
  }

  private optionalValue(value: string | null): string | null {
    const trimmed = value?.trim();

    return trimmed ? trimmed : null;
  }

  private getErrorMessage(error: HttpErrorResponse, fallback: string): string {
    const detail = error.error?.detail;

    if (typeof detail === 'string' && detail.trim().length > 0) {
      return detail;
    }

    return fallback;
  }
}
