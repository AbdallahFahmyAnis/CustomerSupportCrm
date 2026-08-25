import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { validateSmsIngestInput } from './schema';
import { IngestSmsService } from './service';
import { IngestSmsCommand, IngestSmsResult } from './sms.types';

/** SDD CRM-011 — CQRS handler for SMS intake. */
@Injectable()
@CommandHandler(IngestSmsCommand)
export class IngestSmsHandler
  implements ICommandHandler<IngestSmsCommand, IngestSmsResult>
{
  constructor(private readonly service: IngestSmsService) {}

  async execute(command: IngestSmsCommand): Promise<IngestSmsResult> {
    const validationError = validateSmsIngestInput({
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
