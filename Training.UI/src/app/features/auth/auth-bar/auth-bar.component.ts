import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';

@Component({
  selector: 'app-auth-bar',
  imports: [RouterLink],
  templateUrl: './auth-bar.component.html',
  styleUrl: './auth-bar.component.scss'
})
export class AuthBarComponent implements OnInit {
  private auth = inject(AuthService)
  user = this.auth.user
  confirming = signal(false)
  error = signal<string | null>(null)

  ngOnInit() {
    this.auth.me().subscribe()
  }

  canEdit() {
    const roles = this.user()?.roles ?? []
    return roles.includes('Manager') || roles.includes('Admin')
  }

  loginUrl() {
    return this.auth.googleLoginUrl()
  }

  logout() {
    this.confirming.set(false)
    this.error.set(null)
    this.auth.logout().subscribe(() => this.auth.me().subscribe())
  }

  askDelete() {
    this.error.set(null)
    this.confirming.set(true)
  }

  cancelDelete() {
    this.confirming.set(false)
  }

  deleteAccount() {
    this.auth.deleteMe().subscribe({
      next: () => this.auth.me().subscribe(() => {
        this.confirming.set(false)
        this.error.set(null)
      }),
      error: (error: HttpErrorResponse) => {
        this.confirming.set(false)
        this.error.set(error.status === 409
          ? 'Не можна видалити останнього адміністратора.'
          : 'Не вдалося видалити дані.')
      }
    })
  }
}
