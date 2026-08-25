import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { DevChatProvider } from '../../../infrastructure/chat/dev-chat.provider';
import { IngestChatBody, IngestChatCommand } from './chat.types';

/** SDD CRM-010 — POST /api/channels/intake/chat */
@Controller()
export class IngestChatRoute {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly chatProvider: DevChatProvider,
  ) {}

  @Post('api/channels/intake/chat')
  ingest(@Body() body: IngestChatBody) {
    const parsed = this.chatProvider.parseInbound(body);
    return this.commandBus.execute(
      new IngestChatCommand(
        parsed.email,
        parsed.body,
        parsed.name,
        parsed.subject,
        parsed.ticketId,
        parsed.sessionId,
      ),
    );
  }
}
