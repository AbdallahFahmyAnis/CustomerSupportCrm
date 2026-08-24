import { Controller, Get, Param } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { ListTicketMessagesQuery } from '../messages.types';

/** SDD CRM-012 — GET /api/channels/tickets/:ticketId/messages */
@Controller()
export class ListTicketMessagesRoute {
  constructor(private readonly queryBus: QueryBus) {}

  @Get('api/channels/tickets/:ticketId/messages')
  list(@Param('ticketId') ticketId: string) {
    return this.queryBus.execute(new ListTicketMessagesQuery(ticketId));
  }
}
