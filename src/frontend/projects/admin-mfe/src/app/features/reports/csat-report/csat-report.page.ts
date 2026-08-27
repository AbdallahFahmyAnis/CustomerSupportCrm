import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageStore } from 'shared';
import { NgApexchartsModule } from 'ng-apexcharts';
import { barChart, radialScore } from '../report-charts';
import { ReportsStore } from '../reports.store';
import { isoEnd, isoStart } from '../ticket-reports/ticket-reports.page';

/** SDD CRM-033 */
@Component({
  selector: 'app-csat-report-page',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive, DatePipe, NgApexchartsModule],
  templateUrl: './csat-report.html',
  styleUrls: ['./csat-report.scss'],
})
export class CsatReportPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ReportsStore);
  from = '';
  to = '';

  readonly scoreRadial = computed(() => {
    const c = this.store.csat();
    return radialScore(c?.averageRating ?? 0, 5, this.lang.t('averageRating'));
  });

  readonly distributionChart = computed(() => {
    const c = this.store.csat();
    const dist = new Map((c?.distribution ?? []).map((d) => [d.rating, d.count]));
    const ratings = [1, 2, 3, 4, 5];
    return barChart(
      ratings.map((r) => `${r} ★`),
      ratings.map((r) => dist.get(r) ?? 0),
      { name: this.lang.t('responses'), color: '#ffb400' },
    );
  });

  readonly agentChart = computed(() => {
    const c = this.store.csat();
    const rows = [...(c?.byAgent ?? [])].sort((a, b) => b.averageRating - a.averageRating).slice(0, 8);
    return barChart(
      rows.map((a) => a.agentName || this.lang.t('unassigned')),
      rows.map((a) => a.averageRating),
      { horizontal: true, name: this.lang.t('averageRating'), color: '#56ca00' },
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
    this.store.loadCsat(isoStart(this.from), isoEnd(this.to));
  }
}
