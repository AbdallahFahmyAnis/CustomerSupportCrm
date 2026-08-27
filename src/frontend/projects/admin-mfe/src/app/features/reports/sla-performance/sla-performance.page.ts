import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageStore } from 'shared';
import { NgApexchartsModule } from 'ng-apexcharts';
import { donutChart, emptyDonut, groupedBarChart, radialPercent } from '../report-charts';
import { ReportsStore } from '../reports.store';
import { isoEnd, isoStart } from '../ticket-reports/ticket-reports.page';

/** SDD CRM-032 */
@Component({
  selector: 'app-sla-performance-page',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive, DatePipe, NgApexchartsModule],
  templateUrl: './sla-performance.html',
  styleUrls: ['./sla-performance.scss'],
})
export class SlaPerformancePage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ReportsStore);
  from = '';
  to = '';

  readonly breachRadial = computed(() => {
    const s = this.store.sla();
    return radialPercent(
      s?.breachPercent ?? 0,
      this.lang.t('breachPercent'),
      (s?.breachPercent ?? 0) > 20 ? '#ff4c51' : '#56ca00',
    );
  });

  readonly outcomeDonut = computed(() => {
    const s = this.store.sla();
    if (!s || s.ticketCount <= 0) return emptyDonut(this.lang.t('noChartData'));
    const ok = Math.max(0, s.ticketCount - s.resolutionBreached);
    return donutChart(
      [ok, s.resolutionBreached],
      [this.lang.t('withinSla'), this.lang.t('breached')],
      ['#56ca00', '#ff4c51'],
    );
  });

  readonly agentChart = computed(() => {
    const s = this.store.sla();
    const rows = [...(s?.byAgent ?? [])].sort((a, b) => b.ticketCount - a.ticketCount).slice(0, 8);
    return groupedBarChart(
      rows.map((a) => a.agentName || this.lang.t('unassigned')),
      [
        {
          name: this.lang.t('tickets'),
          data: rows.map((a) => a.ticketCount),
          color: '#8c57ff',
        },
        {
          name: this.lang.t('breached'),
          data: rows.map((a) => a.resolutionBreached),
          color: '#ff4c51',
        },
      ],
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
    this.store.loadSla(isoStart(this.from), isoEnd(this.to));
  }
}
