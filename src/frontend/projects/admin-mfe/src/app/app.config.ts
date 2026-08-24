import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { ADMIN_ROUTES } from './remote.routes';
import { provideAdminCore } from './core/core.providers';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(ADMIN_ROUTES),
    provideAdminCore(),
  ],
};
