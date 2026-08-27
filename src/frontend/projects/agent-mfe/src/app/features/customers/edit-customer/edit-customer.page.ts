import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
  readonly lang = inject(LanguageStore);
  private readonly api = inject(CustomersApi);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  id = '';
  step = 0;
  displayName = '';
  uniqueIdentifier = '';
  organization = '';
  status = 'Active';
  dupOpen = false;
  attempted = false;
  readonly warning = signal<DuplicateWarning | null>(null);
  readonly error = signal('');

  readonly steps = computed<CrmWizardStep[]>(() => [
    { title: this.lang.t('stepDetails'), subtitle: this.lang.t('profileAndOrg') },
    { title: this.lang.t('stepReview'), subtitle: this.lang.t('confirmAndSave') },
  ]);

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
      error: () => this.error.set(this.lang.t('customerNotFound')),
    });
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
    this.error.set('');
    this.api
      .update(this.id, {
        displayName: this.displayName.trim(),
        uniqueIdentifier: this.uniqueIdentifier.trim(),
        organization: this.organization.trim() || undefined,
        status: this.status,
      })
      .subscribe({
        next: () => {
          this.feedback.success('successGeneric');
          void this.router.navigate(['/agent/customers', this.id]);
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === 409) {
            this.warning.set(err.error as DuplicateWarning);
            this.dupOpen = true;
            return;
          }
          this.feedback.error('saveFailed');
          this.error.set(this.lang.t('saveFailed'));
        },
      });
  }
}
