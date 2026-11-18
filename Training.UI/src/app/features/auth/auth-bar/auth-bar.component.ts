import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../data/services/auth.service';
import { AuthUser } from '../../../data/interfaces/auth-user.interface';

@Component({
  selector: 'app-auth-bar',
  imports: [],
  templateUrl: './auth-bar.component.html',
  styleUrl: './auth-bar.component.scss'
})
export class AuthBarComponent implements OnInit {
  private auth = inject(AuthService)
  user = signal<AuthUser | null>(null)

  ngOnInit() {
    this.auth.me().subscribe(user => this.user.set(user))
  }

  loginUrl() {
    return this.auth.googleLoginUrl()
  }

  logout() {
    this.auth.logout().subscribe(() => this.auth.me().subscribe(user => this.user.set(user)))
  }
}
