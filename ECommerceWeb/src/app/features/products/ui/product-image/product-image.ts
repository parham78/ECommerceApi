import { Component, computed, input } from '@angular/core';

const PRODUCT_IMAGES: Record<string, string> = {
  'LEGACY-1': '/images/products/keyboard.webp',
  'LEGACY-4': '/images/products/mouse.webp',
  'MONITOR-GAMING-001': '/images/products/gaming-monitor.webp',
};

@Component({
  selector: 'app-product-image',
  templateUrl: './product-image.html',
  styleUrl: './product-image.scss',
})
export class ProductImage {
  readonly sku = input.required<string>();
  readonly productName = input.required<string>();

  readonly imageSrc = computed(() => PRODUCT_IMAGES[this.sku()] ?? null);
}
