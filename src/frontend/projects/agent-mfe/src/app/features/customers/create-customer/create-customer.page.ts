import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  CrmModalComponent,
  CrmWizardComponent,
  CrmWizardStep,
  CrmWizardStepDirective,
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
  private readonly api = inject(CustomersApi);
  private readonly router = inject(Router);

  step = 0;
  displayName = '';
  uniqueIdentifier = '';
  organization = '';
  status = 'Active';
  dupOpen = false;
  readonly warning = signal<DuplicateWarning | null>(null);
  readonly error = signal('');

  readonly steps: CrmWizardStep[] = [
    { title: 'Details', subtitle: 'Profile and organization' },
    { title: 'Review', subtitle: 'Confirm and create' },
  ];

  get avatarLetter(): string {
    return (this.displayName.trim() || '?').charAt(0).toUpperCase();
  }

  canAdvance(): boolean {
    if (this.step === 0) {
      return !!this.displayName.trim() && !!this.uniqueIdentifier.trim();
    }
    return !!this.displayName.trim() && !!this.uniqueIdentifier.trim();
  }

  save(): void {
    this.warning.set(null);
    this.error.set('');
    this.api
      .create({
        displayName: this.displayName.trim(),
        uniqueIdentifier: this.uniqueIdentifier.trim(),
        organization: this.organization.trim() || undefined,
        status: this.status,
      })
      .subscribe({
        next: (c) => void this.router.navigate(['/agent/customers', c.id]),
        error: (err: HttpErrorResponse) => {
          if (err.status === 409) {
            this.warning.set(err.error as DuplicateWarning);
            this.dupOpen = true;
            return;
          }
          this.error.set('Save failed.');
        },
      });
  }
}
