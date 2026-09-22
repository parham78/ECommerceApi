import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthSession } from '../../features/auth/data-access/auth-session';

@Component({
  selector: 'app-admin-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './admin-shell.html',
  styleUrl: './admin-shell.scss',
})
export class AdminShell {
  private readonly authSession = inject(AuthSession);

  private readonly router = inject(Router);

  signOut(): void {
    this.authSession.clear();

    void this.router.navigate(['/login']);
  }
}
