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
  CHAT_PROVIDER,
  ChatProvider,
} from '../../../infrastructure/chat/chat-provider';
import { ReplyChatCommand, ReplyChatResult } from './reply.types';

/** SDD CRM-010 — send outbound live chat and persist channel message. */
@Injectable()
export class ReplyChatService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
    @Inject(CHAT_PROVIDER) private readonly chatProvider: ChatProvider,
  ) {}

  async reply(command: ReplyChatCommand): Promise<ReplyChatResult> {
    const ticketId = command.ticketId.trim();
    const body = command.body.trim();
    const ticket = await this.downstream.getTicket(ticketId);
    if (!ticket) {
      throw new NotFoundException('Ticket not found.');
    }

    let to = command.to?.trim().toLowerCase() ?? '';
    if (!to) {
      const request = await this.store.findRequestByTicketId(ticketId);
      if (request?.email?.includes('@')) {
        to = request.email.trim().toLowerCase();
      }
    }
    if (!to) {
      const messages = await this.store.listMessagesForTicket(ticketId);
      const inbound = [...messages]
        .reverse()
        .find(
          (m) =>
            m.channel === 'LiveChat' &&
            m.direction === 'Inbound' &&
            m.fromEmail?.includes('@'),
        );
      if (inbound?.fromEmail) {
        to = inbound.fromEmail.trim().toLowerCase();
      }
    }
    if (!to) {
      throw new BadRequestException(
        'Could not resolve recipient email for this chat ticket.',
      );
    }

    await this.chatProvider.sendOutbound({
      to,
      body,
      ticketId,
    });

    const messageId = randomUUID();
    await this.store.addMessage({
      id: messageId,
      ticketId,
      channel: 'LiveChat',
      direction: 'Outbound',
      body,
      fromEmail: 'crm-chat',
      createdAt: new Date().toISOString(),
    });

    return { messageId, ticketId, to };
  }
}
