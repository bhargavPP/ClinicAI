// src/app/core/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, switchMap, filter, take, catchError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // ✅ Always send cookies
    const clonedRequest = req.clone({
      withCredentials: true
    });

    return next.handle(clonedRequest).pipe(
      catchError((err: HttpErrorResponse) => {

        // 🔁 Handle 401 → refresh token
        if (err.status === 401 && !this.isRefreshing) {

          this.isRefreshing = true;

          return this.auth.refreshToken().pipe(
            switchMap(() => {
              this.isRefreshing = false;

              // ✅ Retry original request
              return next.handle(req.clone({ withCredentials: true }));
            }),
            catchError(error => {
              this.isRefreshing = false;
              this.auth.logout();
              return throwError(() => error);
            })
          );
        }

        return throwError(() => err);
      })
    );
  }

  private addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);

      const refreshToken = this.auth.getRefreshToken();

      if (!refreshToken) {
        this.auth.logout();
        return throwError(() => 'No refresh token');
      }

      return this.auth.refreshToken().pipe(
        switchMap((res) => {
          this.isRefreshing = false;

          // ✅ Store new tokens
          this.auth.StoreTokens(res.accessToken, res.refreshToken);

          this.refreshSubject.next(res.accessToken);

          return next.handle(this.addToken(req, res.accessToken));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.auth.logout();
          return throwError(() => err);
        })
      );
    }

    return this.refreshSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => next.handle(this.addToken(req, token!)))
    );
  }
}
