import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ReportsStore } from '../reports.store';
import { isoEnd, isoStart } from '../ticket-reports/ticket-reports.page';

/** SDD CRM-033 */
@Component({
  selector: 'app-csat-report-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './csat-report.html',
  styleUrls: ['./csat-report.scss'],
})
export class CsatReportPage implements OnInit {
  readonly store = inject(ReportsStore);
  from = '';
  to = '';

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
