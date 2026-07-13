import { Component, Input } from '@angular/core';
// pipes
import { DatePipe } from '@angular/common';
import { ProductIconPipe } from '../../../core/pipes/product-icon.pipe';
import { FirstParagraphPipe } from '../../../core/pipes/first-paragraph.pipe';
import { TitleBracketPipe } from '../../../core/pipes/title-bracket.pipe';

@Component({
  selector: 'app-update-list',
  imports: [ProductIconPipe, DatePipe, FirstParagraphPipe, TitleBracketPipe],
  template: ` 
        <div class="updates-grid">
          @for(item of data;track item.id) {
            <article class="update-card">
            <div class="card-top">
              <div class="product">
                <span class="product-icon"><img [src]="item.source | productIcon" alt=""></span>
                <span>{{ item.source }}</span>
              </div>
              @if (item.title | titleBracket; as badge) {
                <span class="badge">{{ badge }}</span>
              }
            </div>
            <h3>{{ item.title }}</h3>
            <p>{{ item.description | firstParagraph }}</p>
            <div class="card-footer">
              <time>{{ item.publishedAt | date: 'mediumDate' }}</time>
              <a class="card-link" href="{{ item.link }}" target="_blank" rel="noreferrer">
                Read update →
              </a>
            </div>
          </article>     
          }
 
        </div>

        <div class="empty-state" hidden>
          <h3>No updates found</h3>
          <p>Try another search term or select a different product.</p>
          <button id="clearFilters" type="button">Clear filters</button>
        </div>     
  `,
  styles: ``,
})
export class UpdateList {
    @Input({ required: true }) data: any = [];
}
