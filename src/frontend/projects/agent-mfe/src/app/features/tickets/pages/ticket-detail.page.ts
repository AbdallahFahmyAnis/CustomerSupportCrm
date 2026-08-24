import { DatePipe } from '@angular/common';
import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TicketsApi } from '../data-access/tickets.api';
import { TicketsStore } from '../data-access/tickets.store';
import { TicketPriorityBadgeComponent } from '../ui/ticket-priority-badge.component';

/** Smart detail page — Feature-Based + Signals. */
@Component({
  selector: 'app-ticket-detail-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe, TicketPriorityBadgeComponent],
  template: `
    <section class="page">
      <p><a routerLink="/agent/tickets">← Tickets</a></p>

      @if (store.error()) {
        <p class="error">{{ store.error() }}</p>
      }

      @if (store.selected(); as t) {
        <header class="row">
          <div>
            <h1>{{ t.ticketNumber }} — {{ t.subject }}</h1>
            <p>
              <a [routerLink]="['/agent/customers', t.customerId]">{{ t.customerName }}</a>
              ·
              <app-ticket-priority-badge
                [priority]="t.priority"
                [hot]="t.priority === 'High' || t.priority === 'Urgent'"
              />
              · {{ t.status }}
              @if (t.isEscalated) {
                · escalated
              }
            </p>
          </div>
        </header>

        <p class="desc">{{ t.description || 'No description.' }}</p>

        <div class="panels">
          <form class="card" (ngSubmit)="saveClass()">
            <h2>Classification</h2>
            <label>
              Category
              <select name="category" [(ngModel)]="category">
                @for (c of store.options()?.categories ?? []; track c) {
                  <option [value]="c">{{ c }}</option>
                }
              </select>
            </label>
            <label>
              Priority
              <select name="priority" [(ngModel)]="priority">
                @for (p of store.options()?.priorities ?? []; track p) {
                  <option [value]="p">{{ p }}</option>
                }
              </select>
            </label>
            <button type="submit" class="btn">Save classification</button>
          </form>

          <form class="card" (ngSubmit)="saveAssign()">
            <h2>Assignment</h2>
            <label>
              Agent
              <select name="agentId" [(ngModel)]="agentId">
                <option value="">Unassigned</option>
                @for (a of store.options()?.agents ?? []; track a.id) {
                  <option [value]="a.id">{{ a.name }}</option>
                }
              </select>
            </label>
            <button type="submit" class="btn">Assign</button>
          </form>

          <form class="card" (ngSubmit)="saveStatus()">
            <h2>Status</h2>
            <label>
              New status
              <select name="status" [(ngModel)]="status">
                @for (s of store.options()?.statuses ?? []; track s) {
                  <option [value]="s">{{ s }}</option>
                }
              </select>
            </label>
            <button type="submit" class="btn">Change status</button>
          </form>

          <form class="card" (ngSubmit)="escalate()">
            <h2>Escalate</h2>
            <label>
              Assign to (optional)
              <select name="escalateTo" [(ngModel)]="escalateTo">
                <option value="">Keep / unassigned</option>
                @for (a of store.options()?.agents ?? []; track a.id) {
                  <option [value]="a.id">{{ a.name }}</option>
                }
              </select>
            </label>
            <button type="submit" class="btn danger">Escalate</button>
          </form>
        </div>

        <h2>History</h2>
        <ul class="history">
          @for (h of t.history; track h.id) {
            <li>
              <strong>{{ h.field }}</strong>
              {{ h.oldValue || '—' }} → {{ h.newValue || '—' }}
              <span class="meta">{{ h.changedBy }} · {{ h.changedAt | date: 'short' }}</span>
            </li>
          }
        </ul>
      }
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; }
    .row { margin-bottom: 1rem; }
    .desc { white-space: pre-wrap; background: #fff; padding: 0.85rem; border-radius: 0.5rem; }
    .panels { display: grid; gap: 1rem; grid-template-columns: repeat(auto-fit, minmax(14rem, 1fr)); margin: 1rem 0; }
    .card { background: #fff; border: 1px solid #e2e8f0; border-radius: 0.5rem; padding: 0.85rem; display: grid; gap: 0.55rem; }
    label { display: grid; gap: 0.25rem; font-size: 0.9rem; }
    select { padding: 0.4rem; }
    .btn { background: #2563eb; color: #fff; border: 0; border-radius: 0.375rem; padding: 0.45rem 0.75rem; width: fit-content; }
    .btn.danger { background: #b91c1c; }
    .history { list-style: none; padding: 0; }
    .history li { background: #fff; margin-bottom: 0.4rem; padding: 0.55rem 0.75rem; border-radius: 0.35rem; }
    .meta { display: block; color: #64748b; font-size: 0.85rem; }
    .error { color: #b91c1c; }
  `,
})
export class TicketDetailPage implements OnInit {
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly route = inject(ActivatedRoute);

  category = '';
  priority = '';
  agentId = '';
  status = '';
  escalateTo = '';
  private id = '';

  constructor() {
    effect(() => {
      const t = this.store.selected();
      if (!t || t.id !== this.id) return;
      this.category = t.category;
      this.priority = t.priority;
      this.agentId = t.assignedAgentId ?? '';
      this.status = t.status;
    });
  }

  ngOnInit(): void {
    this.store.loadOptions();
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.store.loadDetail(this.id);
  }

  saveClass(): void {
    this.api.updateClassification(this.id, this.category, this.priority).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Classification failed.'),
    });
  }

  saveAssign(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.agentId);
    this.api.assign(this.id, this.agentId || null, agent?.name ?? null).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Assign failed.'),
    });
  }

  saveStatus(): void {
    this.api.changeStatus(this.id, this.status).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Status change failed.'),
    });
  }

  escalate(): void {
    const agent = this.store.options()?.agents.find((a) => a.id === this.escalateTo);
    this.api.escalate(this.id, agent?.id, agent?.name).subscribe({
      next: () => this.store.refreshDetail(this.id),
      error: (err) => this.store.error.set(err?.error?.error ?? 'Escalate failed.'),
    });
  }
}
