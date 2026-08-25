import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { DevWhatsAppProvider } from '../../../infrastructure/whatsapp/dev-whatsapp.provider';
import { IngestWhatsAppBody, IngestWhatsAppCommand } from './whatsapp.types';

/** SDD CRM-009 / CRM-040 — POST /api/channels/intake/whatsapp */
@Controller()
export class IngestWhatsAppRoute {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly whatsAppProvider: DevWhatsAppProvider,
  ) {}

  @Post('api/channels/intake/whatsapp')
  ingest(@Body() body: IngestWhatsAppBody) {
    const parsed = this.whatsAppProvider.parseInbound(body);
    return this.commandBus.execute(
      new IngestWhatsAppCommand(
        parsed.from,
        parsed.body,
        parsed.name,
        parsed.subject,
      ),
    );
  }
}
