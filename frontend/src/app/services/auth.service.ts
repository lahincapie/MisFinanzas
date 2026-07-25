import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResult } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;
  private readonly tokenKey = 'misfinanzas_token';

  login(credentials: LoginRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${this.baseUrl}/login`, credentials).pipe(
      tap(result => this.saveToken(result.token))
    );
  }

  register(data: RegisterRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${this.baseUrl}/register`, data).pipe(
      tap(result => this.saveToken(result.token))
    );
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  private saveToken(token: string): void {
    sessionStorage.setItem(this.tokenKey, token);
  }
}