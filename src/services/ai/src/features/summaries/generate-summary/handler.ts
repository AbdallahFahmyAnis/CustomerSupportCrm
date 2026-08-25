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
    try {
      await this.downstream.saveAiSummary(ticket.id, result.summary, result.highlights);
    } catch {
      // Best-effort persist; still return generated summary to the agent.
    }
    return { ticketId: ticket.id, ...result };
  }
}
