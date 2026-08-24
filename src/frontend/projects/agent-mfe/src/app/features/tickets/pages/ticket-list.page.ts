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
  templateUrl: './ticket-list.page.html',
  styleUrls: ['./ticket-list.page.scss'],
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
