import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { AuthUser } from '../interfaces/auth-user.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient)
  private baseApiUrl = 'http://localhost:5166/'
  user = signal<AuthUser | null>(null)

  me() {
    return this.http.get<AuthUser>(`${this.baseApiUrl}api/v1/auth/me`, { withCredentials: true }).pipe(
      tap(user => this.user.set(user))
    )
  }

  logout() {
    return this.http.post<void>(`${this.baseApiUrl}api/v1/auth/logout`, {}, { withCredentials: true })
  }

  acceptConsent() {
    return this.http.post<void>(`${this.baseApiUrl}api/v1/auth/consent`, {}, { withCredentials: true })
  }

  deleteMe() {
    return this.http.delete<void>(`${this.baseApiUrl}api/v1/me`, { withCredentials: true })
  }

  googleLoginUrl() {
    return `${this.baseApiUrl}api/v1/auth/oauth/google?returnUrl=${encodeURIComponent(window.location.origin)}`
  }
}
