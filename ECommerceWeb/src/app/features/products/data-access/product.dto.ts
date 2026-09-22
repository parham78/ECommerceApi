export interface ProductResponseDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;

  categoryId: number;
  categoryName: string;
  categorySlug: string;
}
