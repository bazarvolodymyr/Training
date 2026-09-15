import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Question } from '../interfaces/question.interface';
import { Answer } from '../interfaces/answer.interface';
import { AnswerResult } from '../interfaces/answer-result.interface';

@Injectable({
  providedIn: 'root'
})
export class QuestionsService {
http = inject(HttpClient)
baseApiUrl = 'http://localhost:5166/';
  constructor() { }
  getQuestion(){
    return this.http.get<Question>(`${this.baseApiUrl}Exercise/GetQuestion`)
  }
  sendAnswer(answer:Answer){
    return this.http.post<AnswerResult>(`${this.baseApiUrl}Exercise/QuestionResponse`,answer)
  }
}
