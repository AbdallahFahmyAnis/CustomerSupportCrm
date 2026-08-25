import { Controller, Headers, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { MarkReadCommand } from '../inbox.types';

/** SDD CRM-020 — POST /api/notifications/:id/read */
@Controller()
export class MarkReadRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/notifications/:id/read')
  mark(
    @Param('id') id: string,
    @Headers('x-crm-user-id') userId?: string,
  ) {
    return this.commandBus.execute(new MarkReadCommand(userId ?? '', id));
  }
}
