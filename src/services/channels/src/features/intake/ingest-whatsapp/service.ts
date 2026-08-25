import { Injectable } from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { normalizePhone } from './schema';
import { IngestWhatsAppCommand, IngestWhatsAppResult } from './whatsapp.types';

/** SDD CRM-009 — create customer + ticket and persist WhatsApp channel message. */
@Injectable()
export class IngestWhatsAppService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async ingest(command: IngestWhatsAppCommand): Promise<IngestWhatsAppResult> {
    const from = normalizePhone(command.from);
    const body = command.body.trim();
    const subject =
      command.subject?.trim() ||
      `WhatsApp from ${from}`;
    const name =
      (command.name?.trim() || `WhatsApp ${from}`).trim();

    const customer = await this.downstream.findOrCreateCustomerByPhone(
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
        channel: 'WhatsApp',
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
