import { Routes } from '@angular/router';

/** SDD CRM-031…034 */
export const REPORTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./ticket-reports/ticket-reports.page').then((m) => m.TicketReportsPage),
  },
];
