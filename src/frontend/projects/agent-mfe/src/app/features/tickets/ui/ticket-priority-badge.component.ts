import { Component, input } from '@angular/core';

/** Presentational priority badge — Feature-Based ui/. */
@Component({
  selector: 'app-ticket-priority-badge',
  standalone: true,
  template: `<span class="badge" [class.hot]="hot()">{{ priority() }}</span>`,
  styles: `
    .badge {
      display: inline-block;
      padding: 0.15rem 0.5rem;
      border-radius: 0.25rem;
      background: #e2e8f0;
      font-size: 0.8rem;
      font-weight: 600;
    }
    .badge.hot {
      background: #fecaca;
      color: #7f1d1d;
    }
  `,
})
export class TicketPriorityBadgeComponent {
  readonly priority = input.required<string>();
  readonly hot = input(false);
}
