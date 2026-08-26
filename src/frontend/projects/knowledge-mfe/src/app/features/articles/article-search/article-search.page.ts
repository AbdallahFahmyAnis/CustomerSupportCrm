import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { LanguageStore } from 'shared';
import { ARTICLE_KINDS, ARTICLE_STATUSES } from '../articles.models';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-022 — ranked knowledge search page. */
@Component({
  selector: 'app-article-search-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './article-search.html',
  styleUrls: ['./article-search.scss'],
})
export class ArticleSearchPage {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ArticlesStore);
  readonly kinds = ARTICLE_KINDS;
  readonly statuses = ARTICLE_STATUSES;

  q = 'password';
  kind = '';
  status = '';
  publishedOnly = true;

  run(): void {
    this.store.rankedSearch({
      q: this.q.trim(),
      kind: this.kind || undefined,
      status: this.status || undefined,
      publishedOnly: this.publishedOnly,
    });
  }
}
