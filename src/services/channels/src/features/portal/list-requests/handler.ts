import { BadRequestException, Injectable } from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { ListPortalRequestsQuery, PortalRequestDto } from '../portal.types';
import { requireEmail } from './schema';
import { ListPortalRequestsService } from './service';

/** SDD CRM-028 — CQRS handler for portal request list. */
@Injectable()
@QueryHandler(ListPortalRequestsQuery)
export class ListPortalRequestsHandler
  implements IQueryHandler<ListPortalRequestsQuery, PortalRequestDto[]>
{
  constructor(private readonly service: ListPortalRequestsService) {}

  async execute(query: ListPortalRequestsQuery): Promise<PortalRequestDto[]> {
    try {
      const email = requireEmail(query.email);
      return this.service.listByEmail(email);
    } catch (err) {
      throw new BadRequestException(
        err instanceof Error ? err.message : 'Invalid email.',
      );
    }
  }
}
