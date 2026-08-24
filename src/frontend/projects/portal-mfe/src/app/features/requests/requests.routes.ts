import { Routes } from '@angular/router';

/** SDD CRM-027 / CRM-028 — portal requests feature routes. */
export const REQUESTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/portal-home.page').then((m) => m.PortalHomePage),
  },
  {
    path: 'submit',
    loadComponent: () =>
      import('./pages/submit-request.page').then((m) => m.SubmitRequestPage),
  },
  {
    path: 'track',
    loadComponent: () =>
      import('./pages/track-requests.page').then((m) => m.TrackRequestsPage),
  },
];
