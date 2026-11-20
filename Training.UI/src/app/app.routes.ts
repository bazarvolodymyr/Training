import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'consent',
    loadComponent: () => import('./features/auth/consent/consent.component').then(m => m.ConsentComponent)
  }
];
