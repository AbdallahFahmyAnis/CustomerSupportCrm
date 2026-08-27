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
  LanguageStore,
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
  readonly lang = inject(LanguageStore);
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

  readonly columns = computed<CrmDataViewColumn[]>(() => [
    { key: 'occurredAt', header: this.lang.t('whenUtc') },
    { key: 'action', header: this.lang.t('actionCol') },
    { key: 'actorEmail', header: this.lang.t('actor') },
    { key: 'targetEmail', header: this.lang.t('target') },
    { key: 'detail', header: this.lang.t('detail') },
    { key: 'success', header: this.lang.t('result') },
  ]);

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
