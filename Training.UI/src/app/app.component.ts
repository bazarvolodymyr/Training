import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { QuestionCardComponent } from "./common-ui/question-card/question-card.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, QuestionCardComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Training.UI';

}
