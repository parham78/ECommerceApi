import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ActivatedRoute, Router } from '@angular/router';

import {
  catchError,
  combineLatest,
  distinctUntilChanged,
  EMPTY,
  finalize,
  map,
  of,
  startWith,
  Subject,
  switchMap,
} from 'rxjs';

import { Pagination } from '../../../../shared/ui/pagination/pagination';

import { CategoryResponseDto } from '../../data-access/category.dto';

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

  readonly categories = signal<CategoryResponseDto[]>([]);

  readonly isLoading = signal(false);

  readonly categoriesLoading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly currentPage = signal(1);

  readonly pageSize = signal(12);

  readonly totalCount = signal(0);

  readonly totalPages = signal(0);

  readonly searchValue = signal('');

  readonly categoryValue = signal('');

  readonly minPriceValue = signal('');

  readonly maxPriceValue = signal('');

  readonly inStockValue = signal(false);

  readonly sortValue = signal('default');

  readonly filtersOpen = signal(false);

  readonly allProductsCount = computed(() =>
    this.categories().reduce(
      (total, category) => total + category.productCount,

      0,
    ),
  );

  readonly hasActiveFilters = computed(
    () =>
      this.searchValue().trim().length > 0 ||
      this.categoryValue().length > 0 ||
      this.minPriceValue().length > 0 ||
      this.maxPriceValue().length > 0 ||
      this.inStockValue() ||
      this.sortValue() !== 'default',
  );

  ngOnInit(): void {
    this.loadCategories();

    const query$ = this.route.queryParamMap.pipe(
      map((params): ProductQuery => {
        const pageParam = Number(params.get('page'));

        const pageSizeParam = Number(params.get('pageSize'));

        const page = pageParam > 0 ? pageParam : 1;

        const pageSize = [12, 24, 48].includes(pageSizeParam) ? pageSizeParam : 12;

        const search = this.cleanString(params.get('search'));

        const category = this.cleanString(params.get('category'));

        const minPrice = this.parsePrice(params.get('minPrice'));

        const maxPrice = this.parsePrice(params.get('maxPrice'));

        const inStock = params.get('inStock') === 'true' ? true : undefined;

        const requestedSortBy = params.get('sortBy');

        const sortBy =
          requestedSortBy === 'name' || requestedSortBy === 'price' ? requestedSortBy : undefined;

        const requestedSortDirection = params.get('sortDirection');

        const sortDirection =
          requestedSortDirection === 'asc' || requestedSortDirection === 'desc'
            ? requestedSortDirection
            : undefined;

        return {
          page,
          pageSize,
          search,
          category,
          minPrice,
          maxPrice,
          inStock,
          sortBy,
          sortDirection,
        };
      }),

      distinctUntilChanged((previous, current) => this.queriesEqual(previous, current)),
    );

    combineLatest([query$, this.retryTrigger.pipe(startWith(undefined))])
      .pipe(
        switchMap(([query]) => {
          this.syncControls(query);

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

  applyFilters(): void {
    const search = this.cleanString(this.searchValue());

    const minPrice = this.parsePrice(this.minPriceValue());

    const maxPrice = this.parsePrice(this.maxPriceValue());

    const sort = this.getSortQuery();

    void this.router.navigate([], {
      relativeTo: this.route,

      queryParams: {
        page: 1,

        search: search ?? null,

        category: this.categoryValue() || null,

        minPrice: minPrice ?? null,

        maxPrice: maxPrice ?? null,

        inStock: this.inStockValue() ? true : null,

        sortBy: sort.sortBy ?? null,

        sortDirection: sort.sortDirection ?? null,
      },

      queryParamsHandling: 'merge',
    });

    this.filtersOpen.set(false);
  }

  selectCategory(category: string): void {
    this.categoryValue.set(category);

    this.applyFilters();
  }

  clearFilters(): void {
    this.searchValue.set('');
    this.categoryValue.set('');
    this.minPriceValue.set('');
    this.maxPriceValue.set('');
    this.inStockValue.set(false);
    this.sortValue.set('default');

    void this.router.navigate([], {
      relativeTo: this.route,

      queryParams: {
        page: 1,
        search: null,
        category: null,
        minPrice: null,
        maxPrice: null,
        inStock: null,
        sortBy: null,
        sortDirection: null,
      },

      queryParamsHandling: 'merge',
    });

    this.filtersOpen.set(false);
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

  toggleFilters(): void {
    this.filtersOpen.update((open) => !open);
  }

  retry(): void {
    this.retryTrigger.next();
  }

  private loadCategories(): void {
    this.categoriesLoading.set(true);

    this.productsApi
      .listCategories()
      .pipe(
        catchError(() => of([] as CategoryResponseDto[])),

        finalize(() => {
          this.categoriesLoading.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((categories) => {
        this.categories.set(categories);
      });
  }

  private syncControls(query: ProductQuery): void {
    this.searchValue.set(query.search ?? '');

    this.categoryValue.set(query.category ?? '');

    this.minPriceValue.set(query.minPrice !== undefined ? String(query.minPrice) : '');

    this.maxPriceValue.set(query.maxPrice !== undefined ? String(query.maxPrice) : '');

    this.inStockValue.set(query.inStock === true);

    if (query.sortBy && query.sortDirection) {
      this.sortValue.set(`${query.sortBy}-${query.sortDirection}`);
    } else {
      this.sortValue.set('default');
    }
  }

  private getSortQuery(): {
    sortBy?: string;
    sortDirection?: string;
  } {
    switch (this.sortValue()) {
      case 'name-asc':
        return {
          sortBy: 'name',
          sortDirection: 'asc',
        };

      case 'name-desc':
        return {
          sortBy: 'name',
          sortDirection: 'desc',
        };

      case 'price-asc':
        return {
          sortBy: 'price',
          sortDirection: 'asc',
        };

      case 'price-desc':
        return {
          sortBy: 'price',
          sortDirection: 'desc',
        };

      default:
        return {};
    }
  }

  private cleanString(value: string | null): string | undefined {
    const cleaned = value?.trim();

    return cleaned ? cleaned : undefined;
  }

  private parsePrice(value: string | null): number | undefined {
    if (value === null || value.trim() === '') {
      return undefined;
    }

    const parsed = Number(value);

    if (!Number.isFinite(parsed) || parsed < 0) {
      return undefined;
    }

    return parsed;
  }

  private queriesEqual(previous: ProductQuery, current: ProductQuery): boolean {
    return (
      previous.page === current.page &&
      previous.pageSize === current.pageSize &&
      previous.search === current.search &&
      previous.category === current.category &&
      previous.minPrice === current.minPrice &&
      previous.maxPrice === current.maxPrice &&
      previous.inStock === current.inStock &&
      previous.sortBy === current.sortBy &&
      previous.sortDirection === current.sortDirection
    );
  }
}
