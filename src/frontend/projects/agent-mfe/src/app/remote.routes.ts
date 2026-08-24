import { Routes } from '@angular/router';
import { AgentHomePage } from './features/home/home/home.page';

/** SDD 002–003 — customers + tickets Feature-Based routes. */
export const AGENT_ROUTES: Routes = [
  { path: '', component: AgentHomePage },
  {
    path: 'customers',
    loadChildren: () =>
      import('./features/customers/customers.routes').then((m) => m.CUSTOMERS_ROUTES),
  },
  {
    path: 'tickets',
    loadChildren: () => import('./features/tickets/tickets.routes').then((m) => m.TICKETS_ROUTES),
  },
];
