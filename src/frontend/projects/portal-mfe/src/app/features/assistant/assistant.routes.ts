import { Routes } from '@angular/router';

/** SDD CRM-026 — portal AI assistant. */
export const ASSISTANT_ROUTES: Routes = [
  {
    path: 'assistant',
    loadComponent: () =>
      import('./assistant-page/assistant.page').then((m) => m.AssistantPage),
  },
];
