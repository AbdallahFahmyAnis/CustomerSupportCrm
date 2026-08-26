import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  CrmModalComponent,
  CrmWizardComponent,
  CrmWizardStep,
  CrmWizardStepDirective,
  FormFeedbackStore,
  LanguageStore,
} from 'shared';
import { DuplicateWarning } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001 — create customer wizard (Materio create-deal shape). */
@Component({
  selector: 'app-customer-create',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    CrmWizardComponent,
    CrmWizardStepDirective,
    CrmModalComponent,
  ],
  templateUrl: './create-customer.html',
  styleUrls: ['./create-customer.scss'],
})
export class CustomerCreateComponent {
  readonly lang = inject(LanguageStore);
  private readonly api = inject(CustomersApi);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly router = inject(Router);

  step = 0;
  attempted = false;
  displayName = '';
  uniqueIdentifier = '';
  organization = '';
  status = 'Active';
  dupOpen = false;
  readonly warning = signal<DuplicateWarning | null>(null);

  readonly steps = computed<CrmWizardStep[]>(() => [
    { title: this.lang.t('stepDetails'), subtitle: this.lang.t('customerDetails') },
    { title: this.lang.t('stepReview'), subtitle: this.lang.t('confirmCreateCustomer') },
  ]);

  get avatarLetter(): string {
    return (this.displayName.trim() || '?').charAt(0).toUpperCase();
  }

  canAdvance(): boolean {
    return !!this.displayName.trim() && !!this.uniqueIdentifier.trim();
  }

  onAdvanceBlocked(): void {
    this.attempted = true;
    this.feedback.error('formInvalid');
  }

  save(): void {
    this.attempted = true;
    if (!this.canAdvance()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.warning.set(null);
    this.api
      .create({
        displayName: this.displayName.trim(),
        uniqueIdentifier: this.uniqueIdentifier.trim(),
        organization: this.organization.trim() || undefined,
        status: this.status,
      })
      .subscribe({
        next: (c) => {
          this.feedback.success('createCustomerSuccess');
          void this.router.navigate(['/agent/customers', c.id]);
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === 409) {
            this.warning.set(err.error as DuplicateWarning);
            this.dupOpen = true;
            return;
          }
          this.feedback.errorText(
            (err.error as { error?: string })?.error ?? this.lang.t('failGeneric'),
          );
        },
      });
  }
}
