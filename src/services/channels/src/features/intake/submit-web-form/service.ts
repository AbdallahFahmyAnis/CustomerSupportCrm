import { Injectable } from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import {
  SubmitWebFormCommand,
  SubmitWebFormResult,
} from '../intake.types';

/** SDD CRM-012 / CRM-027 — create customer + ticket and persist request. */
@Injectable()
export class SubmitWebFormService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async submit(command: SubmitWebFormCommand): Promise<SubmitWebFormResult> {
    const name = command.name.trim();
    const email = command.email.trim().toLowerCase();
    const subject = command.subject.trim();
    const message = command.message.trim();

    const customer = await this.downstream.findOrCreateCustomer(name, email);
    const ticket = await this.downstream.createTicket({
      customerId: customer.id,
      customerName: customer.displayName,
      subject,
      description: message,
    });

    const now = new Date().toISOString();
    const requestId = randomUUID();
    this.store.addRequest(
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
        channel: 'WebForm',
        direction: 'Inbound',
        body: message,
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
}
