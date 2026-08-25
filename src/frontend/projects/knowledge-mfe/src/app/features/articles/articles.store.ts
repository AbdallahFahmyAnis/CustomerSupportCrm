import { Injectable, inject, signal } from '@angular/core';
import { ArticlesApi } from './articles.api';
import { ArticleDetail, ArticleSummary } from './articles.models';

/** SDD CRM-021 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class ArticlesStore {
  private readonly api = inject(ArticlesApi);

  readonly items = signal<ArticleSummary[]>([]);
  readonly selected = signal<ArticleDetail | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');

  load(q = ''): void {
    this.loading.set(true);
    this.error.set('');
    this.api.search(q).subscribe({
      next: (rows) => {
        this.items.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load articles.');
        this.loading.set(false);
      },
    });
  }

  loadDetail(id: string): void {
    this.loading.set(true);
    this.error.set('');
    this.api.get(id).subscribe({
      next: (row) => {
        this.selected.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Article not found.');
        this.loading.set(false);
      },
    });
  }

  create(body: { title: string; body: string; kind: string; status: string }, onDone: (id: string) => void): void {
    this.saving.set(true);
    this.error.set('');
    this.api.create(body).subscribe({
      next: (row) => {
        this.saving.set(false);
        onDone(row.id);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Create failed.');
        this.saving.set(false);
      },
    });
  }

  update(
    id: string,
    body: { title: string; body: string; kind: string; status: string },
    onDone: () => void,
  ): void {
    this.saving.set(true);
    this.error.set('');
    this.api.update(id, body).subscribe({
      next: (row) => {
        this.selected.set(row);
        this.saving.set(false);
        onDone();
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Save failed.');
        this.saving.set(false);
      },
    });
  }
}
