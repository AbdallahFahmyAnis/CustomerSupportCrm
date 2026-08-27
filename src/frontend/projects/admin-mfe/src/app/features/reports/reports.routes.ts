import { Routes } from '@angular/router';

/** SDD CRM-031…034 */
export const REPORTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./ticket-reports/ticket-reports.page').then((m) => m.TicketReportsPage),
  },
  {
    path: 'sla',
    loadComponent: () =>
      import('./sla-performance/sla-performance.page').then((m) => m.SlaPerformancePage),
  },
  {
    path: 'csat',
    loadComponent: () =>
      import('./csat-report/csat-report.page').then((m) => m.CsatReportPage),
  },
];
