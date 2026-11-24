import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ArticleComponent } from '../../../common-ui/article/article.component';
import { QuestionCardComponent } from '../../../common-ui/question-card/question-card.component';
import { LessonArticle } from '../../../data/interfaces/lesson-article.interface';
import { LessonsService } from '../../../data/services/lessons.service';

@Component({
  selector: 'app-lesson',
  imports: [RouterLink, ArticleComponent, QuestionCardComponent],
  templateUrl: './lesson.component.html',
  styleUrl: './lesson.component.scss'
})
export class LessonComponent implements OnInit {
  private route = inject(ActivatedRoute)
  private lessonsApi = inject(LessonsService)
  article = signal<LessonArticle | null>(null)
  missing = signal(false)

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'))
      this.article.set(null)
      this.missing.set(false)
      if (!Number.isInteger(id) || id < 1) {
        this.missing.set(true)
        return
      }

      this.lessonsApi.get(id).subscribe({
        next: article => this.article.set(article),
        error: () => this.missing.set(true)
      })
    })
  }
}
