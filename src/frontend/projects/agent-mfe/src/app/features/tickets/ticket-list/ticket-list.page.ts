import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataToolbarDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
  LanguageStore,
  SessionApi,
} from 'shared';
import { TicketPriorityBadgeComponent } from '../components/ticket-priority-badge/ticket-priority-badge.component';
import { TicketsStore } from '../tickets.store';

/** Smart page — Feature-Based + Signals. */
@Component({
  selector: 'app-ticket-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    TicketPriorityBadgeComponent,
    CrmDataViewComponent,
    CrmDataToolbarDirective,
    CrmDataCellDirective,
    CrmDataCardDirective,
  ],
  templateUrl: './ticket-list.html',
  styleUrls: ['./ticket-list.scss'],
})
export class TicketListPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(TicketsStore);
  private readonly session = inject(SessionApi);
  q = '';
  mine = false;
  viewMode: CrmDataViewMode = 'list';

  readonly columns = computed<CrmDataViewColumn[]>(() => [
    { key: 'ticketNumber', header: this.lang.t('ticketId') },
    { key: 'subject', header: this.lang.t('subject') },
    { key: 'customerName', header: this.lang.t('customer') },
    { key: 'priority', header: this.lang.t('priority') },
    { key: 'status', header: this.lang.t('status') },
    { key: 'assignedAgentName', header: this.lang.t('assignee') },
  ]);

  ngOnInit(): void {
    this.store.loadOptions();
    const me = this.session.session()?.id;
    this.mine = !!me;
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.assignedOnly.set(this.mine);
    const me = this.session.session()?.id;
    this.store.loadList(me);
  }
}
