import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { firstValueFrom } from 'rxjs';

export function authInitializer() {
  const auth = inject(AuthService);

  return () => {
    return firstValueFrom(
      auth.me()
    ).catch(() => {
      // silent fail (user not logged in)
      return Promise.resolve();
    });
  };
}
