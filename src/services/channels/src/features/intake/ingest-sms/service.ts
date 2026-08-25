import { Injectable } from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { normalizePhone } from './schema';
import { IngestSmsCommand, IngestSmsResult } from './sms.types';

/** SDD CRM-011 — create customer + ticket and persist SMS channel message. */
@Injectable()
export class IngestSmsService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async ingest(command: IngestSmsCommand): Promise<IngestSmsResult> {
    const from = normalizePhone(command.from);
    const body = command.body.trim();
    const subject = command.subject?.trim() || `SMS from ${from}`;
    const name = (command.name?.trim() || `SMS ${from}`).trim();

    const customer = await this.downstream.findOrCreateCustomerBySms(
      name,
      from,
    );
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
        channel: 'Sms',
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
