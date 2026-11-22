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
    path: 'admin',
    loadComponent: () => import('./features/admin/media-library/media-library.component').then(m => m.MediaLibraryComponent)
  },
  {
    path: 'admin/lessons/:id',
    loadComponent: () => import('./features/admin/lesson-media/lesson-media.component').then(m => m.LessonMediaComponent)
  },
  {
    path: 'consent',
    loadComponent: () => import('./features/auth/consent/consent.component').then(m => m.ConsentComponent)
  }
];
