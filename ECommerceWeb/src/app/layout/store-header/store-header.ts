import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthSession } from '../../features/auth/data-access/auth-session';

@Component({
  selector: 'app-store-header',
  imports: [RouterLink],
  templateUrl: './store-header.html',
  styleUrl: './store-header.scss',
})
export class StoreHeader {
  readonly authSession = inject(AuthSession);

  signOut(): void {
    this.authSession.clear();
  }
}
