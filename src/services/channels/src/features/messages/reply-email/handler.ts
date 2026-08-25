import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { ReplyEmailCommand, ReplyEmailResult } from './reply.types';
import { validateReplyEmailInput } from './schema';
import { ReplyEmailService } from './service';

/** SDD CRM-040 — CQRS handler for outbound email reply. */
@Injectable()
@CommandHandler(ReplyEmailCommand)
export class ReplyEmailHandler
  implements ICommandHandler<ReplyEmailCommand, ReplyEmailResult>
{
  constructor(private readonly service: ReplyEmailService) {}

  async execute(command: ReplyEmailCommand): Promise<ReplyEmailResult> {
    const validationError = validateReplyEmailInput(command.ticketId, {
      body: command.body,
      to: command.to,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.reply(command);
  }
}
