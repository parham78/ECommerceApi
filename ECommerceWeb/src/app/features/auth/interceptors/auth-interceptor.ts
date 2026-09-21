import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { AuthSession } from '../data-access/auth-session';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authSession = inject(AuthSession);

  const token = authSession.token();

  if (!token || !authSession.isAuthenticated()) {
    return next(request);
  }

  const authenticatedRequest = request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authenticatedRequest);
};
