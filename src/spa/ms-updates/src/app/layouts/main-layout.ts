import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink],
  template: ` 
    <header class="site-header">
      <a class="brand" href="#" aria-label="Microsoft Updates home">
        <span class="brand-mark" aria-hidden="true">
          <span></span><span></span><span></span><span></span>
        </span>
        <span>Microsoft Updates</span>
      </a>

      <nav class="header-actions" aria-label="Page actions">
        <button class="icon-button" id="themeToggle" type="button" aria-label="Toggle color theme">
          <svg viewBox="0 0 24 24" aria-hidden="true">
            <path d="M12 3v2m0 14v2M3 12h2m14 0h2M5.64 5.64l1.42 1.42m9.88 9.88 1.42 1.42m0-12.72-1.42 1.42M7.06 16.94l-1.42 1.42M16 12a4 4 0 1 1-8 0 4 4 0 0 1 8 0Z" />
          </svg>
        </button>
        <a class="subscribe-button" href="#newsletter">Get weekly digest</a>
      </nav>
    </header>

    <main>
       <router-outlet></router-outlet> 
    </main>

    <footer>
      <span>Microsoft Updates</span>
      <p>An independent product update dashboard concept.</p>
      <a href="#">Back to top ↑</a>
    </footer>
  `,
  styles: ``,
})
export class MainLayout {}
