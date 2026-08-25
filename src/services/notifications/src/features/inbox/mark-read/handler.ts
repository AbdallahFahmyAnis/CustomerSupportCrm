import {
  Injectable,
  NotFoundException,
  UnauthorizedException,
} from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { NotificationsStore } from '../../../infrastructure/database/notifications.store';
import { MarkReadCommand } from '../inbox.types';
import { requireUserId } from '../list-inbox/schema';

/** SDD CRM-020 — mark notification read. */
@Injectable()
@CommandHandler(MarkReadCommand)
export class MarkReadHandler implements ICommandHandler<MarkReadCommand> {
  constructor(private readonly store: NotificationsStore) {}

  execute(command: MarkReadCommand) {
    try {
      const userId = requireUserId(command.userId);
      const row = this.store.markRead(userId, command.id);
      if (!row) {
        throw new NotFoundException('Notification not found.');
      }
      return Promise.resolve(row);
    } catch (err) {
      if (err instanceof NotFoundException) {
        throw err;
      }
      throw new UnauthorizedException(
        err instanceof Error ? err.message : 'Unauthorized',
      );
    }
  }
}
