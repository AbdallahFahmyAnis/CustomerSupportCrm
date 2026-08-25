import { Routes } from '@angular/router';

export const AUDIT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./audit-list/audit-list.page').then((m) => m.AuditListPage),
  },
];
