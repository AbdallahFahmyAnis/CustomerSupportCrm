import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { IngestEmailCommand, IngestEmailResult } from './email.types';
import { validateEmailIngestInput } from './schema';
import { IngestEmailService } from './service';

/** SDD CRM-008 — CQRS handler for email intake. */
@Injectable()
@CommandHandler(IngestEmailCommand)
export class IngestEmailHandler
  implements ICommandHandler<IngestEmailCommand, IngestEmailResult>
{
  constructor(private readonly service: IngestEmailService) {}

  async execute(command: IngestEmailCommand): Promise<IngestEmailResult> {
    const validationError = validateEmailIngestInput({
      from: command.from,
      subject: command.subject,
      body: command.body,
      name: command.name,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.ingest(command);
  }
}
