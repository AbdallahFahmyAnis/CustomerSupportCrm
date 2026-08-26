import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  CrmWizardComponent,
  CrmWizardStep,
  CrmWizardStepDirective,
  FormFeedbackStore,
  LanguageStore,
} from 'shared';
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
  readonly lang = inject(LanguageStore);
  readonly store = inject(TicketsStore);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly api = inject(TicketsApi);
  private readonly router = inject(Router);

  step = 0;
  attempted = false;
  customers: CustomerOption[] = [];
  customerQuery = '';
  customerId = '';
  subject = '';
  description = '';
  category = 'General';
  priority = 'Medium';

  readonly steps = computed<CrmWizardStep[]>(() => [
    { title: this.lang.t('stepCustomer'), subtitle: this.lang.t('selectCustomer') },
    { title: this.lang.t('stepDetails'), subtitle: this.lang.t('ticketDetails') },
    { title: this.lang.t('stepClassify'), subtitle: this.lang.t('classification') },
    { title: this.lang.t('stepReview'), subtitle: this.lang.t('confirmCreateTicket') },
  ]);

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

  onAdvanceBlocked(): void {
    this.attempted = true;
    this.feedback.error('formInvalid');
  }

  submit(): void {
    this.attempted = true;
    const customer = this.customers.find((c) => c.id === this.customerId);
    if (!customer || !this.subject.trim()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.create(
      {
        customerId: customer.id,
        customerName: customer.displayName,
        subject: this.subject.trim(),
        description: this.description.trim() || undefined,
        category: this.category,
        priority: this.priority,
      },
      (id) => {
        this.feedback.success('createTicketSuccess');
        void this.router.navigate(['/agent/tickets', id]);
      },
      (msg) => this.feedback.errorText(msg),
    );
  }
}
