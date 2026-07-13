import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  template: `   
      <section class="hero">
        <p class="eyebrow">404</p>
        <h2>Page not found</h2>
        <p class="hero-copy">
          The page you are looking for does not exist.
        </p>
        <a class="subscribe-button" routerLink="/">Back to home</a>
      </section>
  
  `,
  styles: ``,
})
export class NotFound {}
