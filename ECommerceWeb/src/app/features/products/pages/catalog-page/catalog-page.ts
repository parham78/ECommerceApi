import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import {
  catchError,
  combineLatest,
  distinctUntilChanged,
  EMPTY,
  finalize,
  map,
  startWith,
  Subject,
  switchMap,
} from 'rxjs';

import { Pagination } from '../../../../shared/ui/pagination/pagination';
import { ProductResponseDto } from '../../data-access/product.dto';
import { ProductQuery } from '../../data-access/product-query';
import { ProductsApi } from '../../data-access/products-api';
import { ProductCard } from '../../ui/product-card/product-card';

@Component({
  selector: 'app-catalog-page',
  imports: [ProductCard, Pagination],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.scss',
})
export class CatalogPage implements OnInit {
  private readonly productsApi = inject(ProductsApi);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  private readonly retryTrigger = new Subject<void>();

  readonly products = signal<ProductResponseDto[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly currentPage = signal(1);
  readonly pageSize = signal(12);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);

  ngOnInit(): void {
    const query$ = this.route.queryParamMap.pipe(
      map((params): ProductQuery => {
        const pageParam = Number(params.get('page'));
        const pageSizeParam = Number(params.get('pageSize'));

        const page = pageParam > 0 ? pageParam : 1;

        const pageSize = [12, 24, 48].includes(pageSizeParam) ? pageSizeParam : 12;

        return {
          page,
          pageSize,
        };
      }),

      distinctUntilChanged(
        (previous, current) =>
          previous.page === current.page && previous.pageSize === current.pageSize,
      ),
    );

    combineLatest([query$, this.retryTrigger.pipe(startWith(undefined))])
      .pipe(
        switchMap(([query]) => {
          this.currentPage.set(query.page);
          this.pageSize.set(query.pageSize);

          this.isLoading.set(true);
          this.errorMessage.set(null);

          return this.productsApi.list(query).pipe(
            catchError(() => {
              this.products.set([]);
              this.totalCount.set(0);
              this.totalPages.set(0);

              this.errorMessage.set('We could not load products. Please try again.');

              return EMPTY;
            }),
            finalize(() => {
              this.isLoading.set(false);
            }),
          );
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((result) => {
        this.products.set(result.items);
        this.currentPage.set(result.currentPage);
        this.pageSize.set(result.pageSize);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
      });
  }

  changePage(page: number): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        page,
        pageSize: this.pageSize(),
      },
      queryParamsHandling: 'merge',
    });
  }
  changePageSize(value: string): void {
    const pageSize = Number(value);

    if (![12, 24, 48].includes(pageSize)) {
      return;
    }

    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        page: 1,
        pageSize,
      },
      queryParamsHandling: 'merge',
    });
  }

  retry(): void {
    this.retryTrigger.next();
  }
}
