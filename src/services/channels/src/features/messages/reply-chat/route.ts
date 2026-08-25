import { Body, Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { ReplyChatBody, ReplyChatCommand } from './reply.types';

/** SDD CRM-010 — POST /api/channels/tickets/:ticketId/messages/chat */
@Controller()
export class ReplyChatRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/tickets/:ticketId/messages/chat')
  reply(@Param('ticketId') ticketId: string, @Body() body: ReplyChatBody) {
    return this.commandBus.execute(
      new ReplyChatCommand(ticketId, body?.body ?? '', body?.to),
    );
  }
}
