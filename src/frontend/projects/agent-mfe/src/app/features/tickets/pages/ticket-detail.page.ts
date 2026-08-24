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
  templateUrl: './ticket-detail.page.html',
  styleUrls: ['./ticket-detail.page.scss'],
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
