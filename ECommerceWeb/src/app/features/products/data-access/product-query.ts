export interface ProductQuery {
  page: number;
  pageSize: number;
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  sortBy?: string;
  sortDirection?: string;
}
