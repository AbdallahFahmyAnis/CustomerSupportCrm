import { Routes } from '@angular/router';

/** SDD CRM-027 / CRM-028 — portal requests feature routes. */
export const REQUESTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./portal-home/portal-home.page').then((m) => m.PortalHomePage),
  },
  {
    path: 'submit',
    loadComponent: () =>
      import('./submit-request/submit-request.page').then((m) => m.SubmitRequestPage),
  },
  {
    path: 'track',
    loadComponent: () =>
      import('./track-requests/track-requests.page').then((m) => m.TrackRequestsPage),
  },
];
