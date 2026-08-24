import { Component, input } from '@angular/core';

/** Presentational priority badge — Feature-Based ui/. */
@Component({
  selector: 'app-ticket-priority-badge',
  standalone: true,
  templateUrl: './ticket-priority-badge.component.html',
  styleUrls: ['./ticket-priority-badge.component.scss'],
})
export class TicketPriorityBadgeComponent {
  readonly priority = input.required<string>();
  readonly hot = input(false);
}
