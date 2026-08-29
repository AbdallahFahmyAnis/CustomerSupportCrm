import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageStore } from 'shared';
import { ARTICLE_KINDS, ARTICLE_STATUSES } from '../articles.models';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-021 — article list (locale from shell language). */
@Component({
  selector: 'app-article-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive, DatePipe],
  templateUrl: './article-list.html',
  styleUrls: ['./article-list.scss'],
})
export class ArticleListPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ArticlesStore);
  readonly kinds = ARTICLE_KINDS;
  readonly statuses = ARTICLE_STATUSES;

  q = '';
  readonly kindFilter = signal('');
  readonly statusFilter = signal('');

  readonly filtered = computed(() => {
    const kind = this.kindFilter();
    const status = this.statusFilter();
    return this.store.items().filter((a) => {
      if (kind && a.kind !== kind) return false;
      if (status && a.status !== status) return false;
      return true;
    });
  });

  readonly stats = computed(() => {
    const items = this.store.items();
    return {
      total: items.length,
      published: items.filter((a) => a.status === 'Published').length,
      draft: items.filter((a) => a.status === 'Draft').length,
      faqs: items.filter((a) => a.kind === 'Faq').length,
    };
  });

  constructor() {
    effect(() => {
      const locale = this.lang.lang();
      this.store.load(this.q, locale);
    });
  }

  ngOnInit(): void {
    this.store.load(this.q, this.lang.lang());
  }

  search(): void {
    this.store.load(this.q, this.lang.lang());
  }

  setKind(kind: string): void {
    this.kindFilter.set(kind);
  }

  setStatus(status: string): void {
    this.statusFilter.set(status);
  }

  excerpt(title: string): string {
    return title.length > 72 ? `${title.slice(0, 72)}…` : title;
  }
}
