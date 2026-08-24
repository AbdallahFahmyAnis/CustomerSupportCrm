import { BadRequestException, Injectable } from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { ListTicketMessagesQuery } from '../messages.types';
import { requireTicketId } from './schema';
import { ListTicketMessagesService } from './service';

/** SDD CRM-012 — CQRS handler for ticket messages. */
@Injectable()
@QueryHandler(ListTicketMessagesQuery)
export class ListTicketMessagesHandler
  implements IQueryHandler<ListTicketMessagesQuery>
{
  constructor(private readonly service: ListTicketMessagesService) {}

  async execute(query: ListTicketMessagesQuery) {
    try {
      const ticketId = requireTicketId(query.ticketId);
      return await this.service.list(ticketId);
    } catch (err) {
      throw new BadRequestException(
        err instanceof Error ? err.message : 'Invalid ticket id.',
      );
    }
  }
}
