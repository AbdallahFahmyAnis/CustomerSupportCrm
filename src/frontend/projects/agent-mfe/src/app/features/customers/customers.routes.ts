import { Routes } from '@angular/router';

/** SDD CRM-001…003 — customers feature routes. */
export const CUSTOMERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./customer-list/customer-list.page').then((m) => m.CustomerListComponent),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./create-customer/create-customer.page').then((m) => m.CustomerCreateComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./customer-detail/customer-detail.page').then((m) => m.CustomerDetailComponent),
  },
  {
    path: ':id/edit',
    loadComponent: () =>
      import('./edit-customer/edit-customer.page').then((m) => m.CustomerEditComponent),
  },
];
