import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Attachment,
  Contact,
  CustomerDetail,
  CustomerSummary,
  Note,
} from './customer.models';

/** SDD CRM-001 / specs/002-customer-profiles — commands and queries via gateway. */
@Injectable({ providedIn: 'root' })
export class CustomersApi {
  private readonly http = inject(HttpClient);

  search(q = ''): Observable<CustomerSummary[]> {
    const query = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<CustomerSummary[]>(`/api/customers${query}`);
  }

  get(id: string): Observable<CustomerDetail> {
    return this.http.get<CustomerDetail>(`/api/customers/${id}`);
  }

  create(body: {
    displayName: string;
    uniqueIdentifier: string;
    organization?: string;
    status?: string;
  }): Observable<CustomerSummary> {
    return this.http.post<CustomerSummary>('/api/customers', body);
  }

  update(
    id: string,
    body: {
      displayName: string;
      uniqueIdentifier: string;
      organization?: string;
      status?: string;
    },
  ): Observable<CustomerSummary> {
    return this.http.put<CustomerSummary>(`/api/customers/${id}`, body);
  }

  addContact(
    customerId: string,
    body: { type: string; value: string; isPrimary: boolean },
  ): Observable<Contact> {
    return this.http.post<Contact>(`/api/customers/${customerId}/contacts`, body);
  }

  deactivateContact(customerId: string, contactId: string): Observable<void> {
    return this.http.post<void>(
      `/api/customers/${customerId}/contacts/${contactId}/deactivate`,
      {},
    );
  }

  addNote(customerId: string, body: string): Observable<Note> {
    return this.http.post<Note>(`/api/customers/${customerId}/notes`, { body });
  }

  uploadAttachment(customerId: string, file: File): Observable<Attachment> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<Attachment>(`/api/customers/${customerId}/attachments`, form);
  }

  attachmentUrl(customerId: string, attachmentId: string): string {
    return `/api/customers/${customerId}/attachments/${attachmentId}`;
  }
}
