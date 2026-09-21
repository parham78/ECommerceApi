import { CurrencyPipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ProductResponseDto } from '../../data-access/product.dto';
import { ProductImage } from '../product-image/product-image';

@Component({
  selector: 'app-product-card',
  imports: [ProductImage, CurrencyPipe, RouterLink],
  templateUrl: './product-card.html',
  styleUrl: './product-card.scss',
})
export class ProductCard {
  readonly product = input.required<ProductResponseDto>();
}
