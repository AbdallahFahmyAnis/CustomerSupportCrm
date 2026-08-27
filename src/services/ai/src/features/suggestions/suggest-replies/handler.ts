import { Injectable, NotFoundException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { suggestReplies } from '../../../infrastructure/ai/heuristic.provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { SuggestRepliesCommand } from './schema';

/** SDD CRM-024 */
@Injectable()
@CommandHandler(SuggestRepliesCommand)
export class SuggestRepliesHandler implements ICommandHandler<SuggestRepliesCommand> {
  constructor(private readonly downstream: DownstreamClient) {}

  async execute(command: SuggestRepliesCommand) {
    const ticket = await this.downstream.getTicket(command.ticketId);
    if (!ticket) throw new NotFoundException('Ticket not found.');
    return suggestReplies(ticket);
  }
}
