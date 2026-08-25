import { Body, Controller, Post } from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { ChatCommand } from './schema';

/** SDD CRM-026 — POST /api/ai/chat */
@Controller()
export class PortalChatRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/ai/chat')
  chat(@Body() body: { message?: string; sessionId?: string }) {
    return this.commandBus.execute(new ChatCommand(body?.message ?? '', body?.sessionId));
  }
}
