import { Injectable, inject, signal } from '@angular/core';
import { SlaApi } from './sla.api';
import { AutoAssignRule, EscalationSettings, SlaPolicy } from './sla.models';

/** SDD CRM-017 / CRM-018 / CRM-019 — Feature-Based + Signals store. */
@Injectable({ providedIn: 'root' })
export class SlaStore {
  private readonly api = inject(SlaApi);

  readonly policies = signal<SlaPolicy[]>([]);
  readonly assignRules = signal<AutoAssignRule[]>([]);
  readonly escalation = signal<EscalationSettings | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly saved = signal(false);

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.saved.set(false);
    let pending = 3;
    const done = () => {
      pending -= 1;
      if (pending <= 0) {
        this.loading.set(false);
      }
    };
    this.api.list().subscribe({
      next: (rows) => {
        this.policies.set(rows);
        done();
      },
      error: () => {
        this.error.set('Could not load SLA policies.');
        done();
      },
    });
    this.api.listAssignRules().subscribe({
      next: (rows) => {
        this.assignRules.set(rows);
        done();
      },
      error: () => {
        this.error.set('Could not load assign rules.');
        done();
      },
    });
    this.api.getEscalationSettings().subscribe({
      next: (row) => {
        this.escalation.set(row);
        done();
      },
      error: () => {
        this.error.set('Could not load escalation settings.');
        done();
      },
    });
  }

  savePolicy(priority: string, firstResponseMinutes: number, resolutionMinutes: number): void {
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

  saveAssignRules(rules: AutoAssignRule[]): void {
    this.saving.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.replaceAssignRules(rules).subscribe({
      next: (rows) => {
        this.assignRules.set(rows);
        this.saving.set(false);
        this.saved.set(true);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Assign rules save failed.');
        this.saving.set(false);
      },
    });
  }

  saveEscalation(body: Omit<EscalationSettings, 'updatedAt'>): void {
    this.saving.set(true);
    this.error.set('');
    this.saved.set(false);
    this.api.updateEscalationSettings(body).subscribe({
      next: (row) => {
        this.escalation.set(row);
        this.saving.set(false);
        this.saved.set(true);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Escalation save failed.');
        this.saving.set(false);
      },
    });
  }
}
