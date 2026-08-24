import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { SubmitWebFormBody, SubmitWebFormCommand } from '../intake.types';

/** SDD CRM-012 / CRM-027 — POST /api/channels/intake/web-form */
@Controller()
export class SubmitWebFormRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/intake/web-form')
  submit(@Body() body: SubmitWebFormBody) {
    return this.commandBus.execute(
      new SubmitWebFormCommand(
        body?.name ?? '',
        body?.email ?? '',
        body?.subject ?? '',
        body?.message ?? '',
      ),
    );
  }
}
