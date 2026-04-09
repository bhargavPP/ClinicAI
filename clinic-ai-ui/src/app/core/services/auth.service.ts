import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
 
export interface AuthUser {
  id: number;
  email: string;
  name: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
}
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: AuthUser;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly API_URL = `${environment.apiUrl}/auth`;
  private readonly USER_KEY = 'auth_user';
  private _currentUser = signal<AuthUser | null>(this.getUserFromStorage());
  private _isLoading = signal<boolean>(false);

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly isAuthenticated = computed(() => !!this._currentUser());
  //readonly userRoles = computed(() => this._currentUser()?.role ?? []);

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    this._isLoading.set(true);
    return this.http.post<AuthResponse>(`${this.API_URL}/login`, credentials, {withCredentials:true}).pipe(
      tap((response) => {
        this._isLoading.set(false);
        this.setUser(response.user);
      }),
      catchError((error) => {
        this._isLoading.set(false);
        console.error("🔥 FULL ERROR:", error);
  return throwError(() => error);
      //  return throwError(() => new Error(error.error?.message || 'Login failed'));
      }));
  }

  logout(): void {
    this.http.post(`${this.API_URL}/logout`, {}, {
      withCredentials: true
    }).subscribe();

    this._currentUser.set(null);
    localStorage.removeItem(this.USER_KEY); 
    this.router.navigate(['/login']);
  }
  private getUserFromStorage(): AuthUser | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }
    
  refreshToken(): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap((response) => {
        this._currentUser.set(response.user);
      }) 
    );
  }
  private setUser(user: AuthUser) {
    this._currentUser.set(user);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }   

  hasRole(role: string): boolean {
    return this._currentUser()?.role===role;
  }
  register(payload: RegisterRequest): Observable<any> {
    this._isLoading.set(true);
   
    return this.http.post<any>(`${this.API_URL}/register`, payload, {
      withCredentials: true
    }).pipe(
      tap(response => {
        this._isLoading.set(false);
        this.setUser(response.user);
      }),
      catchError(err => {
        this._isLoading.set(false);
        return throwError(() => err);
      })
    );
  }
  me(): Observable<any> {
    return this.http.get<any>(`${this.API_URL}/me`).pipe(
      tap(user => {
        // Only set if we got a real user back
        if (user?.userId) {
          this._currentUser.set({
            id: user.userId,
            email: user.email,
            name: user.name ?? '',
            role: user.role
          });
        }
      }),   // cache so guard doesn't re-fetch
      catchError(() => {
        this._currentUser.set(null);
        return of(null);
      })
    );
  }
   
  getCurrentUser() {
    return this._currentUser();
  }

}
