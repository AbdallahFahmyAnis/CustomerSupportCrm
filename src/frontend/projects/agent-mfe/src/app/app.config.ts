import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { AGENT_ROUTES } from './remote.routes';
import { provideAgentCore } from './core/core.providers';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(AGENT_ROUTES),
    provideAgentCore(),
  ],
};
