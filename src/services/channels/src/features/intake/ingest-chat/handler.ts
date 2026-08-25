import { BadRequestException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { validateChatIngestInput } from './schema';
import { IngestChatService } from './service';
import { IngestChatCommand, IngestChatResult } from './chat.types';

/** SDD CRM-010 — CQRS handler for live chat intake. */
@CommandHandler(IngestChatCommand)
export class IngestChatHandler
  implements ICommandHandler<IngestChatCommand, IngestChatResult>
{
  constructor(private readonly service: IngestChatService) {}

  async execute(command: IngestChatCommand): Promise<IngestChatResult> {
    const validationError = validateChatIngestInput({
      email: command.email,
      body: command.body,
      name: command.name,
      subject: command.subject,
      ticketId: command.ticketId,
      sessionId: command.sessionId,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.ingest(command);
  }
}
