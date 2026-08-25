import { Routes } from '@angular/router';

/** SDD CRM-043 */
export const DEPARTMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./departments-list/departments.page').then((m) => m.DepartmentsPage),
  },
];
