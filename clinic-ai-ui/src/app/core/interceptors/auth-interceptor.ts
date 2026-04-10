import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { switchMap, filter, take, catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // ❌ Skip auth endpoints entirely
    if (this.isAuthEndpoint(req.url)) {
      return next.handle(req.clone({ withCredentials: true }));
    }

    return next.handle(req.clone({ withCredentials: true })).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status !== 401) {
          return throwError(() => err);
        }

        return this.handle401(req, next);
      })
    );
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null); // block waiters

      return this.auth.refreshToken().pipe(
        switchMap(() => {
          this.isRefreshing = false;
          this.refreshSubject.next('done'); // release waiters
          return next.handle(req.clone({ withCredentials: true }));
        }),
        catchError(err => {
          this.isRefreshing = false;
          this.refreshSubject.next('error');
          this.auth.logout();
          return throwError(() => err);
        })
      );

    } else {
      // ⏳ Another request is already refreshing — wait for it
      return this.refreshSubject.pipe(
        filter(state => state !== null),  // wait until refresh completes
        take(1),
        switchMap(state => {
          if (state === 'error') {
            return throwError(() => new Error('Session expired'));
          }
          return next.handle(req.clone({ withCredentials: true }));
        })
      );
    }
  }

  private isAuthEndpoint(url: string): boolean {
    return ['/auth/login', '/auth/register', '/auth/refresh', '/auth/me', '/auth/logout']
      .some(path => url.includes(path));
  }
}
