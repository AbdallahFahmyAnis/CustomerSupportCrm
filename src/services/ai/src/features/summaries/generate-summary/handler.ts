import { Injectable, NotFoundException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { summarizeTicket } from '../../../infrastructure/ai/heuristic.provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { GenerateSummaryCommand } from './schema';

/** SDD CRM-023 */
@Injectable()
@CommandHandler(GenerateSummaryCommand)
export class GenerateSummaryHandler implements ICommandHandler<GenerateSummaryCommand> {
  constructor(private readonly downstream: DownstreamClient) {}

  async execute(command: GenerateSummaryCommand) {
    const ticket = await this.downstream.getTicket(command.ticketId);
    if (!ticket) throw new NotFoundException('Ticket not found.');
    const result = summarizeTicket(ticket);
    return { ticketId: ticket.id, ...result };
  }
}
