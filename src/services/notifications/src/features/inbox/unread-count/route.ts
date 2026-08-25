import { Controller, Get, Headers } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { UnreadCountQuery } from '../inbox.types';

/** SDD CRM-020 — GET /api/notifications/unread-count */
@Controller()
export class UnreadCountRoute {
  constructor(private readonly queryBus: QueryBus) {}

  @Get('api/notifications/unread-count')
  count(@Headers('x-crm-user-id') userId?: string) {
    return this.queryBus.execute(new UnreadCountQuery(userId ?? ''));
  }
}
