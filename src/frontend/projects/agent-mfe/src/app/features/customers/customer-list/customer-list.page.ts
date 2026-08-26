import { Component, OnInit, computed, inject, signal } from '@angular/core';
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
} from 'shared';
import { CustomerSummary } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001 — customer list / search container. */
@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    CrmDataViewComponent,
    CrmDataToolbarDirective,
    CrmDataCellDirective,
    CrmDataCardDirective,
  ],
  templateUrl: './customer-list.html',
  styleUrls: ['./customer-list.scss'],
})
export class CustomerListComponent implements OnInit {
  readonly lang = inject(LanguageStore);
  private readonly api = inject(CustomersApi);
  readonly customers = signal<CustomerSummary[]>([]);
  readonly error = signal('');
  q = '';
  viewMode: CrmDataViewMode = 'list';

  readonly columns = computed<CrmDataViewColumn[]>(() => [
    { key: 'displayName', header: this.lang.t('name') },
    { key: 'uniqueIdentifier', header: this.lang.t('uniqueId') },
    { key: 'organization', header: this.lang.t('organization') },
    { key: 'status', header: this.lang.t('status') },
  ]);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.error.set('');
    this.api.search(this.q).subscribe({
      next: (rows) => this.customers.set(rows),
      error: () => this.error.set(this.lang.t('loadCustomersFailed')),
    });
  }
}
