import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { StoreFooter } from '../store-footer/store-footer';
import { StoreHeader } from '../store-header/store-header';

@Component({
  selector: 'app-storefront-shell',
  imports: [RouterOutlet, StoreHeader, StoreFooter],
  templateUrl: './storefront-shell.html',
  styleUrl: './storefront-shell.scss',
})
export class StorefrontShell {}
