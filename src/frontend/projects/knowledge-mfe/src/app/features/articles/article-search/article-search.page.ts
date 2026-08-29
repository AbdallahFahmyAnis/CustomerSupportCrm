import { Component, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageStore } from 'shared';
import { ARTICLE_KINDS, ARTICLE_STATUSES } from '../articles.models';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-022 — ranked knowledge search page. */
@Component({
  selector: 'app-article-search-page',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './article-search.html',
  styleUrls: ['./article-search.scss'],
})
export class ArticleSearchPage {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ArticlesStore);
  readonly kinds = ARTICLE_KINDS;
  readonly statuses = ARTICLE_STATUSES;
  readonly searched = signal(false);

  q = '';
  kind = '';
  status = '';
  publishedOnly = true;

  constructor() {
    effect(() => {
      const locale = this.lang.lang();
      if (!this.searched()) return;
      const query = this.q.trim();
      if (!query) return;
      this.store.rankedSearch({
        q: query,
        kind: this.kind || undefined,
        status: this.status || undefined,
        publishedOnly: this.publishedOnly,
        locale,
      });
    });
  }

  run(): void {
    const query = this.q.trim();
    if (!query) {
      return;
    }
    this.searched.set(true);
    this.store.rankedSearch({
      q: query,
      kind: this.kind || undefined,
      status: this.status || undefined,
      publishedOnly: this.publishedOnly,
      locale: this.lang.lang(),
    });
  }

  scoreWidth(score: number): string {
    const pct = Math.max(8, Math.min(100, Math.round(score * 12)));
    return `${pct}%`;
  }
}
