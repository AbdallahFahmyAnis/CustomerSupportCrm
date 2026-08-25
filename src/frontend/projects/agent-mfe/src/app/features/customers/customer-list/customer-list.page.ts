import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataToolbarDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
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
  private readonly api = inject(CustomersApi);
  readonly customers = signal<CustomerSummary[]>([]);
  readonly error = signal('');
  q = '';
  viewMode: CrmDataViewMode = 'list';

  readonly columns: CrmDataViewColumn[] = [
    { key: 'displayName', header: 'Name' },
    { key: 'uniqueIdentifier', header: 'Unique ID' },
    { key: 'organization', header: 'Organization' },
    { key: 'status', header: 'Status' },
  ];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.error.set('');
    this.api.search(this.q).subscribe({
      next: (rows) => this.customers.set(rows),
      error: () => this.error.set('Could not load customers.'),
    });
  }
}
