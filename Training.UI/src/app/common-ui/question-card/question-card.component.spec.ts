import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionCardComponent } from './question-card.component';

describe('QuestionCardComponent', () => {
  let component: QuestionCardComponent;
  let fixture: ComponentFixture<QuestionCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionCardComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuestionCardComponent);
    fixture.componentRef.setInput('lessonId', 1);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
