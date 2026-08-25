import { BadRequestException, Injectable } from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { IngestChatCommand, IngestChatResult } from './chat.types';

/** SDD CRM-010 — create or continue live chat ticket and persist channel message. */
@Injectable()
export class IngestChatService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async ingest(command: IngestChatCommand): Promise<IngestChatResult> {
    const email = command.email.trim().toLowerCase();
    const body = command.body.trim();
    const name = (command.name?.trim() || email.split('@')[0] || 'Chat visitor').trim();
    const subject =
      command.subject?.trim() ||
      `Live chat from ${email}`;

    if (command.ticketId?.trim()) {
      return this.continueTicket(command.ticketId.trim(), email, body);
    }

    const customer = await this.downstream.findOrCreateCustomer(name, email);
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
        email,
        name: customer.displayName,
        subject,
        status: ticket.status ?? 'New',
        createdAt: now,
      },
      {
        id: randomUUID(),
        ticketId: ticket.id,
        channel: 'LiveChat',
        direction: 'Inbound',
        body,
        fromEmail: email,
        createdAt: now,
      },
    );

    return {
      requestId,
      ticketId: ticket.id,
      ticketNumber: ticket.ticketNumber,
    };
  }

  private async continueTicket(
    ticketId: string,
    email: string,
    body: string,
  ): Promise<IngestChatResult> {
    const existing = await this.store.findRequestByTicketId(ticketId);
    if (!existing) {
      throw new BadRequestException('ticketId is not a known chat/portal request.');
    }
    if (existing.email.trim().toLowerCase() !== email) {
      throw new BadRequestException('email does not match this chat ticket.');
    }

    const ticket = await this.downstream.getTicket(ticketId);
    if (!ticket) {
      throw new BadRequestException('Ticket not found.');
    }

    await this.store.addMessage({
      id: randomUUID(),
      ticketId,
      channel: 'LiveChat',
      direction: 'Inbound',
      body,
      fromEmail: email,
      createdAt: new Date().toISOString(),
    });

    return {
      requestId: existing.id,
      ticketId,
      ticketNumber: existing.ticketNumber || ticket.ticketNumber,
    };
  }
}
