import { BadRequestException } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { CreateNotificationCommand } from '../inbox.types';
import { NotificationsStore } from '../../../infrastructure/database/notifications.store';
import { NotificationKind } from '../../../domain/notification';

const KINDS: NotificationKind[] = ['assignment', 'sla', 'system', 'mention'];

/** SDD CRM-016 — create inbox notification. */
@CommandHandler(CreateNotificationCommand)
export class CreateNotificationHandler
  implements ICommandHandler<CreateNotificationCommand>
{
  constructor(private readonly store: NotificationsStore) {}

  execute(command: CreateNotificationCommand) {
    if (!command.userId?.trim()) {
      throw new BadRequestException('userId is required');
    }
    if (!command.title?.trim() || !command.body?.trim()) {
      throw new BadRequestException('title and body are required');
    }
    const kind = (command.kind?.trim() || 'system') as NotificationKind;
    if (!KINDS.includes(kind)) {
      throw new BadRequestException(`kind must be one of: ${KINDS.join(', ')}`);
    }
    return this.store.create({
      userId: command.userId,
      title: command.title,
      body: command.body,
      kind,
      href: command.href,
    });
  }
}
