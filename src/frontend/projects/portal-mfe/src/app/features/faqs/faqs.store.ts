import { Injectable, computed, inject, signal } from '@angular/core';
import { FaqsApi } from './faqs.api';
import { PortalFaqDetail, PortalFaqSummary } from './faqs.models';

/** SDD CRM-029 — portal FAQs feature store (signals). */
@Injectable({ providedIn: 'root' })
export class FaqsStore {
  private readonly api = inject(FaqsApi);

  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);
  private readonly _items = signal<PortalFaqSummary[]>([]);
  private readonly _query = signal('');

  private readonly _detailLoading = signal(false);
  private readonly _detailError = signal<string | null>(null);
  private readonly _detail = signal<PortalFaqDetail | null>(null);

  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly items = this._items.asReadonly();
  readonly query = this._query.asReadonly();
  readonly hasItems = computed(() => this._items().length > 0);

  readonly detailLoading = this._detailLoading.asReadonly();
  readonly detailError = this._detailError.asReadonly();
  readonly detail = this._detail.asReadonly();

  load(q = '', locale?: string): void {
    this._query.set(q);
    this._loading.set(true);
    this._error.set(null);
    this.api.list(q, locale).subscribe({
      next: (rows) => {
        this._items.set(rows);
        this._loading.set(false);
      },
      error: (err) => {
        this._error.set(this.readError(err));
        this._items.set([]);
        this._loading.set(false);
      },
    });
  }

  loadDetail(id: string): void {
    this._detailLoading.set(true);
    this._detailError.set(null);
    this._detail.set(null);
    this.api.get(id).subscribe({
      next: (row) => {
        this._detail.set(row);
        this._detailLoading.set(false);
      },
      error: (err) => {
        this._detailError.set(this.readError(err, 'FAQ not found.'));
        this._detailLoading.set(false);
      },
    });
  }

  private readError(err: unknown, fallback = 'Request failed.'): string {
    const e = err as { error?: { message?: string | string[]; error?: string }; message?: string; status?: number };
    if (e?.status === 404) {
      return 'FAQ not found.';
    }
    const msg = e?.error?.message ?? e?.error?.error ?? e?.message;
    if (Array.isArray(msg)) {
      return msg.join(' ');
    }
    return typeof msg === 'string' && msg.trim() ? msg : fallback;
  }
}
