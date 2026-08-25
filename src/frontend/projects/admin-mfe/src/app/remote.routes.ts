import { Routes } from '@angular/router';
import { AdminHomePage } from './features/home/home/home.page';

/** SDD CRM-035 / CRM-036 / CRM-037 / CRM-031…034 — admin Feature-Based routes. */
export const ADMIN_ROUTES: Routes = [
  { path: '', component: AdminHomePage },
  {
    path: 'users',
    loadChildren: () => import('./features/users/users.routes').then((m) => m.USERS_ROUTES),
  },
  {
    path: 'roles',
    loadChildren: () => import('./features/roles/roles.routes').then((m) => m.ROLES_ROUTES),
  },
  {
    path: 'audit',
    loadChildren: () => import('./features/audit/audit.routes').then((m) => m.AUDIT_ROUTES),
  },
  {
    path: 'settings',
    loadChildren: () => import('./features/settings/settings.routes').then((m) => m.SETTINGS_ROUTES),
  },
  {
    path: 'departments',
    loadChildren: () =>
      import('./features/departments/departments.routes').then((m) => m.DEPARTMENTS_ROUTES),
  },
  {
    path: 'sla',
    loadChildren: () => import('./features/sla/sla.routes').then((m) => m.SLA_ROUTES),
  },
  {
    path: 'reports',
    loadChildren: () => import('./features/reports/reports.routes').then((m) => m.REPORTS_ROUTES),
  },
];
