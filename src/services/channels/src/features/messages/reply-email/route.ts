import { Body, Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { ReplyEmailBody, ReplyEmailCommand } from './reply.types';

/** SDD CRM-040 — POST /api/channels/tickets/:ticketId/messages/email */
@Controller()
export class ReplyEmailRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/tickets/:ticketId/messages/email')
  reply(@Param('ticketId') ticketId: string, @Body() body: ReplyEmailBody) {
    return this.commandBus.execute(
      new ReplyEmailCommand(ticketId, body?.body ?? '', body?.to),
    );
  }
}
