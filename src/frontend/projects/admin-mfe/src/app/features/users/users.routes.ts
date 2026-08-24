import { Routes } from '@angular/router';

export const USERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./user-list/user-list.page').then((m) => m.UserListPage),
  },
  {
    path: 'new',
    loadComponent: () => import('./create-user/create-user.page').then((m) => m.UserCreatePage),
  },
];
