import { Injectable } from '@nestjs/common';
import { ChannelsStore } from '../../../infrastructure/database/channels.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { PortalRequestDto } from '../portal.types';

/** SDD CRM-028 — load portal requests and refresh live ticket status. */
@Injectable()
export class ListPortalRequestsService {
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async listByEmail(email: string): Promise<PortalRequestDto[]> {
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
