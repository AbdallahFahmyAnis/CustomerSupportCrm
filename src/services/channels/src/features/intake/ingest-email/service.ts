import { Injectable } from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { IngestEmailCommand, IngestEmailResult } from './email.types';

/** SDD CRM-008 — create customer + ticket and persist Email channel message. */
@Injectable()
export class IngestEmailService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async ingest(command: IngestEmailCommand): Promise<IngestEmailResult> {
    const from = command.from.trim().toLowerCase();
    const subject = command.subject.trim();
    const body = command.body.trim();
    const name = (command.name?.trim() || from.split('@')[0] || 'Email Customer').trim();

    const customer = await this.downstream.findOrCreateCustomer(name, from);
    const ticket = await this.downstream.createTicket({
      customerId: customer.id,
      customerName: customer.displayName,
      subject,
      description: body,
    });

    const now = new Date().toISOString();
    const requestId = randomUUID();
    await this.store.addRequest(
      {
        id: requestId,
        ticketId: ticket.id,
        ticketNumber: ticket.ticketNumber,
        customerId: customer.id,
        email: from,
        name: customer.displayName,
        subject,
        status: ticket.status ?? 'New',
        createdAt: now,
      },
      {
        id: randomUUID(),
        ticketId: ticket.id,
        channel: 'Email',
        direction: 'Inbound',
        body,
        fromEmail: from,
        createdAt: now,
      },
    );

    return {
      requestId,
      ticketId: ticket.id,
      ticketNumber: ticket.ticketNumber,
    };
  }
}
