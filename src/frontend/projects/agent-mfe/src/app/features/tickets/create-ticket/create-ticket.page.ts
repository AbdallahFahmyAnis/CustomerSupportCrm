import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CustomerOption } from '../tickets.models';
import { TicketsApi } from '../tickets.api';
import { TicketsStore } from '../tickets.store';

/** Smart create page — Feature-Based + Signals. */
@Component({
  selector: 'app-create-ticket-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './create-ticket.html',
  styleUrls: ['./create-ticket.scss'],
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
