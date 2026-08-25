import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  CrmModalComponent,
  CrmWizardComponent,
  CrmWizardStep,
  CrmWizardStepDirective,
} from 'shared';
import { DuplicateWarning } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001 — edit customer wizard. */
@Component({
  selector: 'app-customer-edit',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    CrmWizardComponent,
    CrmWizardStepDirective,
    CrmModalComponent,
  ],
  templateUrl: './edit-customer.html',
  styleUrls: ['./edit-customer.scss'],
})
export class CustomerEditComponent implements OnInit {
  private readonly api = inject(CustomersApi);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  id = '';
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
    { title: 'Review', subtitle: 'Confirm and save' },
  ];

  get avatarLetter(): string {
    return (this.displayName.trim() || '?').charAt(0).toUpperCase();
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.api.get(this.id).subscribe({
      next: (c) => {
        this.displayName = c.displayName;
        this.uniqueIdentifier = c.uniqueIdentifier;
        this.organization = c.organization ?? '';
        this.status = c.status;
      },
      error: () => this.error.set('Customer not found.'),
    });
  }

  canAdvance(): boolean {
    return !!this.displayName.trim() && !!this.uniqueIdentifier.trim();
  }

  save(): void {
    this.warning.set(null);
    this.error.set('');
    this.api
      .update(this.id, {
        displayName: this.displayName.trim(),
        uniqueIdentifier: this.uniqueIdentifier.trim(),
        organization: this.organization.trim() || undefined,
        status: this.status,
      })
      .subscribe({
        next: () => void this.router.navigate(['/agent/customers', this.id]),
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
