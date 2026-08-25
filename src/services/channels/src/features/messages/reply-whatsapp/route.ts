import { Body, Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { ReplyWhatsAppBody, ReplyWhatsAppCommand } from './reply.types';

/** SDD CRM-009 — POST /api/channels/tickets/:ticketId/messages/whatsapp */
@Controller()
export class ReplyWhatsAppRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/tickets/:ticketId/messages/whatsapp')
  reply(@Param('ticketId') ticketId: string, @Body() body: ReplyWhatsAppBody) {
    return this.commandBus.execute(
      new ReplyWhatsAppCommand(ticketId, body?.body ?? '', body?.to),
    );
  }
}
