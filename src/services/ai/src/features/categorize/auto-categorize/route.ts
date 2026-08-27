import { Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { AutoCategorizeCommand } from './schema';

/** SDD CRM-025 — POST /api/ai/tickets/:id/categorize */
@Controller()
export class AutoCategorizeRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/ai/tickets/:id/categorize')
  categorize(@Param('id') id: string) {
    return this.commandBus.execute(new AutoCategorizeCommand(id));
  }
}
