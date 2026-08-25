import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { CreateNotificationCommand } from '../inbox.types';

/** SDD CRM-016 — POST /api/notifications */
@Controller()
export class CreateNotificationRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/notifications')
  create(
    @Body()
    body: {
      userId?: string;
      title?: string;
      body?: string;
      kind?: string;
      href?: string;
    },
  ) {
    return this.commandBus.execute(
      new CreateNotificationCommand(
        body.userId ?? '',
        body.title ?? '',
        body.body ?? '',
        body.kind ?? 'system',
        body.href,
      ),
    );
  }
}
