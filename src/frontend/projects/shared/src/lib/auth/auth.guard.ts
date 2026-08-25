import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { SessionApi } from '../session.api';

/** Require an authenticated session; otherwise send to sign-in. */
export const authGuard: CanActivateFn = () => {
  const session = inject(SessionApi);
  const router = inject(Router);
  const current = session.session();
  if (current?.authenticated) {
    return true;
  }
  return session.load().pipe(
    map((s) => (s?.authenticated ? true : router.createUrlTree(['/']))),
    catchError(() => of(router.createUrlTree(['/']))),
  );
};
