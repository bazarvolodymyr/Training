import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AttemptResult } from '../interfaces/attempt-result.interface';
import { NextQuestion } from '../interfaces/next-question.interface';

@Injectable({
  providedIn: 'root'
})
export class QuestionsService {
  private http = inject(HttpClient)
  private baseApiUrl = 'http://localhost:5166/'

  next(lessonId: number, difficulty: string) {
    return this.http.get<NextQuestion>(
      `${this.baseApiUrl}api/v1/lessons/${lessonId}/questions/next?difficulty=${difficulty}`,
      { withCredentials: true }
    )
  }

  submit(exerciseId: number, difficulty: string, answer: string) {
    return this.http.post<AttemptResult>(
      `${this.baseApiUrl}api/v1/exercises/${exerciseId}/attempts`,
      { difficulty, answer },
      { withCredentials: true }
    )
  }
}
