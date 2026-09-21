export interface AddressResponseDto {
  id: number;

  label: string;
  recipientName: string;

  addressLine1: string;
  addressLine2: string | null;

  city: string;
  province: string;
  postalCode: string;
  country: string;

  phoneNumber: string | null;

  isDefault: boolean;
}

export interface CreateAddressRequestDto {
  label: string;
  recipientName: string;

  addressLine1: string;
  addressLine2: string | null;

  city: string;
  province: string;
  postalCode: string;
  country: string;

  phoneNumber: string | null;

  isDefault: boolean;
}

export interface UpdateAddressRequestDto {
  label: string;
  recipientName: string;

  addressLine1: string;
  addressLine2: string | null;

  city: string;
  province: string;
  postalCode: string;
  country: string;

  phoneNumber: string | null;

  isDefault: boolean;
}
