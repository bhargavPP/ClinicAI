// src/app/core/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, switchMap, filter, take, catchError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<boolean>(false);

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const clonedRequest = req.clone({
      withCredentials: true
    });

    return next.handle(clonedRequest).pipe(
      catchError((err: HttpErrorResponse) => {

        // ❌ Ignore auth endpoints
        if (req.url.includes('/auth/login') ||
          req.url.includes('/auth/register') ||
          req.url.includes('/auth/refresh') ||
          req.url.includes('/auth/me')||
           req.url.includes('/auth/logout'))
         {
          return throwError(() => err);
        }

        // 🔁 Handle 401
        if (err.status === 401) {

          if (!this.isRefreshing) {
            this.isRefreshing = true;
            this.refreshSubject.next(false);

            return this.auth.refreshToken().pipe(
              switchMap(() => {
                this.isRefreshing = false;
                this.refreshSubject.next(true);

                return next.handle(req.clone({ withCredentials: true }));
              }),
              catchError(error => {
                this.isRefreshing = false;
                this.auth.logout();
                console.log("error", error);
                return throwError(() => error);
              })
            );
          } else {
            // ⏳ Wait until refresh completes
            return this.refreshSubject.pipe(
              filter(done => done === true),
              take(1),
              switchMap(() => next.handle(clonedRequest))
            );
          }
        }

        return throwError(() => err);
      })
    );
  }

}
