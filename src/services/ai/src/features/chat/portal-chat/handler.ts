import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { chatReply } from '../../../infrastructure/ai/heuristic.provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { ChatCommand } from './schema';

/** SDD CRM-026 */
@Injectable()
@CommandHandler(ChatCommand)
export class PortalChatHandler implements ICommandHandler<ChatCommand> {
  constructor(private readonly downstream: DownstreamClient) {}

  async execute(command: ChatCommand) {
    const message = (command.message || '').trim();
    if (!message) throw new BadRequestException('message is required.');
    const faqs = await this.downstream.listPortalFaqs(message);
    const result = chatReply(message, faqs);
    return {
      reply: result.reply,
      sources: result.sources,
      sessionId: command.sessionId || null,
    };
  }
}
