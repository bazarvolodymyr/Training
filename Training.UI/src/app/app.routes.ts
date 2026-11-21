import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/learn/catalog/catalog.component').then(m => m.CatalogComponent)
  },
  {
    path: 'lessons/:id',
    loadComponent: () => import('./features/learn/lesson/lesson.component').then(m => m.LessonComponent)
  },
  {
    path: 'consent',
    loadComponent: () => import('./features/auth/consent/consent.component').then(m => m.ConsentComponent)
  }
];
