import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SessionApi } from 'shared';
import { TicketsStore } from '../data-access/tickets.store';
import { TicketPriorityBadgeComponent } from '../ui/ticket-priority-badge.component';

/** Smart page — Feature-Based + Signals. */
@Component({
  selector: 'app-ticket-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, TicketPriorityBadgeComponent],
  template: `
    <section class="page">
      <header class="row">
        <div>
          <h1>Tickets</h1>
          <p>{{ store.urgentCount() }} high/urgent in view</p>
        </div>
        <a routerLink="/agent/tickets/new" class="btn">Create ticket</a>
      </header>

      <form class="row" (ngSubmit)="search()">
        <input name="q" [(ngModel)]="q" placeholder="Search id, customer, subject" />
        <label class="check">
          <input type="checkbox" name="mine" [(ngModel)]="mine" (change)="search()" />
          Assigned to me
        </label>
        <button type="submit" class="btn secondary">Search</button>
      </form>

      @if (store.error()) {
        <p class="error">{{ store.error() }}</p>
      }

      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Subject</th>
            <th>Customer</th>
            <th>Priority</th>
            <th>Status</th>
            <th>Assignee</th>
          </tr>
        </thead>
        <tbody>
          @for (t of store.tickets(); track t.id) {
            <tr [class.hot-row]="t.priority === 'High' || t.priority === 'Urgent'">
              <td><a [routerLink]="['/agent/tickets', t.id]">{{ t.ticketNumber }}</a></td>
              <td>{{ t.subject }}</td>
              <td>
                <a [routerLink]="['/agent/customers', t.customerId]">{{ t.customerName }}</a>
              </td>
              <td>
                <app-ticket-priority-badge
                  [priority]="t.priority"
                  [hot]="t.priority === 'High' || t.priority === 'Urgent'"
                />
              </td>
              <td>{{ t.status }}@if (t.isEscalated) { · escalated }</td>
              <td>{{ t.assignedAgentName || 'Unassigned' }}</td>
            </tr>
          }
        </tbody>
      </table>
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; }
    .row { display: flex; flex-wrap: wrap; gap: 0.75rem; align-items: center; margin-bottom: 1rem; }
    input[type='text'], input:not([type]) { flex: 1; min-width: 12rem; padding: 0.45rem 0.6rem; }
    table { width: 100%; border-collapse: collapse; background: #fff; }
    th, td { text-align: start; padding: 0.6rem; border-bottom: 1px solid #e2e8f0; }
    .hot-row { background: #fff1f2; }
    .btn { background: #2563eb; color: #fff; text-decoration: none; border: 0; border-radius: 0.375rem; padding: 0.45rem 0.8rem; }
    .btn.secondary { background: #334155; }
    .check { display: flex; gap: 0.35rem; align-items: center; }
    .error { color: #b91c1c; }
  `,
})
export class TicketListPage implements OnInit {
  readonly store = inject(TicketsStore);
  private readonly session = inject(SessionApi);
  q = '';
  mine = false;

  ngOnInit(): void {
    this.store.loadOptions();
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.assignedOnly.set(this.mine);
    const me = this.session.session()?.id;
    this.store.loadList(me);
  }
}
