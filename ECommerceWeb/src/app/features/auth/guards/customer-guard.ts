import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthSession } from '../data-access/auth-session';

export const customerGuard: CanActivateFn = () => {
  const authSession = inject(AuthSession);
  const router = inject(Router);

  if (!authSession.isAuthenticated()) {
    return router.createUrlTree(['/login']);
  }

  if (!authSession.roles().includes('Customer')) {
    return router.createUrlTree(['/products']);
  }

  return true;
};
