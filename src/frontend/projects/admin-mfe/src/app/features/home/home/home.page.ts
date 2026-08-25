import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReportsStore } from '../../reports/reports.store';

/** SDD CRM-035 / CRM-034 — admin home with management KPIs. */
@Component({
  selector: 'app-admin-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class AdminHomePage implements OnInit {
  readonly reports = inject(ReportsStore);

  ngOnInit(): void {
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 30);
    this.reports.loadDashboard(start.toISOString(), end.toISOString());
  }
}
