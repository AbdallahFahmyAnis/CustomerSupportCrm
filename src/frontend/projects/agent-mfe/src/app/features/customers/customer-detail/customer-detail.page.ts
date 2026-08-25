import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CrmModalComponent, CrmTimelineComponent, CrmTimelineItem } from 'shared';
import { CustomerDetail } from '../customers.models';
import { CustomersApi } from '../customers.api';

/** SDD CRM-001/002/003 — customer detail (Materio account-settings shape). */
@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmModalComponent, CrmTimelineComponent],
  templateUrl: './customer-detail.html',
  styleUrls: ['./customer-detail.scss'],
  providers: [DatePipe],
})
export class CustomerDetailComponent implements OnInit {
  readonly api = inject(CustomersApi);
  private readonly route = inject(ActivatedRoute);
  private readonly datePipe = inject(DatePipe);
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

  readonly timelineItems = computed<CrmTimelineItem[]>(() => {
    const c = this.customer();
    if (!c) {
      return [];
    }
    return c.timeline.map((item) => ({
      id: item.id,
      title: item.kind,
      body: item.summary,
      timeLabel: this.datePipe.transform(item.occurredAt, 'medium') ?? '',
    }));
  });

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
