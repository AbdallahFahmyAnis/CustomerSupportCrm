import { Routes } from '@angular/router';
import { AdminHomePage } from './features/home/home/home.page';

/** SDD CRM-035 / CRM-036 / CRM-037 — admin Feature-Based routes. */
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
];
