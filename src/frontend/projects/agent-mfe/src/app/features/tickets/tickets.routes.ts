import { Routes } from '@angular/router';

/** SDD CRM-004…007 — tickets feature routes. */
export const TICKETS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./ticket-list/ticket-list.page').then((m) => m.TicketListPage),
  },
  {
    path: 'new',
    loadComponent: () => import('./create-ticket/create-ticket.page').then((m) => m.TicketCreatePage),
  },
  {
    path: ':id',
    loadComponent: () => import('./ticket-detail/ticket-detail.page').then((m) => m.TicketDetailPage),
  },
];
