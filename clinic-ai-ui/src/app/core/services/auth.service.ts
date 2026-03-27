import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { enumRole } from '../../../common/enum';

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
  private readonly enumRol = enumRole;
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
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
        this._currentUser.set(response.user);
        localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
       // this.StoreTokens(response.accessToken, response.refreshToken);
      }),
      catchError((error) => {
        this._isLoading.set(false);
        return throwError(() => new Error(error.error?.message || 'Login failed'));
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
  public StoreTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }
  private clearTokens(): void {
    const refreshToken = this.getRefreshToken();

    if (refreshToken) {
      this.http.post(`${this.API_URL}/logout`, { refreshToken }).subscribe();
    }
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  refreshToken(): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap((response) => {
        this._currentUser.set(response.user);
      }),
      catchError((error) => {
        this.logout();
        return throwError(() => new Error('Token refresh failed'));
      })
    );
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
    return this._currentUser()?.role===role;
  }
  register(payload: RegisterRequest): Observable<any> {
    this._isLoading.set(true);
   
    return this.http.post<any>(`${this.API_URL}/register`, payload, {
      withCredentials: true
    }).pipe(
      tap(response => {
        this._currentUser.set(response.user);
        this._isLoading.set(false); localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
      }),
      catchError(err => {
        this._isLoading.set(false);
        return throwError(() => err);
      })
    );
  }

  getAuthHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${this.getAccessToken()}`
    });
  }
  getCurrentUser() {
    return this._currentUser();
  }

  //getUserRole(): string | null {
  //  return this._currentUser()?.roles || null;
  //}

  //isAdmin(): boolean {
  //  return this.getUserRole() === enumRole.Admin;
  //}

  //isDoctor(): boolean {
  //  return this.getUserRole() === enumRole.Doctor;
  //}

  //isPatient(): boolean {
  //  return this.getUserRole() === enumRole.Patient;
  //}
  // ── PRIVATE ────────────────────────────────────────────

  // Stores tokens + user in localStorage and updates signal
  private handleAuthSuccess(res: AuthResponse): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, res.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, res.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(res.user));
    this._currentUser.set(res.user);
  }


  private clearStorage(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}
