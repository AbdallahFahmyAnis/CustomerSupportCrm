import { Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { SuggestRepliesCommand } from './schema';

/** SDD CRM-024 — POST /api/ai/tickets/:id/suggestions */
@Controller()
export class SuggestRepliesRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/ai/tickets/:id/suggestions')
  suggest(@Param('id') id: string) {
    return this.commandBus.execute(new SuggestRepliesCommand(id));
  }
}
