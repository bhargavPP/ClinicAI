import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const isLoggedIn = auth.isAuthenticated() ;

  if (isLoggedIn) return true;

  router.navigate(['/login'], {
    queryParams: { returnUrl: location.pathname }
  });
  return false;
};
