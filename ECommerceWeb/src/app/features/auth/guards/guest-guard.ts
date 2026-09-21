import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthSession } from '../data-access/auth-session';

export const guestGuard: CanActivateFn = () => {
  const authSession = inject(AuthSession);
  const router = inject(Router);

  if (authSession.isAuthenticated()) {
    return router.createUrlTree(['/products']);
  }

  return true;
};
