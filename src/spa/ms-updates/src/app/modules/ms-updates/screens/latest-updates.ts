import { Component } from '@angular/core';

// components
import { UpdateList } from '../components/update-list';

@Component({
  selector: 'app-latest-updates',
  imports: [UpdateList],
  template: ` 
      <section class="hero">
        <p class="eyebrow">One feed. Every important update.</p>
        <h1>Stay current without<br />the noise.</h1>
        <p class="hero-copy">
          The latest updates from the Microsoft Cloud & AI ecosystem.
        </p>

        <!-- <div class="search-wrap">
          <svg viewBox="0 0 24 24" aria-hidden="true">
            <circle cx="11" cy="11" r="7"></circle>
            <path d="m20 20-4-4"></path>
          </svg>
          <label class="sr-only" for="searchInput">Search updates</label>
          <input id="searchInput" type="search" placeholder="Search updates, products, or topics..." />
          <kbd>/</kbd>
        </div> -->
      </section>
  
      <app-update-list></app-update-list>
  `,
  styles: ``,
})
export class LatestUpdates {}
