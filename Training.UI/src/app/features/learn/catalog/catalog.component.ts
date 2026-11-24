import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LessonSummary } from '../../../data/interfaces/lesson-summary.interface';
import { LessonsService } from '../../../data/services/lessons.service';

@Component({
  selector: 'app-catalog',
  imports: [RouterLink],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss'
})
export class CatalogComponent implements OnInit {
  private lessonsApi = inject(LessonsService)
  lessons = signal<LessonSummary[]>([])

  ngOnInit() {
    this.lessonsApi.list().subscribe(lessons => this.lessons.set(lessons))
  }
}
