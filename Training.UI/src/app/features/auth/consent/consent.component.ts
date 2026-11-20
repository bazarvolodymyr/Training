import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';

@Component({
  selector: 'app-consent',
  imports: [],
  templateUrl: './consent.component.html',
  styleUrl: './consent.component.scss'
})
export class ConsentComponent implements OnInit {
  private auth = inject(AuthService)
  private router = inject(Router)
  error = signal<string | null>(null)

  ngOnInit() {
    this.auth.me().subscribe(user => {
      if (user.isAuthenticated) {
        this.router.navigateByUrl('/')
      }
    })
  }

  accept() {
    this.error.set(null)
    this.auth.acceptConsent().subscribe({
      next: () => this.auth.me().subscribe(() => this.router.navigateByUrl('/')),
      error: () => this.error.set('Не вдалося зберегти згоду. Увійдіть через Google ще раз.')
    })
  }
}
