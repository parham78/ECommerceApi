import { Component, inject, OnInit, signal } from '@angular/core';

import { ProductResponseDto } from '../../data-access/product.dto';
import { ProductsApi } from '../../data-access/products-api';

@Component({
  selector: 'app-catalog-page',
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.scss',
})
export class CatalogPage implements OnInit {
  private readonly productsApi = inject(ProductsApi);

  readonly products = signal<ProductResponseDto[]>([]);

  ngOnInit(): void {
    this.productsApi
      .list({
        page: 1,
        pageSize: 12,
      })
      .subscribe((result) => {
        this.products.set(result.items);
      });
  }
}
