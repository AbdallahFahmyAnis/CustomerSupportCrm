import { Controller, Get, Query } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { ListPortalRequestsQuery } from '../portal.types';

/** SDD CRM-028 — GET /api/channels/portal/requests?email= */
@Controller()
export class ListPortalRequestsRoute {
  constructor(private readonly queryBus: QueryBus) {}

  @Get('api/channels/portal/requests')
  list(@Query('email') email: string) {
    return this.queryBus.execute(new ListPortalRequestsQuery(email));
  }
}
