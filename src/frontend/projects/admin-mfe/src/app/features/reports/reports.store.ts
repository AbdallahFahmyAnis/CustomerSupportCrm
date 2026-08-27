import { Injectable, inject, signal } from '@angular/core';
import { ReportsApi } from './reports.api';
import { CsatReport, SlaPerformanceReport, TicketReportSummary } from './reports.models';

/** SDD CRM-031…034 */
@Injectable({ providedIn: 'root' })
export class ReportsStore {
  private readonly api = inject(ReportsApi);

  readonly loading = signal(false);
  readonly error = signal('');
  readonly summary = signal<TicketReportSummary | null>(null);
  readonly sla = signal<SlaPerformanceReport | null>(null);
  readonly csat = signal<CsatReport | null>(null);

  loadSummary(from?: string, to?: string): void {
    this.loading.set(true);
    this.error.set('');
    this.api.summary(from, to).subscribe({
      next: (row) => {
        this.summary.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load ticket report.');
        this.loading.set(false);
      },
    });
  }

  loadSla(from?: string, to?: string): void {
    this.loading.set(true);
    this.error.set('');
    this.api.slaPerformance(from, to).subscribe({
      next: (row) => {
        this.sla.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load SLA performance.');
        this.loading.set(false);
      },
    });
  }

  loadCsat(from?: string, to?: string): void {
    this.loading.set(true);
    this.error.set('');
    this.api.csat(from, to).subscribe({
      next: (row) => {
        this.csat.set(row);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load CSAT report.');
        this.loading.set(false);
      },
    });
  }

  /** SDD CRM-034 — load KPI sources in parallel. */
  loadDashboard(from?: string, to?: string): void {
    this.loading.set(true);
    this.error.set('');
    let pending = 3;
    const done = () => {
      pending -= 1;
      if (pending <= 0) this.loading.set(false);
    };
    this.api.summary(from, to).subscribe({
      next: (row) => {
        this.summary.set(row);
        done();
      },
      error: () => {
        this.error.set('Could not load dashboard KPIs.');
        done();
      },
    });
    this.api.slaPerformance(from, to).subscribe({
      next: (row) => {
        this.sla.set(row);
        done();
      },
      error: () => done(),
    });
    this.api.csat(from, to).subscribe({
      next: (row) => {
        this.csat.set(row);
        done();
      },
      error: () => done(),
    });
  }
}
