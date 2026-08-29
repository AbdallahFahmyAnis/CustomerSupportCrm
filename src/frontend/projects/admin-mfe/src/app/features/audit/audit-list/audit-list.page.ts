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
  MessageKey,
} from 'shared';
import { AuditStore } from '../audit.store';

/** SDD CRM-036 / specs/051 — audit list smart page. */
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
  serviceFilter = '';
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
    { key: 'service', header: this.lang.t('auditService') },
    { key: 'action', header: this.lang.t('actionCol') },
    { key: 'actorEmail', header: this.lang.t('actor') },
    { key: 'targetEmail', header: this.lang.t('target') },
    { key: 'detail', header: this.lang.t('detail') },
    { key: 'success', header: this.lang.t('result') },
  ]);

  readonly pageLabel = computed(() => {
    const total = this.store.total();
    if (total === 0) {
      return this.lang.t('auditNoRows');
    }
    const start = this.store.skip() + 1;
    const end = Math.min(this.store.skip() + this.store.take(), total);
    return this.lang
      .t('auditPageOf')
      .replace('{start}', String(start))
      .replace('{end}', String(end))
      .replace('{total}', String(total));
  });

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.service.set(this.serviceFilter);
    this.store.skip.set(0);
    this.fromSig.set(this.fromDate);
    this.toSig.set(this.toDate);
    this.store.load();
  }

  prevPage(): void {
    if (this.store.pageIndex() <= 0) {
      return;
    }
    this.store.goToPage(this.store.pageIndex() - 1);
  }

  nextPage(): void {
    if (this.store.pageIndex() + 1 >= this.store.pageCount()) {
      return;
    }
    this.store.goToPage(this.store.pageIndex() + 1);
  }

  errorText(): string {
    const key = this.store.error();
    if (!key) {
      return '';
    }
    return key === 'auditLoadFailed' ? this.lang.t('auditLoadFailed' as MessageKey) : key;
  }
}
