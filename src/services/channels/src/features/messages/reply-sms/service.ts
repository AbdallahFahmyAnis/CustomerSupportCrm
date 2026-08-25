import {
  BadRequestException,
  Inject,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import {
  SMS_PROVIDER,
  SmsProvider,
} from '../../../infrastructure/sms/sms-provider';
import { normalizePhone } from '../../intake/ingest-sms/schema';
import { ReplySmsCommand, ReplySmsResult } from './reply.types';

/** SDD CRM-011 / CRM-040 — send outbound SMS and persist channel message. */
@Injectable()
export class ReplySmsService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
    @Inject(SMS_PROVIDER) private readonly smsProvider: SmsProvider,
  ) {}

  async reply(command: ReplySmsCommand): Promise<ReplySmsResult> {
    const ticketId = command.ticketId.trim();
    const body = command.body.trim();
    const ticket = await this.downstream.getTicket(ticketId);
    if (!ticket) {
      throw new NotFoundException('Ticket not found.');
    }

    let to = command.to ? normalizePhone(command.to) : '';
    if (!to) {
      const request = await this.store.findRequestByTicketId(ticketId);
      if (request?.email && !request.email.includes('@')) {
        to = normalizePhone(request.email);
      }
    }
    if (!to) {
      const messages = await this.store.listMessagesForTicket(ticketId);
      const inbound = [...messages]
        .reverse()
        .find(
          (m) =>
            m.channel === 'Sms' &&
            m.direction === 'Inbound' &&
            m.fromEmail,
        );
      if (inbound?.fromEmail) {
        to = normalizePhone(inbound.fromEmail);
      }
    }
    if (!to) {
      to = (await this.downstream.getCustomerPhone(ticket.customerId)) ?? '';
    }
    if (!to) {
      throw new BadRequestException(
        'Could not resolve recipient phone for this ticket.',
      );
    }

    await this.smsProvider.sendOutbound({
      to,
      body,
      ticketId,
    });

    const messageId = randomUUID();
    await this.store.addMessage({
      id: messageId,
      ticketId,
      channel: 'Sms',
      direction: 'Outbound',
      body,
      fromEmail: 'crm-sms',
      createdAt: new Date().toISOString(),
    });

    return { messageId, ticketId, to };
  }
}
