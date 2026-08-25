import { Injectable, NotFoundException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { categorizeTicket } from '../../../infrastructure/ai/heuristic.provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { AutoCategorizeCommand } from './schema';

/** SDD CRM-025 */
@Injectable()
@CommandHandler(AutoCategorizeCommand)
export class AutoCategorizeHandler implements ICommandHandler<AutoCategorizeCommand> {
  constructor(private readonly downstream: DownstreamClient) {}

  async execute(command: AutoCategorizeCommand) {
    const ticket = await this.downstream.getTicket(command.ticketId);
    if (!ticket) throw new NotFoundException('Ticket not found.');
    return categorizeTicket(ticket);
  }
}
