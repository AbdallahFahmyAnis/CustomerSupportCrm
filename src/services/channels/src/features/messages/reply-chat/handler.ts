import { BadRequestException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { ReplyChatCommand, ReplyChatResult } from './reply.types';
import { validateReplyChatInput } from './schema';
import { ReplyChatService } from './service';

/** SDD CRM-010 — CQRS handler for outbound live chat reply. */
@CommandHandler(ReplyChatCommand)
export class ReplyChatHandler
  implements ICommandHandler<ReplyChatCommand, ReplyChatResult>
{
  constructor(private readonly service: ReplyChatService) {}

  async execute(command: ReplyChatCommand): Promise<ReplyChatResult> {
    const validationError = validateReplyChatInput(command.ticketId, {
      body: command.body,
      to: command.to,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.reply(command);
  }
}
