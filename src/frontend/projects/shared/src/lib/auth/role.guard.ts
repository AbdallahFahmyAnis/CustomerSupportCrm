import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { SessionApi } from '../session.api';
import { canAccessAdmin, canAccessAgentWorkspace, homePathForRole } from './roles';

function ensureSession() {
  const session = inject(SessionApi);
  const current = session.session();
  if (current?.authenticated) {
    return of(current);
  }
  return session.load().pipe(map((s) => (s?.authenticated ? s : null)));
}

/** Admin MFE — Admin role only. */
export const adminRoleGuard: CanActivateFn = () => {
  const router = inject(Router);
  return ensureSession().pipe(
    map((s) => {
      if (!s) {
        return router.createUrlTree(['/']);
      }
      if (canAccessAdmin(s.role)) {
        return true;
      }
      return router.createUrlTree([homePathForRole(s.role)]);
    }),
    catchError(() => of(router.createUrlTree(['/']))),
  );
};

/** Agent workspace — Agent, Lead, or Admin. */
export const agentRoleGuard: CanActivateFn = () => {
  const router = inject(Router);
  return ensureSession().pipe(
    map((s) => {
      if (!s) {
        return router.createUrlTree(['/']);
      }
      if (canAccessAgentWorkspace(s.role)) {
        return true;
      }
      return router.createUrlTree([homePathForRole(s.role)]);
    }),
    catchError(() => of(router.createUrlTree(['/']))),
  );
};
