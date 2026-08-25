import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataToolbarDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
  CrmDateFieldComponent,
} from 'shared';
import { AuditStore } from '../audit.store';

/** SDD CRM-036 — audit list smart page. */
@Component({
  selector: 'app-audit-list-page',
  standalone: true,
  imports: [
    FormsModule,
    DatePipe,
    CrmDataViewComponent,
    CrmDataToolbarDirective,
    CrmDataCellDirective,
    CrmDataCardDirective,
    CrmDateFieldComponent,
  ],
  templateUrl: './audit-list.html',
  styleUrls: ['./audit-list.scss'],
})
export class AuditListPage implements OnInit {
  readonly store = inject(AuditStore);
  q = '';
  fromDate = '';
  toDate = '';
  viewMode: CrmDataViewMode = 'list';

  private readonly fromSig = signal('');
  private readonly toSig = signal('');

  readonly filteredEntries = computed(() => {
    const from = this.fromSig();
    const to = this.toSig();
    return this.store.entries().filter((e) => {
      const day = (e.occurredAt || '').slice(0, 10);
      if (from && day < from) {
        return false;
      }
      if (to && day > to) {
        return false;
      }
      return true;
    });
  });

  readonly columns: CrmDataViewColumn[] = [
    { key: 'occurredAt', header: 'When (UTC)' },
    { key: 'action', header: 'Action' },
    { key: 'actorEmail', header: 'Actor' },
    { key: 'targetEmail', header: 'Target' },
    { key: 'detail', header: 'Detail' },
    { key: 'success', header: 'Result' },
  ];

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.fromSig.set(this.fromDate);
    this.toSig.set(this.toDate);
    this.store.load();
  }
}
