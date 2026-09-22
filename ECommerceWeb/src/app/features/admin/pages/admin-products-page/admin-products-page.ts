import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, of, switchMap } from 'rxjs';

import { CategoryResponseDto } from '../../../products/data-access/category.dto';
import { ProductsApi } from '../../../products/data-access/products-api';

import {
  AdminProductResponseDto,
  UpdateProductRequestDto,
} from '../../data-access/admin-product.dto';
import { AdminProductsApi } from '../../data-access/admin-products-api';

@Component({
  selector: 'app-admin-products-page',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-products-page.html',
  styleUrl: './admin-products-page.scss',
})
export class AdminProductsPage implements OnInit {
  private readonly productsApi = inject(AdminProductsApi);

  private readonly publicProductsApi = inject(ProductsApi);

  private readonly formBuilder = inject(FormBuilder);

  private readonly destroyRef = inject(DestroyRef);

  readonly products = signal<AdminProductResponseDto[]>([]);

  readonly categories = signal<CategoryResponseDto[]>([]);

  readonly currentPage = signal(1);

  readonly pageSize = signal(10);

  readonly totalCount = signal(0);

  readonly totalPages = signal(0);

  readonly isLoading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly activeFilter = signal<'all' | 'active' | 'inactive'>('all');

  readonly editingProduct = signal<AdminProductResponseDto | null>(null);

  readonly isSaving = signal(false);

  readonly editErrorMessage = signal<string | null>(null);

  readonly editForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],

    sku: ['', [Validators.required, Validators.maxLength(50)]],

    price: [0, [Validators.required, Validators.min(0.01)]],

    categoryId: [0, [Validators.required, Validators.min(1)]],

    stock: [0, [Validators.required, Validators.min(0)]],

    isActive: [true],
  });

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadProducts(page = this.currentPage()): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const filter = this.activeFilter() === 'all' ? undefined : this.activeFilter() === 'active';

    this.productsApi
      .list(page, this.pageSize(), filter)
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          this.products.set(result.items);

          this.currentPage.set(result.currentPage);

          this.pageSize.set(result.pageSize);

          this.totalCount.set(result.totalCount);

          this.totalPages.set(result.totalPages);
        },

        error: () => {
          this.errorMessage.set('We could not load the products.');
        },
      });
  }

  loadCategories(): void {
    this.publicProductsApi
      .listCategories()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (categories) => {
          this.categories.set(categories);
        },

        error: () => {
          this.categories.set([]);
        },
      });
  }

  setActiveFilter(filter: 'all' | 'active' | 'inactive'): void {
    this.activeFilter.set(filter);
    this.loadProducts(1);
  }

  previousPage(): void {
    if (this.currentPage() <= 1) {
      return;
    }

    this.loadProducts(this.currentPage() - 1);
  }

  nextPage(): void {
    if (this.currentPage() >= this.totalPages()) {
      return;
    }

    this.loadProducts(this.currentPage() + 1);
  }

  openEdit(product: AdminProductResponseDto): void {
    this.editErrorMessage.set(null);
    this.editingProduct.set(product);

    this.editForm.setValue({
      name: product.name,
      sku: product.sku,
      price: product.price,
      categoryId: product.categoryId,
      stock: product.stock,
      isActive: product.isActive,
    });
  }

  closeEdit(): void {
    if (this.isSaving()) {
      return;
    }

    this.editingProduct.set(null);
    this.editErrorMessage.set(null);
    this.editForm.reset();
  }

  saveProduct(): void {
    const original = this.editingProduct();

    if (!original) {
      return;
    }

    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const formValue = this.editForm.getRawValue();

    const updateRequest: UpdateProductRequestDto = {
      name: formValue.name.trim(),
      sku: formValue.sku.trim(),
      price: formValue.price,
      categoryId: formValue.categoryId,
      isActive: formValue.isActive,
      rowVersion: original.rowVersion,
    };

    this.isSaving.set(true);
    this.editErrorMessage.set(null);

    this.productsApi
      .update(original.id, updateRequest)
      .pipe(
        switchMap((updatedProduct) => {
          this.editingProduct.set(updatedProduct);

          if (formValue.stock === original.stock) {
            return of(updatedProduct);
          }

          return this.productsApi.updateStock(original.id, {
            newStock: formValue.stock,

            rowVersion: updatedProduct.rowVersion,
          });
        }),

        finalize(() => {
          this.isSaving.set(false);
        }),

        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (updatedProduct) => {
          this.products.update((products) =>
            products.map((product) =>
              product.id === updatedProduct.id ? updatedProduct : product,
            ),
          );

          this.editingProduct.set(null);
          this.editErrorMessage.set(null);
        },

        error: (error: HttpErrorResponse) => {
          if (error.status === 409) {
            this.editErrorMessage.set(
              'This product changed while you were editing it. Refresh the page and try again.',
            );

            return;
          }

          if (error.status === 400) {
            this.editErrorMessage.set('Please check the product information and try again.');

            return;
          }

          this.editErrorMessage.set('We could not save this product. Please try again.');
        },
      });
  }
}
