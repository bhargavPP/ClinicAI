import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // ✅ If already verified in memory, allow immediately
  if (auth.currentUser()) return true;

  // ✅ Otherwise, ask the server (cookie is sent automatically)
  return auth.me().pipe(
    map(user => {
      if (user) return true;
      return router.createUrlTree(['/login']);
    }),
    catchError(() => of(router.createUrlTree(['/login'])))
  );
};
