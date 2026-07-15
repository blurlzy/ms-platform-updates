import { DOCUMENT, CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Loader } from '../core/services/loader.service';

@Component({
  selector: 'app-main-layout',
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: ` 
    @if(loader.isLoading | async){
      <div class="progress-bar" role="progressbar" aria-label="Loading updates"></div>
    }
    
    <header class="site-header">
      <a class="brand" routerLink="/" aria-label="Microsoft Cloud & AI Platform Updates home">
        <span class="brand-mark" aria-hidden="true">
          <span></span><span></span><span></span><span></span>
        </span>
        <span>Microsoft Cloud & AI Platform Updates</span>
      </a>

      <nav class="header-actions" aria-label="Page actions">
        <button
          class="icon-button"
          type="button"
          (click)="toggleTheme()"
        >
          <svg viewBox="0 0 24 24" aria-hidden="true">
            <path d="M12 3v2m0 14v2M3 12h2m14 0h2M5.64 5.64l1.42 1.42m9.88 9.88 1.42 1.42m0-12.72-1.42 1.42M7.06 16.94l-1.42 1.42M16 12a4 4 0 1 1-8 0 4 4 0 0 1 8 0Z" />
          </svg>
        </button>
        <a class="subscribe-button" href="https://zongyi.me" target="_blank" rel="noreferrer">About</a>
      </nav>
    </header>

    <main>
       <router-outlet></router-outlet> 
    </main>

    <footer>
      <span>zongyi.me 🚀 All rights reserved</span>
      <span>
        <a class="social-link" href="https://github.com/blurlzy/ms-platform-updates" target="_blank" rel="noreferrer">
          <img src="assets/icons/github.svg" alt="" />
        </a>
        <a class="social-link" href="https://www.linkedin.com/in/zongyi-li-88445515/" target="_blank" rel="noreferrer">
          <img src="assets/icons/linkedin.svg" alt="" />      
        </a>
      </span>  
      <a style="cursor: pointer;" (click)="backToTop()">Back to top ↑</a>
    </footer>
  `,
  styles: `
    .progress-bar {
      background: var(--cp-accent);
      height: 5px;
      left: 0;
      overflow: hidden;
      position: fixed;
      right: 0;
      top: 0;
      z-index: 1000;
    }

    .progress-bar::after {
      animation: loading-progress 1.3s ease-in-out infinite;
      background: var(--cp-accent-fg);
      content: '';
      height: 100%;
      left: 0;
      opacity: 0.55;
      position: absolute;
      transform: translateX(-100%);
      width: 45%;
    }

    @keyframes loading-progress {
      to {
        transform: translateX(325%);
      }
    }

    @media (prefers-reduced-motion: reduce) {
      .progress-bar::after {
        animation-duration: 2.5s;
      }
    }
  `,
})
export class MainLayout {
  public readonly loader = inject(Loader);
  private readonly document = inject(DOCUMENT);
  protected readonly isDarkTheme = signal(
    (this.document.defaultView?.localStorage.getItem('theme') ?? 'light') === 'dark',
  );

  constructor() {
    this.applyTheme(this.isDarkTheme());
  }

  protected toggleTheme(): void {
    const isDarkTheme = !this.isDarkTheme();

    this.isDarkTheme.set(isDarkTheme);
    this.applyTheme(isDarkTheme);
    this.document.defaultView?.localStorage.setItem('theme', isDarkTheme ? 'dark' : 'light');
  }

  private applyTheme(isDarkTheme: boolean): void {
    if (isDarkTheme) {
      this.document.documentElement.setAttribute('data-theme', 'dark');
    } else {
      this.document.documentElement.removeAttribute('data-theme');
    }
  }

  backToTop(): void {
    this.document.defaultView?.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
