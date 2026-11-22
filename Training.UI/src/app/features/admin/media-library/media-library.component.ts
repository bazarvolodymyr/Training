import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';
import { LessonSummary } from '../../../data/interfaces/lesson-summary.interface';
import { LessonsService } from '../../../data/services/lessons.service';

@Component({
  selector: 'app-media-library',
  imports: [RouterLink],
  templateUrl: './media-library.component.html',
  styleUrl: './media-library.component.scss'
})
export class MediaLibraryComponent implements OnInit {
  private auth = inject(AuthService)
  private lessonsApi = inject(LessonsService)
  allowed = signal(false)
  lessons = signal<LessonSummary[]>([])

  ngOnInit() {
    this.auth.me().subscribe(user => {
      const allowed = user.roles.includes('Manager') || user.roles.includes('Admin')
      this.allowed.set(allowed)
      if (allowed) {
        this.lessonsApi.list().subscribe(lessons => this.lessons.set(lessons))
      }
    })
  }
}
