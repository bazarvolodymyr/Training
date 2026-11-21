import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LessonArticle } from '../interfaces/lesson-article.interface';
import { LessonSummary } from '../interfaces/lesson-summary.interface';

@Injectable({
  providedIn: 'root'
})
export class LessonsService {
  private http = inject(HttpClient)
  private baseApiUrl = 'http://localhost:5166/'

  list() {
    return this.http.get<LessonSummary[]>(`${this.baseApiUrl}api/v1/lessons`)
  }

  get(id: number) {
    return this.http.get<LessonArticle>(`${this.baseApiUrl}api/v1/lessons/${id}`)
  }
}
