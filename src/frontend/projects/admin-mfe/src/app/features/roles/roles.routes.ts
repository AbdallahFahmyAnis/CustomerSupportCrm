import { Routes } from '@angular/router';

export const ROLES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/role-list.page').then((m) => m.RoleListPage),
  },
];
