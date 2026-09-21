export type OrderStatus = 'Pending' | 'Processing' | 'Shipped' | 'Completed' | 'Cancelled';

export interface OrderItemResponseDto {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface OrderResponseDto {
  id: number;

  customerId: number;
  customerName: string;

  totalPrice: number;
  status: OrderStatus;
  createdAt: string;

  shippingRecipientName: string | null;
  shippingAddressLine1: string | null;
  shippingAddressLine2: string | null;
  shippingCity: string | null;
  shippingProvince: string | null;
  shippingPostalCode: string | null;
  shippingCountry: string | null;
  shippingPhoneNumber: string | null;

  items: OrderItemResponseDto[];
}
