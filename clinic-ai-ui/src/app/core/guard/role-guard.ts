import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data?.['roles'] as string[];

  const user = auth.getCurrentUser();

  if (!user) {
    router.navigate(['/login']);
    return false;
  } 
  //console.log(user);
  //const userRoles: string[] = user.roles ?? [user.roles];

  // ✅ Admin override
  if (user.role =='Admin') return true;

  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  const hasRole = requiredRoles.some(role =>
    user.role==role);

  if (hasRole) return true;

  router.navigate(['/unauthorized']);
  return false;
};
