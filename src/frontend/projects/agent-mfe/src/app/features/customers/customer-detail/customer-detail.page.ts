import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CrmModalComponent } from 'shared';
import { CustomerDetail } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001/002/003 — customer detail (Materio account-settings shape). */
@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe, CrmModalComponent],
  templateUrl: './customer-detail.html',
  styleUrls: ['./customer-detail.scss'],
})
export class CustomerDetailComponent implements OnInit {
  readonly api = inject(CustomersApi);
  private readonly route = inject(ActivatedRoute);
  readonly customer = signal<CustomerDetail | null>(null);
  readonly error = signal('');
  tab: 'contacts' | 'activity' | 'timeline' = 'contacts';
  contactType = 'email';
  contactValue = '';
  contactPrimary = false;
  noteBody = '';
  confirmOpen = false;
  private pendingContactId = '';
  private id = '';

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.reload();
  }

  reload(): void {
    this.api.get(this.id).subscribe({
      next: (c) => this.customer.set(c),
      error: () => this.error.set('Customer not found.'),
    });
  }

  addContact(): void {
    this.api
      .addContact(this.id, {
        type: this.contactType,
        value: this.contactValue,
        isPrimary: this.contactPrimary,
      })
      .subscribe({
        next: () => {
          this.contactValue = '';
          this.contactPrimary = false;
          this.reload();
        },
      });
  }

  askDeactivate(contactId: string): void {
    this.pendingContactId = contactId;
    this.confirmOpen = true;
  }

  confirmDeactivate(): void {
    const contactId = this.pendingContactId;
    this.confirmOpen = false;
    this.pendingContactId = '';
    if (!contactId) {
      return;
    }
    this.api.deactivateContact(this.id, contactId).subscribe({ next: () => this.reload() });
  }

  addNote(): void {
    this.api.addNote(this.id, this.noteBody).subscribe({
      next: () => {
        this.noteBody = '';
        this.reload();
      },
    });
  }

  onFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }
    this.api.uploadAttachment(this.id, file).subscribe({
      next: () => {
        input.value = '';
        this.reload();
      },
    });
  }
}
