import { loadRemoteModule } from '@angular-architects/native-federation';
import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent },
  {
    path: 'agent',
    loadChildren: () =>
      loadRemoteModule('agent-mfe', './Routes').then((m) => m.AGENT_ROUTES),
  },
  {
    path: 'portal',
    loadChildren: () =>
      loadRemoteModule('portal-mfe', './Routes').then((m) => m.PORTAL_ROUTES),
  },
  {
    path: 'admin',
    loadChildren: () =>
      loadRemoteModule('admin-mfe', './Routes').then((m) => m.ADMIN_ROUTES),
  },
  {
    path: 'knowledge',
    loadChildren: () =>
      loadRemoteModule('knowledge-mfe', './Routes').then((m) => m.KNOWLEDGE_ROUTES),
  },
];
