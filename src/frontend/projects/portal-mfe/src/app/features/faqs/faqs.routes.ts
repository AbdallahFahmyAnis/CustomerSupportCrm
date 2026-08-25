import { Routes } from '@angular/router';

/** SDD CRM-029 — portal FAQ routes. */
export const FAQS_ROUTES: Routes = [
  {
    path: 'faqs',
    loadComponent: () =>
      import('./faq-list/faq-list.page').then((m) => m.FaqListPage),
  },
  {
    path: 'faqs/:id',
    loadComponent: () =>
      import('./faq-detail/faq-detail.page').then((m) => m.FaqDetailPage),
  },
];
