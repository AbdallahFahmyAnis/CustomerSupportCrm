import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CustomerOption } from '../data-access/ticket.models';
import { TicketsApi } from '../data-access/tickets.api';
import { TicketsStore } from '../data-access/tickets.store';

/** Smart create page — Feature-Based + Signals. */
@Component({
  selector: 'app-ticket-create-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <section class="page">
      <p><a routerLink="/agent/tickets">← Tickets</a></p>
      <h1>Create ticket</h1>

      @if (store.error()) {
        <p class="error">{{ store.error() }}</p>
      }

      <form class="form" (ngSubmit)="submit()">
        <label>
          Customer search
          <input name="cq" [(ngModel)]="customerQuery" (ngModelChange)="findCustomers()" placeholder="Name or id" />
        </label>
        <label>
          Customer
          <select name="customerId" [(ngModel)]="customerId" required>
            <option value="" disabled>Select customer</option>
            @for (c of customers; track c.id) {
              <option [value]="c.id">{{ c.displayName }} ({{ c.uniqueIdentifier }})</option>
            }
          </select>
        </label>
        <label>
          Subject
          <input name="subject" [(ngModel)]="subject" required />
        </label>
        <label>
          Description
          <textarea name="description" [(ngModel)]="description" rows="4"></textarea>
        </label>
        <label>
          Category
          <select name="category" [(ngModel)]="category" required>
            @for (c of store.options()?.categories ?? []; track c) {
              <option [value]="c">{{ c }}</option>
            }
          </select>
        </label>
        <label>
          Priority
          <select name="priority" [(ngModel)]="priority" required>
            @for (p of store.options()?.priorities ?? []; track p) {
              <option [value]="p">{{ p }}</option>
            }
          </select>
        </label>
        <button type="submit" class="btn">Create</button>
      </form>
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; max-width: 36rem; }
    .form { display: grid; gap: 0.85rem; }
    label { display: grid; gap: 0.35rem; font-weight: 600; }
    input, select, textarea { padding: 0.45rem 0.6rem; font: inherit; }
    .btn { background: #2563eb; color: #fff; border: 0; border-radius: 0.375rem; padding: 0.55rem 0.9rem; width: fit-content; }
    .error { color: #b91c1c; }
  `,
})
export class TicketCreatePage implements OnInit {
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly router = inject(Router);

  customers: CustomerOption[] = [];
  customerQuery = '';
  customerId = '';
  subject = '';
  description = '';
  category = 'General';
  priority = 'Medium';

  ngOnInit(): void {
    this.store.loadOptions();
    this.findCustomers();
  }

  findCustomers(): void {
    this.api.searchCustomers(this.customerQuery).subscribe({
      next: (rows) => (this.customers = rows),
    });
  }

  submit(): void {
    const customer = this.customers.find((c) => c.id === this.customerId);
    if (!customer || !this.subject.trim()) return;
    this.store.create(
      {
        customerId: customer.id,
        customerName: customer.displayName,
        subject: this.subject.trim(),
        description: this.description.trim() || undefined,
        category: this.category,
        priority: this.priority,
      },
      (id) => void this.router.navigate(['/agent/tickets', id]),
    );
  }
}
