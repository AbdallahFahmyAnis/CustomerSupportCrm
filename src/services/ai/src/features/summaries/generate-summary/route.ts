import { Controller, Param, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { GenerateSummaryCommand } from './schema';

/** SDD CRM-023 — POST /api/ai/tickets/:id/summary */
@Controller()
export class GenerateSummaryRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/ai/tickets/:id/summary')
  generate(@Param('id') id: string) {
    return this.commandBus.execute(new GenerateSummaryCommand(id));
  }
}
