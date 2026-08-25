import { Routes } from '@angular/router';

export const ROLES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./role-list/role-list.page').then((m) => m.RoleListPage),
  },
  {
    path: 'permissions',
    loadComponent: () =>
      import('./permission-list/permission-list.page').then((m) => m.PermissionListPage),
  },
];
