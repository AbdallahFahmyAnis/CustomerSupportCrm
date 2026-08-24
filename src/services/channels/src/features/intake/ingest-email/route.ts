import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { DevEmailProvider } from '../../../infrastructure/email/dev-email.provider';
import { IngestEmailBody, IngestEmailCommand } from './email.types';

/** SDD CRM-008 / CRM-040 — POST /api/channels/intake/email */
@Controller()
export class IngestEmailRoute {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly emailProvider: DevEmailProvider,
  ) {}

  @Post('api/channels/intake/email')
  ingest(@Body() body: IngestEmailBody) {
    const parsed = this.emailProvider.parseInbound(body);
    return this.commandBus.execute(
      new IngestEmailCommand(
        parsed.from,
        parsed.subject,
        parsed.body,
        parsed.name,
      ),
    );
  }
}
