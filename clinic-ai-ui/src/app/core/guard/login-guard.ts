// src/app/core/guard/login-guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const loginGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // Already logged in → go straight to dashboard
  if (auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }
  return true;
};
