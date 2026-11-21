import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthBarComponent } from "./features/auth/auth-bar/auth-bar.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AuthBarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Training.UI';

}
