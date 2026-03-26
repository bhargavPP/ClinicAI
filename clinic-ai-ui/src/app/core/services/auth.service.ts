import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';

export interface AuthUser {
  id: number;
  email: string;
  name: string;
  roles: string[];
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
  private readonly API_URL = 'https://your-api.com/api/auth'; // 🔁 Replace with your .NET API base URL
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  private _currentUser = signal<AuthUser | null>(this.getUserFromStorage());
  private _isLoading = signal<boolean>(false);

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly isAuthenticated = computed(() => !!this._currentUser());
  readonly userRoles = computed(() => this._currentUser()?.roles ?? []);

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    this._isLoading.set(true);
    return this.http.post<AuthResponse>(`${this.API_URL}/login`, credentials).pipe(
      tap((response) => {
        this._isLoading.set(false);
        this._currentUser.set(response.user);
        this.StoreTokens(response.accessToken, response.refreshToken);
      }),
      catchError((error) => {
        this._isLoading.set(false);
        return throwError(() => new Error(error.error?.message || 'Login failed'));
      }));
  }

  logOut(): void {
    this.clearTokens();
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }
  private getUserFromStorage(): AuthUser | null {
    const token = localStorage.getItem(this.ACCESS_TOKEN_KEY);
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.user as AuthUser ?? null;
    } catch {
      return null;
    }
  }
  private StoreTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }
  private clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http.post<AuthResponse>(`${this.API_URL}/refresh`, { refreshToken }).pipe(
      tap((response) => {
        this.StoreTokens(response.accessToken, response.refreshToken);
        this._currentUser.set(response.user);
      }),
      catchError((error) => {
        this.logOut();
        return throwError(() => new Error(error.error?.message || 'Token refresh failed'));
      }));
  }
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }
  isTokenExpired(): boolean {
    const token = this.getAccessToken();
    if (!token) return true;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  hasRole(role: string): boolean {
    return this.userRoles().includes(role);
  }
  register(payload: RegisterRequest): Observable<AuthResponse> {
    this._isLoading.set(true);
    return this.http.post<AuthResponse>(`${this.API_URL}/register`, payload).pipe(
      tap(response => {
        this.StoreTokens(response.accessToken, response.refreshToken);
        this._currentUser.set(response.user);
        this._isLoading.set(false);
      }),
      catchError(err => {
        this._isLoading.set(false);
        return throwError(() => err);
      })
    );
  }
}
