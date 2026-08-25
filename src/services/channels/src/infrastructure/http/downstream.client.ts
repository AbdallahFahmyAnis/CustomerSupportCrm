import { Injectable } from '@nestjs/common';
import { channelsConfig } from '../../app/config';

/** SDD CRM-012 — HTTP client to Customers + Tickets (internal). */
@Injectable()
export class DownstreamClient {
  private readonly customersBase = channelsConfig.customersUrl;
  private readonly ticketsBase = channelsConfig.ticketsUrl;

  async findOrCreateCustomer(
    name: string,
    email: string,
  ): Promise<{ id: string; displayName: string }> {
    return this.findOrCreateCustomerWithContact(name, email, 'Email', email);
  }

  /** SDD CRM-009 — phone unique id + WhatsApp contact. */
  async findOrCreateCustomerByPhone(
    name: string,
    phone: string,
  ): Promise<{ id: string; displayName: string }> {
    const unique = phone.trim();
    return this.findOrCreateCustomerWithContact(name, unique, 'WhatsApp', unique);
  }

  /** SDD CRM-011 — phone unique id + SMS contact. */
  async findOrCreateCustomerBySms(
    name: string,
    phone: string,
  ): Promise<{ id: string; displayName: string }> {
    const unique = phone.trim();
    return this.findOrCreateCustomerWithContact(name, unique, 'Sms', unique);
  }

  private async findOrCreateCustomerWithContact(
    name: string,
    uniqueIdentifier: string,
    contactType: string,
    contactValue: string,
  ): Promise<{ id: string; displayName: string }> {
    const unique = uniqueIdentifier.trim();
    const createRes = await fetch(`${this.customersBase}/api/customers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        displayName: name.trim(),
        uniqueIdentifier: unique,
        organization: null,
        status: 'Active',
      }),
    });

    if (createRes.status === 201) {
      const created = (await createRes.json()) as {
        id: string;
        displayName: string;
      };
      await fetch(`${this.customersBase}/api/customers/${created.id}/contacts`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          type: contactType,
          value: contactValue.trim(),
          isPrimary: true,
        }),
      }).catch(() => undefined);
      return { id: created.id, displayName: created.displayName };
    }

    if (createRes.status === 409) {
      const dup = (await createRes.json()) as { existingCustomerId?: string };
      const id = dup.existingCustomerId;
      if (!id) {
        throw new Error('Customer conflict without existingCustomerId.');
      }
      const detailRes = await fetch(`${this.customersBase}/api/customers/${id}`);
      if (!detailRes.ok) {
        return { id, displayName: name.trim() };
      }
      const detail = (await detailRes.json()) as { displayName: string };
      return { id, displayName: detail.displayName ?? name.trim() };
    }

    const text = await createRes.text();
    throw new Error(`Customers create failed (${createRes.status}): ${text}`);
  }

  async createTicket(input: {
    customerId: string;
    customerName: string;
    subject: string;
    description: string;
  }): Promise<{ id: string; ticketNumber: string; status: string }> {
    const res = await fetch(`${this.ticketsBase}/api/tickets`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Crm-User-Email': 'portal@crm.local',
      },
      body: JSON.stringify({
        customerId: input.customerId,
        customerName: input.customerName,
        subject: input.subject,
        description: input.description,
        category: 'General',
        priority: 'Medium',
      }),
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`Tickets create failed (${res.status}): ${text}`);
    }

    const ticket = (await res.json()) as {
      id: string;
      ticketNumber: string;
      status: string;
    };
    return ticket;
  }

  async getTicketStatus(ticketId: string): Promise<string | null> {
    try {
      const res = await fetch(`${this.ticketsBase}/api/tickets/${ticketId}`);
      if (!res.ok) {
        return null;
      }
      const ticket = (await res.json()) as { status?: string };
      return ticket.status ?? null;
    } catch {
      return null;
    }
  }

  async getTicket(ticketId: string): Promise<{
    id: string;
    subject: string;
    customerId: string;
    customerName: string;
  } | null> {
    try {
      const res = await fetch(`${this.ticketsBase}/api/tickets/${ticketId}`);
      if (!res.ok) {
        return null;
      }
      const ticket = (await res.json()) as {
        id: string;
        subject: string;
        customerId: string;
        customerName: string;
      };
      return ticket;
    } catch {
      return null;
    }
  }

  async getCustomerEmail(customerId: string): Promise<string | null> {
    try {
      const res = await fetch(`${this.customersBase}/api/customers/${customerId}`);
      if (!res.ok) {
        return null;
      }
      const detail = (await res.json()) as {
        uniqueIdentifier?: string;
        contacts?: { type: string; value: string; isPrimary: boolean; isActive: boolean }[];
      };
      const primaryEmail = detail.contacts?.find(
        (c) =>
          c.isActive &&
          c.type === 'Email' &&
          c.isPrimary &&
          /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(c.value),
      )?.value;
      if (primaryEmail) {
        return primaryEmail.trim().toLowerCase();
      }
      const anyEmail = detail.contacts?.find(
        (c) =>
          c.isActive &&
          c.type === 'Email' &&
          /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(c.value),
      )?.value;
      if (anyEmail) {
        return anyEmail.trim().toLowerCase();
      }
      const uid = detail.uniqueIdentifier?.trim().toLowerCase() ?? '';
      if (/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(uid)) {
        return uid;
      }
      return null;
    } catch {
      return null;
    }
  }

  /** SDD CRM-009 — resolve WhatsApp/phone contact for outbound. */
  async getCustomerPhone(customerId: string): Promise<string | null> {
    try {
      const res = await fetch(`${this.customersBase}/api/customers/${customerId}`);
      if (!res.ok) {
        return null;
      }
      const detail = (await res.json()) as {
        uniqueIdentifier?: string;
        contacts?: { type: string; value: string; isPrimary: boolean; isActive: boolean }[];
      };
      const types = new Set(['whatsapp', 'phone', 'sms']);
      const primary = detail.contacts?.find(
        (c) =>
          c.isActive &&
          types.has(c.type.toLowerCase()) &&
          c.isPrimary &&
          c.value.replace(/\D/g, '').length >= 8,
      )?.value;
      if (primary) {
        return primary.trim();
      }
      const any = detail.contacts?.find(
        (c) =>
          c.isActive &&
          types.has(c.type.toLowerCase()) &&
          c.value.replace(/\D/g, '').length >= 8,
      )?.value;
      if (any) {
        return any.trim();
      }
      const uid = detail.uniqueIdentifier?.trim() ?? '';
      if (!uid.includes('@') && uid.replace(/\D/g, '').length >= 8) {
        return uid;
      }
      return null;
    } catch {
      return null;
    }
  }
}
