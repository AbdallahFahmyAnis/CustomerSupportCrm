import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SessionApi } from 'shared';
import { TicketsApi } from '../../tickets/tickets.api';
import { TicketSummary, TicketTask } from '../../tickets/tickets.models';

/** SDD CRM-013 / CRM-014 — agent home with my tickets + due tasks. */
@Component({
  selector: 'app-agent-home-page',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class AgentHomePage implements OnInit {
  private readonly api = inject(TicketsApi);
  private readonly session = inject(SessionApi);

  readonly mine = signal<TicketSummary[]>([]);
  readonly dueTasks = signal<TicketTask[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');

  ngOnInit(): void {
    const me = this.session.session()?.id;
    if (!me) {
      this.error.set('Sign in to see your assigned tickets.');
      return;
    }
    this.loading.set(true);
    const endOfDay = new Date();
    endOfDay.setHours(23, 59, 59, 999);

    this.api.search('', me).subscribe({
      next: (rows) => {
        this.mine.set(rows.slice(0, 8));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load assigned tickets.');
        this.loading.set(false);
      },
    });

    this.api.listMyTasks(me, endOfDay.toISOString()).subscribe({
      next: (rows) => this.dueTasks.set(rows.slice(0, 8)),
      error: () => this.dueTasks.set([]),
    });
  }
}
