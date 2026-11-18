import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { QuestionCardComponent } from "./common-ui/question-card/question-card.component";
import { AuthBarComponent } from "./features/auth/auth-bar/auth-bar.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, QuestionCardComponent, AuthBarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Training.UI';

}
