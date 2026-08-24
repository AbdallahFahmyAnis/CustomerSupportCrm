import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CustomerSummary } from './customer.models';
import { CustomersApi } from './customers.api';

/** SDD CRM-001 — customer list / search container. */
@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
})
export class CustomerListComponent implements OnInit {
  private readonly api = inject(CustomersApi);
  readonly customers = signal<CustomerSummary[]>([]);
  readonly error = signal('');
  q = '';

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
