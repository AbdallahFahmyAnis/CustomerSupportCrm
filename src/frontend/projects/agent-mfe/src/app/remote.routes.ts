import { Routes } from '@angular/router';
import { AgentWorkspaceComponent } from './agent-workspace.component';
import { CustomerCreateComponent } from './customers/customer-create.component';
import { CustomerDetailComponent } from './customers/customer-detail.component';
import { CustomerEditComponent } from './customers/customer-edit.component';
import { CustomerListComponent } from './customers/customer-list.component';

/** SDD 002–003 — customers + Feature-Based tickets */
export const AGENT_ROUTES: Routes = [
  { path: '', component: AgentWorkspaceComponent },
  { path: 'customers', component: CustomerListComponent },
  { path: 'customers/new', component: CustomerCreateComponent },
  { path: 'customers/:id', component: CustomerDetailComponent },
  { path: 'customers/:id/edit', component: CustomerEditComponent },
  {
    path: 'tickets',
    loadChildren: () => import('./features/tickets/tickets.routes').then((m) => m.TICKETS_ROUTES),
  },
];
