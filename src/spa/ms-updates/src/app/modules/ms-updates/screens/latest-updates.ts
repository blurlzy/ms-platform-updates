import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
// service
import { DataService } from '../ms-data.service';
// components
import { UpdateList } from '../components/update-list';
import { Pager } from '../components/pager';

@Component({
  selector: 'app-latest-updates',
  imports: [UpdateList, Pager],
  template: ` 
      <section class="hero">
        <p class="eyebrow">One feed. Every important update.</p>
        <!-- <h1>Stay current without<br />the noise.</h1> -->
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
  
      <section class="updates-section" aria-labelledby="updatesTitle">
        <div class="section-heading">
          <div>
            <p class="section-kicker">Latest updates</p>
            <h2>What's new</h2>
          </div>
          <!-- <label class="sort-control">
            <span>Sort</span>
            <select id="sortSelect">
              <option value="newest">Newest first</option>
              <option value="oldest">Oldest first</option>
              <option value="product">Product name</option>
            </select>
          </label> -->
        </div>

        <div class="filter-row" id="filterRow" aria-label="Filter by product">
          <button class="filter" [class.active]="filterFormGroup.value.source === ''" type="button" (click)="selectSourceFilter('')">All updates</button>
          <button class="filter" [class.active]="filterFormGroup.value.source === 'azure'" type="button" (click)="selectSourceFilter('azure')">Azure</button>
          <button class="filter" [class.active]="filterFormGroup.value.source === 'microsoft fabric'" type="button" (click)="selectSourceFilter('microsoft fabric')">Microsoft Fabric</button>
          <button class="filter" [class.active]="filterFormGroup.value.source === 'microsoft foundry'" type="button" (click)="selectSourceFilter('microsoft foundry')">Microsoft Foundry</button>
          <button class="filter" [class.active]="filterFormGroup.value.source === 'github'" type="button" (click)="selectSourceFilter('github')">GitHub</button>
          <button class="filter" [class.active]="filterFormGroup.value.source === 'microsoft copilot 365'" type="button" (click)="selectSourceFilter('microsoft copilot 365')">Microsoft Copilot 365</button>
         
        </div>

        <div class="results-meta" aria-live="polite">
          <!-- <span>Showing {{ pagedList().data.length }} of {{ pagedList().total }} updates</span>
          <span class="updated-time">Refreshed today</span> -->
        </div>

        <!-- update list component -->
        <app-update-list [data]="pagedList().data"></app-update-list>

        <app-pager
          [pageIndex]="filterFormGroup.value.pageIndex ?? 0"
          [pageSize]="filterFormGroup.value.pageSize ?? pageSize"
          [totalItems]="pagedList().total"
          (pageChange)="selectPage($event)"
        ></app-pager>
      </section>    
  `,
  styles: `

  `,
})
export class LatestUpdates {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dataService = inject(DataService);

  // default page size
  readonly pageSize = 12;
  // properties
  pagedList = signal<any>({ data: [], total: 0 });
  // filter form group
  filterFormGroup = new FormGroup({
    source: new FormControl(''),
    pageSize: new FormControl(this.pageSize),
    pageIndex: new FormControl(0)
  });

  ngOnInit() {
    // query params changes
    this.activatedRoute.queryParams.subscribe((params) => {
      const pageIndex = +params['pageIndex'];
      //const source = String(params['source'] ?? '');

      // retrive the query params
      this.filterFormGroup.patchValue({
        source: params['source'] ?? '',
        pageIndex: pageIndex ? pageIndex : 0,
      });

      // reset the result      			
      this.pagedList.set({ data: [], total: 0 });
      // ensure it scrolls to the top of the page
      // window.scroll(0, 160);
      // retrieve the updates based on the query params
      this.getUpdates(this.filterFormGroup.value.source ?? '', 
                      this.filterFormGroup.value.pageIndex ?? 0, 
                      this.filterFormGroup.value.pageSize ?? this.pageSize);  

    });
  }

  // select source filter
  selectSourceFilter(source: string) {
    // update the source the query string, which will trigger the query params changes event		
    this.router.navigate(['/'], {
      queryParams: {        
        source: source,
        pageIndex: 0, // reset page index
        pageSize: this.filterFormGroup.value.pageSize ?? this.pageSize
      }
    });
    
  }

  selectPage(newPageIndex: number) {
    // ensure it scrolls to the top of the page
    window.scroll(0, 0);
    // update the page index in the query string, which will trigger the query params changes event
    this.router.navigate(['/'], {
      queryParams: {
        source: this.filterFormGroup.value.source ?? '',
        pageIndex: newPageIndex,
        pageSize: this.filterFormGroup.value.pageSize ?? this.pageSize
      }
    });

  }

  private getUpdates(source: string, pageIndex: number, pageSize: number) { 
    this.dataService.getAllUpdates(source, pageIndex, pageSize).subscribe((response) => {
      this.pagedList.set(response);
      //console.log('Updates retrieved:', response);
    });
  }
}

