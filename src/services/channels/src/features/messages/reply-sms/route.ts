import { Body, Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { ReplySmsBody, ReplySmsCommand } from './reply.types';

/** SDD CRM-011 — POST /api/channels/tickets/:ticketId/messages/sms */
@Controller()
export class ReplySmsRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/tickets/:ticketId/messages/sms')
  reply(@Param('ticketId') ticketId: string, @Body() body: ReplySmsBody) {
    return this.commandBus.execute(
      new ReplySmsCommand(ticketId, body?.body ?? '', body?.to),
    );
  }
}
