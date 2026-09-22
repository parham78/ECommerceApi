export interface AdminProductResponseDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
  isActive: boolean;
  categoryId: number;
  categoryName: string;
  categorySlug: string;
  rowVersion: string;
}

export interface CreateProductRequestDto {
  name: string;
  price: number;
  categoryId: number;
  stock: number;
  sku: string;
  isActive: boolean;
}

export interface UpdateProductRequestDto {
  name: string;
  sku: string;
  price: number;
  isActive: boolean;
  categoryId: number;
  rowVersion: string;
}

export interface UpdateStockRequestDto {
  newStock: number;
  rowVersion: string;
}
