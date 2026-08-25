import { Injectable, computed, inject, signal } from '@angular/core';
import { RequestsApi } from './requests.api';
import {
  PortalRequestSummary,
  SubmitRequestBody,
  SubmitRequestResult,
} from './requests.models';

/** SDD CRM-027 / CRM-028 — portal requests feature store (signals). */
@Injectable({ providedIn: 'root' })
export class RequestsStore {
  private readonly api = inject(RequestsApi);

  private readonly _submitting = signal(false);
  private readonly _submitError = signal<string | null>(null);
  private readonly _lastResult = signal<SubmitRequestResult | null>(null);

  private readonly _tracking = signal(false);
  private readonly _trackError = signal<string | null>(null);
  private readonly _requests = signal<PortalRequestSummary[]>([]);
  private readonly _trackEmail = signal('');

  readonly submitting = this._submitting.asReadonly();
  readonly submitError = this._submitError.asReadonly();
  readonly lastResult = this._lastResult.asReadonly();

  readonly tracking = this._tracking.asReadonly();
  readonly trackError = this._trackError.asReadonly();
  readonly requests = this._requests.asReadonly();
  readonly trackEmail = this._trackEmail.asReadonly();
  readonly hasRequests = computed(() => this._requests().length > 0);

  submit(body: SubmitRequestBody): void {
    this._submitting.set(true);
    this._submitError.set(null);
    this._lastResult.set(null);
    this.api.submit(body).subscribe({
      next: (result) => {
        this._lastResult.set(result);
        this._submitting.set(false);
      },
      error: (err) => {
        this._submitError.set(this.readError(err));
        this._submitting.set(false);
      },
    });
  }

  clearLastResult(): void {
    this._lastResult.set(null);
    this._submitError.set(null);
  }

  track(email: string): void {
    this._trackEmail.set(email);
    this._tracking.set(true);
    this._trackError.set(null);
    this.api.track(email).subscribe({
      next: (rows) => {
        this._requests.set(rows);
        this._tracking.set(false);
      },
      error: (err) => {
        this._trackError.set(this.readError(err));
        this._requests.set([]);
        this._tracking.set(false);
      },
    });
  }

  private readError(err: unknown): string {
    const e = err as { error?: { message?: string | string[]; error?: string }; message?: string };
    const msg = e?.error?.message ?? e?.error?.error ?? e?.message;
    if (Array.isArray(msg)) {
      return msg.join(' ');
    }
    return typeof msg === 'string' && msg.trim() ? msg : 'Request failed.';
  }
}
