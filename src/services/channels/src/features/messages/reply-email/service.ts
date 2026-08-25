import {
  BadRequestException,
  Inject,
  Injectable,
  NotFoundException,
} from '@nestjs/common';
import { randomUUID } from 'crypto';
import { channelsConfig } from '../../../app/config';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import {
  EMAIL_PROVIDER,
  EmailProvider,
} from '../../../infrastructure/email/email-provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { ReplyEmailCommand, ReplyEmailResult } from './reply.types';

/** SDD CRM-040 — send outbound email and persist channel message. */
@Injectable()
export class ReplyEmailService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
    @Inject(EMAIL_PROVIDER) private readonly emailProvider: EmailProvider,
  ) {}

  async reply(command: ReplyEmailCommand): Promise<ReplyEmailResult> {
    const ticketId = command.ticketId.trim();
    const body = command.body.trim();
    const ticket = await this.downstream.getTicket(ticketId);
    if (!ticket) {
      throw new NotFoundException('Ticket not found.');
    }

    let to = command.to?.trim().toLowerCase() ?? '';
    if (!to) {
      const request = await this.store.findRequestByTicketId(ticketId);
      if (request?.email) {
        to = request.email;
      }
    }
    if (!to) {
      const messages = await this.store.listMessagesForTicket(ticketId);
      const inbound = [...messages]
        .reverse()
        .find((m) => m.direction === 'Inbound' && m.fromEmail);
      if (inbound?.fromEmail) {
        to = inbound.fromEmail.trim().toLowerCase();
      }
    }
    if (!to) {
      to = (await this.downstream.getCustomerEmail(ticket.customerId)) ?? '';
    }
    if (!to) {
      throw new BadRequestException(
        'Could not resolve recipient email for this ticket.',
      );
    }

    const subject = ticket.subject.startsWith('Re:')
      ? ticket.subject
      : `Re: ${ticket.subject}`;

    await this.emailProvider.sendOutbound({
      to,
      subject,
      body,
      ticketId,
    });

    const messageId = randomUUID();
    await this.store.addMessage({
      id: messageId,
      ticketId,
      channel: 'Email',
      direction: 'Outbound',
      body,
      fromEmail: channelsConfig.smtpFrom,
      createdAt: new Date().toISOString(),
    });

    return { messageId, ticketId, to };
  }
}
