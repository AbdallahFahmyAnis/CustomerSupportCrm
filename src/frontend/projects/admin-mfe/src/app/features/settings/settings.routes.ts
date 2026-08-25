import { Routes } from '@angular/router';

export const SETTINGS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./settings-edit/settings-edit.page').then((m) => m.SettingsEditPage),
  },
];
