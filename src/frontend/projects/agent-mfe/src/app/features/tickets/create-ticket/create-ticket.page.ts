import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CrmWizardComponent, CrmWizardStep, CrmWizardStepDirective } from 'shared';
import { CustomerOption } from '../tickets.models';
import { TicketsApi } from '../tickets.api';
import { TicketsStore } from '../tickets.store';

/** Smart create ticket wizard — Feature-Based + Signals. */
@Component({
  selector: 'app-create-ticket-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmWizardComponent, CrmWizardStepDirective],
  templateUrl: './create-ticket.html',
  styleUrls: ['./create-ticket.scss'],
})
export class TicketCreatePage implements OnInit {
  readonly store = inject(TicketsStore);
  private readonly api = inject(TicketsApi);
  private readonly router = inject(Router);

  step = 0;
  customers: CustomerOption[] = [];
  customerQuery = '';
  customerId = '';
  subject = '';
  description = '';
  category = 'General';
  priority = 'Medium';

  readonly steps: CrmWizardStep[] = [
    { title: 'Customer', subtitle: 'Find and select' },
    { title: 'Details', subtitle: 'Subject and body' },
    { title: 'Classify', subtitle: 'Category and priority' },
    { title: 'Review', subtitle: 'Confirm and create' },
  ];

  ngOnInit(): void {
    this.store.loadOptions();
    this.findCustomers();
  }

  get selectedCustomerLabel(): string {
    const c = this.customers.find((x) => x.id === this.customerId);
    return c ? `${c.displayName} (${c.uniqueIdentifier})` : '—';
  }

  findCustomers(): void {
    this.api.searchCustomers(this.customerQuery).subscribe({
      next: (rows) => (this.customers = rows),
    });
  }

  canAdvance(): boolean {
    if (this.step === 0) {
      return !!this.customerId;
    }
    if (this.step === 1) {
      return !!this.subject.trim();
    }
    return true;
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
