import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-pager',
    template: `
        <nav class="pager" aria-label="Updates pagination">
          @if (pageIndex > 0) {
            <button class="pager-arrow" type="button" (click)="pageChange.emit(pageIndex - 1)">
              <span aria-hidden="true">←</span> Prev
            </button>
          }
          @if ((pageIndex + 1) * pageSize < totalItems) {
            <button class="pager-arrow" type="button" (click)="pageChange.emit(pageIndex + 1)">
              Next <span aria-hidden="true">→</span>
            </button>
          }
        </nav>
  `,
    styles: `
    .pager {
        align-items: center;
        display: flex;
        gap: 10px;
        justify-content: center;
        padding-top: 36px;
    }

    .pager button {
        align-items: center;
        background: var(--cp-surface);
        border: 1px solid var(--cp-border);
        border-radius: 8px;
        color: var(--cp-text-muted);
        cursor: pointer;
        display: flex;
        font-size: 13px;
        font-weight: 700;
        height: 38px;
        justify-content: center;
        min-width: 38px;
        padding: 0 10px;
    }

    .pager button:hover:not(:disabled) {
        border-color: var(--cp-border-strong);
        color: var(--cp-text);
    }

    .pager button:disabled {
        cursor: default;
        opacity: 0.38;
    }

    .pager-arrow span {
        font-size: 17px;
        line-height: 1;
    }
  `,
})
export class Pager {
  @Input({ required: true }) pageIndex = 0;
  @Input({ required: true }) pageSize = 12;
  @Input({ required: true }) totalItems = 0;
  @Output() pageChange = new EventEmitter<number>();
}