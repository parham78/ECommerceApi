import { Component, computed, input } from '@angular/core';

const PRODUCT_IMAGES: Record<string, string> = {
  'LEGACY-1': '/images/products/keyboard.webp',
  'LEGACY-4': '/images/products/mouse.webp',
  'MONITOR-GAMING-001': '/images/products/gaming-monitor.webp',

  'HEADSET-001': '/images/products/headset.webp',
  'WEBCAM-001': '/images/products/webcam.webp',
  'USB-HUB-001': '/images/products/usb-c-hub.webp',
  'SSD-EXT-001': '/images/products/external-ssd.webp',
  'LAPTOP-STAND-001': '/images/products/laptop-stand.webp',
  'DESK-LAMP-001': '/images/products/desk-lamp.webp',
  'MICROPHONE-001': '/images/products/usb-microphone.webp',
  'SPEAKERS-001': '/images/products/desktop-speakers.webp',
  'CONTROLLER-001': '/images/products/game-controller.webp',
  'DOCK-001': '/images/products/docking-station.webp',
  'CHARGER-WL-001': '/images/products/wireless-charger.webp',
  'MONITOR-PORT-001': '/images/products/portable-monitor.webp',
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
