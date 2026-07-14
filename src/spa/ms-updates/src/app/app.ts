import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GlobalErrorHandler } from './core/services/error-handler.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('ms-updates');
  protected readonly errorHandler = inject(GlobalErrorHandler);
}
