import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DuplicateWarning } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001 — create customer command container. */
@Component({
  selector: 'app-customer-create',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './create-customer.html',
  styleUrls: ['./create-customer.scss'],
})
export class CustomerCreateComponent {
  private readonly api = inject(CustomersApi);
  private readonly router = inject(Router);
  displayName = '';
  uniqueIdentifier = '';
  organization = '';
  status = 'Active';
  readonly warning = signal<DuplicateWarning | null>(null);
  readonly error = signal('');

  save(): void {
    this.warning.set(null);
    this.error.set('');
    this.api
      .create({
        displayName: this.displayName,
        uniqueIdentifier: this.uniqueIdentifier,
        organization: this.organization || undefined,
        status: this.status,
      })
      .subscribe({
        next: (c) => void this.router.navigate(['/agent/customers', c.id]),
        error: (err: HttpErrorResponse) => {
          if (err.status === 409) {
            this.warning.set(err.error as DuplicateWarning);
            return;
          }
          this.error.set('Save failed.');
        },
      });
  }
}
