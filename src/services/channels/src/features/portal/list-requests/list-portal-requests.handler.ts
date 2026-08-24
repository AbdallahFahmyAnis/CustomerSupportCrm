import { BadRequestException, Injectable } from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { ChannelsStore } from '../../../persistence/channels.store';
import { DownstreamClient } from '../../../persistence/downstream.client';
import {
  ListPortalRequestsQuery,
  PortalRequestDto,
} from './list-portal-requests.query';

/** SDD CRM-028 — track requests for an email (refresh status when possible). */
@Injectable()
@QueryHandler(ListPortalRequestsQuery)
export class ListPortalRequestsHandler
  implements IQueryHandler<ListPortalRequestsQuery, PortalRequestDto[]>
{
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async execute(query: ListPortalRequestsQuery): Promise<PortalRequestDto[]> {
    const email = query.email?.trim().toLowerCase() ?? '';
    if (!email) {
      throw new BadRequestException('email query parameter is required.');
    }

    const rows = this.store.listRequestsByEmail(email);
    const result: PortalRequestDto[] = [];
    for (const row of rows) {
      const live = await this.downstream.getTicketStatus(row.ticketId);
      result.push({
        requestId: row.id,
        ticketId: row.ticketId,
        ticketNumber: row.ticketNumber,
        subject: row.subject,
        status: live ?? row.status,
        createdAt: row.createdAt,
      });
    }
    return result;
  }
}
