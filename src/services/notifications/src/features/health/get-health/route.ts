import { Controller, Get } from '@nestjs/common';
import { QueryBus } from '@nestjs/cqrs';
import { GetHealthQuery } from './schema';

@Controller()
export class GetHealthRoute {
  constructor(private readonly queryBus: QueryBus) {}

  @Get('health')
  getHealth() {
    return this.queryBus.execute(new GetHealthQuery());
  }

  @Get('api/notifications/health')
  getPrefixedHealth() {
    return this.queryBus.execute(new GetHealthQuery());
  }
}
