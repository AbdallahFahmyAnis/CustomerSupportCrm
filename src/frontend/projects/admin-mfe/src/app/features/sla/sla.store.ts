import { Injectable, inject, signal } from '@angular/core';
import { SlaApi } from './sla.api';
import { SlaPolicy } from './sla.models';

/** SDD CRM-017 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class SlaStore {
  private readonly api = inject(SlaApi);

  readonly policies = signal<SlaPolicy[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly saved = signal(false);

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.list().subscribe({
      next: (rows) => {
        this.policies.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load SLA policies.');
        this.loading.set(false);
      },
    });
  }

  save(priority: string, firstResponseMinutes: number, resolutionMinutes: number): void {
    this.saving.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.update(priority, firstResponseMinutes, resolutionMinutes).subscribe({
      next: (row) => {
        this.policies.update((list) => list.map((p) => (p.priority === row.priority ? row : p)));
        this.saving.set(false);
        this.saved.set(true);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Save failed.');
        this.saving.set(false);
      },
    });
  }
}
