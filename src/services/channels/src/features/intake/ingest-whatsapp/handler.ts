import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { validateWhatsAppIngestInput } from './schema';
import { IngestWhatsAppService } from './service';
import { IngestWhatsAppCommand, IngestWhatsAppResult } from './whatsapp.types';

/** SDD CRM-009 — CQRS handler for WhatsApp intake. */
@Injectable()
@CommandHandler(IngestWhatsAppCommand)
export class IngestWhatsAppHandler
  implements ICommandHandler<IngestWhatsAppCommand, IngestWhatsAppResult>
{
  constructor(private readonly service: IngestWhatsAppService) {}

  async execute(command: IngestWhatsAppCommand): Promise<IngestWhatsAppResult> {
    const validationError = validateWhatsAppIngestInput({
      from: command.from,
      body: command.body,
      name: command.name,
      subject: command.subject,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.ingest(command);
  }
}
