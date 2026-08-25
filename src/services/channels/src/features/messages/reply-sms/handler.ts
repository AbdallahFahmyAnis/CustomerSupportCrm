import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { ReplySmsCommand, ReplySmsResult } from './reply.types';
import { validateReplySmsInput } from './schema';
import { ReplySmsService } from './service';

/** SDD CRM-011 — CQRS handler for outbound SMS reply. */
@Injectable()
@CommandHandler(ReplySmsCommand)
export class ReplySmsHandler
  implements ICommandHandler<ReplySmsCommand, ReplySmsResult>
{
  constructor(private readonly service: ReplySmsService) {}

  async execute(command: ReplySmsCommand): Promise<ReplySmsResult> {
    const validationError = validateReplySmsInput(command.ticketId, {
      body: command.body,
      to: command.to,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.reply(command);
  }
}
