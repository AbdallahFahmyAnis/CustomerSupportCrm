import { Routes } from '@angular/router';

/** SDD CRM-021 — knowledge Feature-Based routes. */
export const KNOWLEDGE_ROUTES: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./features/articles/articles.routes').then((m) => m.ARTICLES_ROUTES),
  },
];
