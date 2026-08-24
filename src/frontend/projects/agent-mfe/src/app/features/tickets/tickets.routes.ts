import { Routes } from '@angular/router';

/** Feature routes — Feature-Based + Signals (tickets). */
export const TICKETS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/ticket-list.page').then((m) => m.TicketListPage),
  },
  {
    path: 'new',
    loadComponent: () => import('./pages/ticket-create.page').then((m) => m.TicketCreatePage),
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/ticket-detail.page').then((m) => m.TicketDetailPage),
  },
];
