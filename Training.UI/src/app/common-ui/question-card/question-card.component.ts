import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AttemptResult } from '../../data/interfaces/attempt-result.interface';
import { NextQuestion } from '../../data/interfaces/next-question.interface';
import { AuthService } from '../../data/services/auth.service';
import { QuestionsService } from '../../data/services/questions.service';

@Component({
  selector: 'app-question-card',
  imports: [FormsModule],
  templateUrl: './question-card.component.html',
  styleUrl: './question-card.component.scss'
})
export class QuestionCardComponent implements OnInit {
  private questions = inject(QuestionsService)
  private auth = inject(AuthService)
  lessonId = input.required<number>()
  difficulty = signal('Easy')
  question = signal<NextQuestion | null>(null)
  answer = signal('')
  result = signal<AttemptResult | null>(null)
  missing = signal(false)
  levels = [
    { id: 'Easy', label: 'Легкий' },
    { id: 'Medium', label: 'Середній' },
    { id: 'Hard', label: 'Складний' }
  ]

  ngOnInit() {
    this.auth.me().subscribe(() => this.load())
  }

  chooseLevel(level: string) {
    this.difficulty.set(level)
    this.load()
  }

  addToken(token: string) {
    const current = this.answer().trim()
    this.answer.set(current.length === 0 ? `${token} ` : `${current} ${token} `)
  }

  submit() {
    const question = this.question()
    if (!question) {
      return
    }

    this.questions.submit(question.exerciseId, this.difficulty(), this.answer()).subscribe({
      next: result => this.result.set(result),
      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.missing.set(true)
        }
      }
    })
  }

  nextQuestion() {
    this.load()
  }

  private load() {
    this.answer.set('')
    this.result.set(null)
    this.missing.set(false)
    this.question.set(null)
    this.questions.next(this.lessonId(), this.difficulty()).subscribe({
      next: question => this.question.set(question),
      error: () => this.missing.set(true)
    })
  }
}
