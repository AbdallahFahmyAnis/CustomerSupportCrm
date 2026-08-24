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
  template: `
    <section class="page">
      <header class="row">
        <h1>Customers</h1>
        <a routerLink="/agent/customers/new" class="btn">Create customer</a>
      </header>
      <form class="row" (ngSubmit)="load()">
        <input name="q" [(ngModel)]="q" placeholder="Search name, id, organization" />
        <button type="submit" class="btn secondary">Search</button>
      </form>
      @if (error()) {
        <p class="error">{{ error() }}</p>
      }
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Unique ID</th>
            <th>Organization</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          @for (c of customers(); track c.id) {
            <tr>
              <td><a [routerLink]="['/agent/customers', c.id]">{{ c.displayName }}</a></td>
              <td>{{ c.uniqueIdentifier }}</td>
              <td>{{ c.organization || '—' }}</td>
              <td>{{ c.status }}</td>
            </tr>
          }
        </tbody>
      </table>
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; }
    .row { display: flex; flex-wrap: wrap; gap: 0.75rem; align-items: center; margin-bottom: 1rem; }
    input { flex: 1; min-width: 12rem; padding: 0.45rem 0.6rem; }
    table { width: 100%; border-collapse: collapse; background: #fff; }
    th, td { text-align: start; padding: 0.6rem; border-bottom: 1px solid #e2e8f0; }
    .btn { background: #2563eb; color: #fff; text-decoration: none; border: 0; border-radius: 0.375rem; padding: 0.45rem 0.8rem; }
    .btn.secondary { background: #334155; }
    .error { color: #b91c1c; }
  `,
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
