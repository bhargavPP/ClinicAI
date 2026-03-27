import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data?.['roles'] as string[];

  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  const hasRole = requiredRoles.some(role => auth.hasRole(role));

  if (!hasRole) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
