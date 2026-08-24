import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CustomerDetail } from './customer.models';
import { CustomersApi } from './customers.api';

/** SDD CRM-001/002/003 — customer detail smart container. */
@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  template: `
    <section class="page">
      <a routerLink="/agent/customers">← Customers</a>
      @if (customer(); as c) {
        <header class="row">
          <div>
            <h1>{{ c.displayName }}</h1>
            <p>{{ c.uniqueIdentifier }} · {{ c.organization || 'No organization' }} · {{ c.status }}</p>
          </div>
          <a class="btn" [routerLink]="['/agent/customers', c.id, 'edit']">Edit</a>
        </header>

        <div class="grid">
          <article>
            <h2>Contacts</h2>
            <ul>
              @for (contact of c.contacts; track contact.id) {
                <li [class.inactive]="!contact.isActive">
                  <strong>{{ contact.type }}</strong>: {{ contact.value }}
                  @if (contact.isPrimary) { <em>primary</em> }
                  @if (!contact.isActive) { <em>inactive</em> }
                  @if (contact.isActive) {
                    <button type="button" class="link" (click)="deactivate(contact.id)">Deactivate</button>
                  }
                </li>
              }
            </ul>
            <form class="stack" (ngSubmit)="addContact()">
              <select name="type" [(ngModel)]="contactType">
                <option value="email">email</option>
                <option value="phone">phone</option>
                <option value="whatsapp">whatsapp</option>
                <option value="address">address</option>
              </select>
              <input name="value" [(ngModel)]="contactValue" placeholder="Value" required />
              <label><input type="checkbox" name="primary" [(ngModel)]="contactPrimary" /> Primary</label>
              <button type="submit" class="btn secondary">Add contact</button>
            </form>
          </article>

          <article>
            <h2>Notes</h2>
            <form class="stack" (ngSubmit)="addNote()">
              <textarea name="note" [(ngModel)]="noteBody" rows="3" required></textarea>
              <button type="submit" class="btn secondary">Add note</button>
            </form>
          </article>

          <article>
            <h2>Attachments</h2>
            <input type="file" (change)="onFile($event)" />
            <ul>
              @for (a of c.attachments; track a.id) {
                <li>
                  <a [href]="api.attachmentUrl(c.id, a.id)" target="_blank" rel="noopener">{{ a.fileName }}</a>
                </li>
              }
            </ul>
          </article>

          <article class="full">
            <h2>Timeline</h2>
            <ul class="timeline">
              @for (item of c.timeline; track item.id) {
                <li>
                  <span class="kind">{{ item.kind }}</span>
                  <span>{{ item.summary }}</span>
                  <time>{{ item.occurredAt | date: 'medium' }}</time>
                </li>
              }
            </ul>
          </article>
        </div>
      } @else if (error()) {
        <p class="error">{{ error() }}</p>
      } @else {
        <p>Loading…</p>
      }
    </section>
  `,
  styles: `
    .page { padding: 1.25rem; }
    .row { display: flex; justify-content: space-between; gap: 1rem; align-items: start; }
    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(16rem, 1fr)); gap: 1rem; }
    article { background: #fff; padding: 1rem; border-radius: 0.5rem; border: 1px solid #e2e8f0; }
    article.full { grid-column: 1 / -1; }
    .stack { display: grid; gap: 0.5rem; margin-top: 0.75rem; }
    .btn { background: #2563eb; color: #fff; text-decoration: none; border: 0; border-radius: 0.375rem; padding: 0.45rem 0.8rem; }
    .btn.secondary { background: #334155; }
    .link { background: none; border: 0; color: #2563eb; cursor: pointer; }
    .inactive { opacity: 0.55; }
    .timeline { list-style: none; padding: 0; margin: 0; display: grid; gap: 0.5rem; }
    .timeline li { display: grid; gap: 0.15rem; padding: 0.5rem 0; border-bottom: 1px solid #e2e8f0; }
    .kind { text-transform: uppercase; font-size: 0.75rem; color: #64748b; }
    .error { color: #b91c1c; }
  `,
})
export class CustomerDetailComponent implements OnInit {
  readonly api = inject(CustomersApi);
  private readonly route = inject(ActivatedRoute);
  readonly customer = signal<CustomerDetail | null>(null);
  readonly error = signal('');
  contactType = 'email';
  contactValue = '';
  contactPrimary = false;
  noteBody = '';
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

  deactivate(contactId: string): void {
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
