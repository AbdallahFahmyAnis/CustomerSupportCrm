import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SessionApi } from 'shared';
import { TicketsApi } from '../../tickets/tickets.api';
import { TicketSummary } from '../../tickets/tickets.models';

/** SDD CRM-013 — agent home with my assigned tickets. */
@Component({
  selector: 'app-agent-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class AgentHomePage implements OnInit {
  private readonly api = inject(TicketsApi);
  private readonly session = inject(SessionApi);

  readonly mine = signal<TicketSummary[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');

  ngOnInit(): void {
    const me = this.session.session()?.id;
    if (!me) {
      this.error.set('Sign in to see your assigned tickets.');
      return;
    }
    this.loading.set(true);
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
  }
}
