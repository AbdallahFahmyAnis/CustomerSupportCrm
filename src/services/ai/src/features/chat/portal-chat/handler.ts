import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { chatReply } from '../../../infrastructure/ai/heuristic.provider';
import { ChatSessionStore } from '../../../infrastructure/chat/chat-session.store';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';
import { ChatCommand } from './schema';

/** SDD CRM-026 polish / 043 */
@Injectable()
@CommandHandler(ChatCommand)
export class PortalChatHandler implements ICommandHandler<ChatCommand> {
  constructor(
    private readonly downstream: DownstreamClient,
    private readonly sessions: ChatSessionStore,
  ) {}

  async execute(command: ChatCommand) {
    const message = (command.message || '').trim();
    if (!message) throw new BadRequestException('message is required.');
    const sessionId = this.sessions.ensureSessionId(command.sessionId);
    const prior = this.sessions.getTurns(sessionId);
    const faqs = await this.downstream.listPortalFaqs(message);
    const result = chatReply(message, faqs, prior);
    this.sessions.append(sessionId, message, result.reply);
    return {
      reply: result.reply,
      sources: result.sources,
      sessionId,
    };
  }
}
