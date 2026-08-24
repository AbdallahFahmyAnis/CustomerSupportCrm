import { BadRequestException, Injectable } from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { ChannelsStore } from '../../../persistence/channels.store';
import { ListTicketMessagesQuery } from './list-ticket-messages.query';

@Injectable()
@QueryHandler(ListTicketMessagesQuery)
export class ListTicketMessagesHandler
  implements IQueryHandler<ListTicketMessagesQuery>
{
  constructor(private readonly store: ChannelsStore) {}

  async execute(query: ListTicketMessagesQuery) {
    const ticketId = query.ticketId?.trim() ?? '';
    if (!ticketId) {
      throw new BadRequestException('ticketId is required.');
    }
    return this.store.listMessagesForTicket(ticketId);
  }
}
