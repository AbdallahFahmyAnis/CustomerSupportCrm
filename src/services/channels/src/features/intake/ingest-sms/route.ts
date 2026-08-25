import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { DevSmsProvider } from '../../../infrastructure/sms/dev-sms.provider';
import { IngestSmsBody, IngestSmsCommand } from './sms.types';

/** SDD CRM-011 / CRM-040 — POST /api/channels/intake/sms */
@Controller()
export class IngestSmsRoute {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly smsProvider: DevSmsProvider,
  ) {}

  @Post('api/channels/intake/sms')
  ingest(@Body() body: IngestSmsBody) {
    const parsed = this.smsProvider.parseInbound(body);
    return this.commandBus.execute(
      new IngestSmsCommand(
        parsed.from,
        parsed.body,
        parsed.name,
        parsed.subject,
      ),
    );
  }
}
