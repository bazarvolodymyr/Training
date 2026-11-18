import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthUser } from '../interfaces/auth-user.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient)
  private baseApiUrl = 'http://localhost:5166/'

  me() {
    return this.http.get<AuthUser>(`${this.baseApiUrl}api/v1/auth/me`, { withCredentials: true })
  }

  logout() {
    return this.http.post<void>(`${this.baseApiUrl}api/v1/auth/logout`, {}, { withCredentials: true })
  }

  googleLoginUrl() {
    return `${this.baseApiUrl}api/v1/auth/oauth/google?returnUrl=${encodeURIComponent(window.location.origin)}`
  }
}
