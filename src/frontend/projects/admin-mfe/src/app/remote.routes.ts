import { Routes } from '@angular/router';
import { AdminHomePage } from './features/home/home/home.page';

/** SDD CRM-035 — admin Feature-Based routes. */
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
];
