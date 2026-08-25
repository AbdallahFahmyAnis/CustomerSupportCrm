import { loadRemoteModule } from '@angular-architects/native-federation';
import { Routes } from '@angular/router';
import { adminRoleGuard, agentRoleGuard, authGuard } from 'shared';
import { HomePage } from './features/home/home/home.page';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomePage },
  {
    path: 'agent',
    canActivate: [authGuard, agentRoleGuard],
    loadChildren: () =>
      loadRemoteModule('agent-mfe', './Routes').then((m) => m.AGENT_ROUTES),
  },
  {
    path: 'portal',
    canActivate: [authGuard],
    loadChildren: () =>
      loadRemoteModule('portal-mfe', './Routes').then((m) => m.PORTAL_ROUTES),
  },
  {
    path: 'admin',
    canActivate: [authGuard, adminRoleGuard],
    loadChildren: () =>
      loadRemoteModule('admin-mfe', './Routes').then((m) => m.ADMIN_ROUTES),
  },
  {
    path: 'knowledge',
    canActivate: [authGuard, adminRoleGuard],
    loadChildren: () =>
      loadRemoteModule('knowledge-mfe', './Routes').then((m) => m.KNOWLEDGE_ROUTES),
  },
];
