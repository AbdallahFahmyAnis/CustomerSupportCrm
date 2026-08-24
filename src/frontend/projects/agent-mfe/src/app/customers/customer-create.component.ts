import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DuplicateWarning } from './customer.models';
import { CustomersApi } from './customers.api';

/** SDD CRM-001 — create customer command container. */
@Component({
  selector: 'app-customer-create',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <section class="page">
      <a routerLink="/agent/customers">← Back</a>
      <h1>Create customer</h1>
      <form (ngSubmit)="save()">
        <label>Name <input name="displayName" [(ngModel)]="displayName" required /></label>
        <label>Unique identifier <input name="uniqueIdentifier" [(ngModel)]="uniqueIdentifier" required /></label>
        <label>Organization <input name="organization" [(ngModel)]="organization" /></label>
        <label>Status
          <select name="status" [(ngModel)]="status">
            <option>Active</option>
            <option>Inactive</option>
          </select>
        </label>
        <button type="submit" class="btn">Save</button>
      </form>
      @if (warning()) {
        <p class="warn">
          {{ warning()!.message }}
          <a [routerLink]="['/agent/customers', warning()!.existingCustomerId]">Open existing</a>
        </p>
      }
      @if (error()) {
        <p class="error">{{ error() }}</p>
      }
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; max-width: 32rem; display: grid; gap: 0.75rem; }
    form { display: grid; gap: 0.75rem; }
    label { display: grid; gap: 0.25rem; }
    input, select { padding: 0.45rem 0.6rem; }
    .btn { background: #2563eb; color: #fff; border: 0; border-radius: 0.375rem; padding: 0.5rem 0.9rem; width: fit-content; }
    .warn { color: #92400e; background: #fef3c7; padding: 0.75rem; border-radius: 0.375rem; }
    .error { color: #b91c1c; }
  `,
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
