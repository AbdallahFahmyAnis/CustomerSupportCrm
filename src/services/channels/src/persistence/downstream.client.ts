import { Injectable } from '@nestjs/common';

/** SDD CRM-012 — HTTP client to Customers + Tickets (internal). */
@Injectable()
export class DownstreamClient {
  private readonly customersBase = (
    process.env.CUSTOMERS_URL ?? 'http://localhost:5102'
  ).replace(/\/$/, '');
  private readonly ticketsBase = (
    process.env.TICKETS_URL ?? 'http://localhost:5103'
  ).replace(/\/$/, '');

  async findOrCreateCustomer(
    name: string,
    email: string,
  ): Promise<{ id: string; displayName: string }> {
    const unique = email.trim().toLowerCase();
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
          type: 'Email',
          value: unique,
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
}
