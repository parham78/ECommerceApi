export interface AdminDashboardSummaryDto {
  totalRevenue: number;
  totalOrders: number;
  averageOrderValue: number;
  activeProducts: number;
  inactiveProducts: number;
  lowStockProducts: number;
}

export interface OrdersByStatusDto {
  status: string;
  count: number;
}

export interface RevenuePointDto {
  year: number;
  month: number;
  revenue: number;
  orders: number;
}

export interface LowStockProductDto {
  id: number;
  name: string;
  sku: string;
  stock: number;
}

export interface RecentAdminOrderDto {
  id: number;
  customerId: number;
  customerName: string;
  totalPrice: number;
  status: string;
  createdAt: string;
}
