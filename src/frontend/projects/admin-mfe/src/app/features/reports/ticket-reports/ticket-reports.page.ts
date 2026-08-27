import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageStore } from 'shared';
import { NgApexchartsModule } from 'ng-apexcharts';
import { barChart, donutChart, emptyDonut } from '../report-charts';
import { ReportsStore } from '../reports.store';

/** SDD CRM-031 — ticket volume report. */
@Component({
  selector: 'app-ticket-reports-page',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive, DatePipe, NgApexchartsModule],
  templateUrl: './ticket-reports.html',
  styleUrls: ['./ticket-reports.scss'],
})
export class TicketReportsPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ReportsStore);
  from = '';
  to = '';

  readonly statusChart = computed(() => {
    const s = this.store.summary();
    if (!s?.byStatus?.length) return emptyDonut(this.lang.t('noChartData'));
    return donutChart(
      s.byStatus.map((b) => b.count),
      s.byStatus.map((b) => b.key),
    );
  });

  readonly categoryChart = computed(() => {
    const s = this.store.summary();
    const rows = s?.byCategory ?? [];
    return barChart(
      rows.map((b) => b.key),
      rows.map((b) => b.count),
      { name: this.lang.t('tickets') },
    );
  });

  readonly priorityChart = computed(() => {
    const s = this.store.summary();
    const rows = s?.byPriority ?? [];
    return barChart(
      rows.map((b) => b.key),
      rows.map((b) => b.count),
      { name: this.lang.t('tickets'), color: '#16b1ff' },
    );
  });

  readonly agentChart = computed(() => {
    const s = this.store.summary();
    const rows = [...(s?.byAgent ?? [])].sort((a, b) => b.count - a.count).slice(0, 8);
    return barChart(
      rows.map((b) => b.agentName || this.lang.t('unassigned')),
      rows.map((b) => b.count),
      { horizontal: true, name: this.lang.t('tickets'), color: '#8c57ff' },
    );
  });

  ngOnInit(): void {
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 30);
    this.from = start.toISOString().slice(0, 10);
    this.to = end.toISOString().slice(0, 10);
    this.reload();
  }

  reload(): void {
    this.store.loadSummary(isoStart(this.from), isoEnd(this.to));
  }
}

export function isoStart(d: string): string | undefined {
  return d ? new Date(d + 'T00:00:00.000Z').toISOString() : undefined;
}

export function isoEnd(d: string): string | undefined {
  return d ? new Date(d + 'T23:59:59.999Z').toISOString() : undefined;
}
