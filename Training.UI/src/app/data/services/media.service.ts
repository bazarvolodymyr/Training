import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { UploadedMedia } from '../interfaces/uploaded-media.interface';

@Injectable({
  providedIn: 'root'
})
export class MediaService {
  private http = inject(HttpClient)
  private baseApiUrl = 'http://localhost:5166/'

  upload(lessonId: number, file: File) {
    const body = new FormData()
    body.append('file', file)
    return this.http.post<UploadedMedia>(
      `${this.baseApiUrl}api/v1/admin/lessons/${lessonId}/media`,
      body,
      { withCredentials: true }
    )
  }

  insert(lessonId: number, mediaId: number, alt: string) {
    return this.http.post<void>(
      `${this.baseApiUrl}api/v1/admin/lessons/${lessonId}/media/${mediaId}/insert`,
      { alt },
      { withCredentials: true }
    )
  }
}
