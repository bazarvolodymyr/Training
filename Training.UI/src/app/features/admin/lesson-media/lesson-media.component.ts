import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';
import { UploadedMedia } from '../../../data/interfaces/uploaded-media.interface';
import { MediaService } from '../../../data/services/media.service';

@Component({
  selector: 'app-lesson-media',
  imports: [FormsModule, RouterLink],
  templateUrl: './lesson-media.component.html',
  styleUrl: './lesson-media.component.scss'
})
export class LessonMediaComponent implements OnInit {
  private route = inject(ActivatedRoute)
  private auth = inject(AuthService)
  private media = inject(MediaService)
  allowed = signal(false)
  lessonId = signal(0)
  alt = signal('')
  uploaded = signal<UploadedMedia | null>(null)
  error = signal<string | null>(null)
  inserted = signal(false)
  private file: File | null = null

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'))
    this.lessonId.set(id)
    this.auth.me().subscribe(user => {
      this.allowed.set(user.roles.includes('Manager') || user.roles.includes('Admin'))
    })
  }

  choose(event: Event) {
    const input = event.target as HTMLInputElement
    this.file = input.files?.[0] ?? null
    this.uploaded.set(null)
    this.inserted.set(false)
    this.error.set(null)
  }

  upload() {
    const file = this.file
    if (!file) {
      this.error.set('Оберіть файл.')
      return
    }

    this.error.set(null)
    this.media.upload(this.lessonId(), file).subscribe({
      next: uploaded => this.uploaded.set(uploaded),
      error: (error: HttpErrorResponse) => this.error.set(this.message(error.status))
    })
  }

  insert() {
    const uploaded = this.uploaded()
    if (!uploaded) {
      return
    }

    this.media.insert(this.lessonId(), uploaded.id, this.alt()).subscribe({
      next: () => this.inserted.set(true),
      error: () => this.error.set('Не вдалося вставити файл у статтю.')
    })
  }

  private message(status: number) {
    if (status === 409) {
      return 'Для цього уроку вже достатньо файлів цього типу.'
    }

    if (status === 413 || status === 400) {
      return 'Файл не підходить: дозволені JPEG, PNG, WebP до 2 МБ і MP3, M4A до 5 МБ.'
    }

    if (status === 403 || status === 401) {
      return 'Потрібна роль Manager або Admin.'
    }

    return 'Не вдалося завантажити файл.'
  }
}
