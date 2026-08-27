import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ReportsStore } from '../reports.store';

/** SDD CRM-031 — ticket volume report. */
@Component({
  selector: 'app-ticket-reports-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './ticket-reports.html',
  styleUrls: ['./ticket-reports.scss'],
})
export class TicketReportsPage implements OnInit {
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
    this.store.loadSummary(isoStart(this.from), isoEnd(this.to));
  }
}

export function isoStart(d: string): string | undefined {
  return d ? new Date(d + 'T00:00:00.000Z').toISOString() : undefined;
}

export function isoEnd(d: string): string | undefined {
  return d ? new Date(d + 'T23:59:59.999Z').toISOString() : undefined;
}
