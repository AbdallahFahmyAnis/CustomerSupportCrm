import { Controller, Get, Headers } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { ListInboxQuery } from '../inbox.types';

/** SDD CRM-020 — GET /api/notifications */
@Controller()
export class ListInboxRoute {
  constructor(private readonly queryBus: QueryBus) {}

  @Get('api/notifications')
  list(@Headers('x-crm-user-id') userId?: string) {
    return this.queryBus.execute(new ListInboxQuery(userId ?? ''));
  }
}
