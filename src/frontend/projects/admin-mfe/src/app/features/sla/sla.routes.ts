import { Routes } from '@angular/router';

export const SLA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./sla-policies/sla-policies.page').then((m) => m.SlaPoliciesPage),
  },
];
