import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { ReplyWhatsAppCommand, ReplyWhatsAppResult } from './reply.types';
import { validateReplyWhatsAppInput } from './schema';
import { ReplyWhatsAppService } from './service';

/** SDD CRM-009 — CQRS handler for outbound WhatsApp reply. */
@Injectable()
@CommandHandler(ReplyWhatsAppCommand)
export class ReplyWhatsAppHandler
  implements ICommandHandler<ReplyWhatsAppCommand, ReplyWhatsAppResult>
{
  constructor(private readonly service: ReplyWhatsAppService) {}

  async execute(command: ReplyWhatsAppCommand): Promise<ReplyWhatsAppResult> {
    const validationError = validateReplyWhatsAppInput(command.ticketId, {
      body: command.body,
      to: command.to,
    });
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.reply(command);
  }
}
