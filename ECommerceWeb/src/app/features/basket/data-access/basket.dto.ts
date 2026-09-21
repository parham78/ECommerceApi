export interface BasketItemResponseDto {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
  availableStock: number;
  isAvailable: boolean;
}

export interface BasketResponseDto {
  id: number | null;
  items: BasketItemResponseDto[];
  totalPrice: number;
}

export interface AddBasketItemRequestDto {
  productId: number;
  quantity: number;
}

export interface UpdateBasketItemRequestDto {
  quantity: number;
}
