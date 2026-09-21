import { registerLocaleData } from '@angular/common';
import localeEnCa from '@angular/common/locales/en-CA';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { API_BASE_URL } from './core/config/api-base-url';
import { routes } from './app.routes';
import { authInterceptor } from './features/auth/interceptors/auth-interceptor';

registerLocaleData(localeEnCa);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    provideRouter(routes),

    provideHttpClient(withInterceptors([authInterceptor])),

    {
      provide: API_BASE_URL,
      useValue: '/api',
    },

    {
      provide: LOCALE_ID,
      useValue: 'en-CA',
    },
  ],
};
