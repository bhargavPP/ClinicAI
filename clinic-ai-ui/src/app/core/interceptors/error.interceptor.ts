import { Injectable,NgZone } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private zone: NgZone) { }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {

        let normalizedError: string[] = [];

        if (error.status === 400 && error.error?.errors) {
          const err = error.error.errors as Record<string, string[]>;
          normalizedError = Object.values(err).flat();
        }
        else if (typeof error.error === 'string') {
          normalizedError = [error.error];
        }
        else if (error.error?.message) {
          normalizedError = [error.error.message];
        }
        else {
          normalizedError = ['Something went wrong'];
        }

        return this.zone.run(() =>
          throwError(() => normalizedError)
        );
      })
    );
  }
}
