import { Injectable } from '@nestjs/common';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';

/** SDD CRM-012 — read messages for a ticket. */
@Injectable()
export class ListTicketMessagesService {
  constructor(private readonly store: ChannelsStore) {}

  async list(ticketId: string) {
    return this.store.listMessagesForTicket(ticketId);
  }
}
