import { Component } from '@angular/core';
import { ProductIconPipe } from '../../../core/pipes/product-icon.pipe';

@Component({
  selector: 'app-update-list',
  imports: [ProductIconPipe],
  template: ` 
      <section class="updates-section" aria-labelledby="updatesTitle">
        <div class="section-heading">
          <div>
            <p class="section-kicker">Latest updates</p>
            <h2 id="updatesTitle">What's new</h2>
          </div>
          <label class="sort-control">
            <span>Sort</span>
            <select id="sortSelect">
              <option value="newest">Newest first</option>
              <option value="oldest">Oldest first</option>
              <option value="product">Product name</option>
            </select>
          </label>
        </div>

        <div class="filter-row" id="filterRow" aria-label="Filter by product">
          <button class="filter active" type="button" data-filter="All">All updates</button>
          <button class="filter" type="button" data-filter="Azure">Azure</button>
          <button class="filter" type="button" data-filter="Foundry">Microsoft Foundry</button>
          <button class="filter" type="button" data-filter="GitHub">GitHub</button>
          <button class="filter" type="button" data-filter="Fabric">Fabric</button>
          <button class="filter" type="button" data-filter="Microsoft 365">Microsoft 365</button>
        </div>

        <div class="results-meta" aria-live="polite">
          <span id="resultCount">Showing 0 updates</span>
          <span class="updated-time">Refreshed today</span>
        </div>

        <div class="updates-grid" id="updatesGrid">
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img [src]="'Azure' | productIcon" alt=""></span>
              <span>Azure</span>
            </div>
            <span class="badge ">Generally available</span>
          </div>
          <h3>Azure Container Apps dynamic sessions</h3>
          <p>Run secure, isolated workloads on demand with fast startup, automatic cleanup, and built-in session management.</p>
          <div class="card-footer">
            <time datetime="2026-07-11">Jul 11, 2026</time>
            <a class="card-link" href="https://azure.microsoft.com/updates" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
      
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img [src]="'Microsoft Foundry' | productIcon" alt=""></span>
              <span>Microsoft Foundry</span>
            </div>
            <span class="badge preview">Preview</span>
          </div>
          <h3>Expanded model choice in Microsoft Foundry</h3>
          <p>New reasoning and multimodal models are available in the model catalog with simplified deployment and evaluation.</p>
          <div class="card-footer">
            <time datetime="2026-07-10">Jul 10, 2026</time>
            <a class="card-link" href="https://learn.microsoft.com/azure/ai-foundry/whats-new" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
      
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img class="github-icon" [src]="'GitHub' | productIcon" alt=""></span>
              <span>GitHub</span>
            </div>
            <span class="badge ">New</span>
          </div>
          <h3>Copilot coding agent improvements</h3>
          <p>Delegate more development tasks with richer repository context, clearer progress updates, and improved pull requests.</p>
          <div class="card-footer">
            <time datetime="2026-07-09">Jul 9, 2026</time>
            <a class="card-link" href="https://github.blog/changelog/" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
      
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img [src]="'Fabric' | productIcon" alt=""></span>
              <span>Fabric</span>
            </div>
            <span class="badge ">Generally available</span>
          </div>
          <h3>Mirroring for additional data sources</h3>
          <p>Bring operational data into OneLake with continuous replication and a streamlined, low-maintenance experience.</p>
          <div class="card-footer">
            <time datetime="2026-07-08">Jul 8, 2026</time>
            <a class="card-link" href="https://learn.microsoft.com/fabric/get-started/whats-new" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
       
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img [src]="'Azure' | productIcon" alt=""></span>
              <span>Azure</span>
            </div>
            <span class="badge preview">Preview</span>
          </div>
          <h3>Serverless GPU support for AI workloads</h3>
          <p>Access elastic GPU capacity without managing infrastructure, designed for inference and bursty AI applications.</p>
          <div class="card-footer">
            <time datetime="2026-07-04">Jul 4, 2026</time>
            <a class="card-link" href="https://azure.microsoft.com/updates" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
      
        <article class="update-card">
          <div class="card-top">
            <div class="product">
              <span class="product-icon"><img [src]="'Microsoft Foundry' | productIcon" alt=""></span>
              <span>Microsoft Foundry</span>
            </div>
            <span class="badge ">New</span>
          </div>
          <h3>Continuous evaluation for production agents</h3>
          <p>Monitor quality, safety, and performance over time using production traces and configurable evaluators.</p>
          <div class="card-footer">
            <time datetime="2026-07-02">Jul 2, 2026</time>
            <a class="card-link" href="https://learn.microsoft.com/azure/ai-foundry/whats-new" target="_blank" rel="noreferrer">
              Read update →
            </a>
          </div>
        </article>
      

      </div>

        <div class="empty-state" id="emptyState" hidden>
          <h3>No updates found</h3>
          <p>Try another search term or select a different product.</p>
          <button id="clearFilters" type="button">Clear filters</button>
        </div>
      </section>
  
      
  `,
  styles: ``,
})
export class UpdateList {}
