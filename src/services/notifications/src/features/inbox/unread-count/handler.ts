import {
  Injectable,
  UnauthorizedException,
} from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { NotificationsStore } from '../../../infrastructure/database/notifications.store';
import { UnreadCountQuery } from '../inbox.types';
import { requireUserId } from '../list-inbox/schema';

/** SDD CRM-020 — unread count for badge. */
@Injectable()
@QueryHandler(UnreadCountQuery)
export class UnreadCountHandler implements IQueryHandler<UnreadCountQuery> {
  constructor(private readonly store: NotificationsStore) {}

  execute(query: UnreadCountQuery) {
    try {
      const userId = requireUserId(query.userId);
      return Promise.resolve({ count: this.store.unreadCount(userId) });
    } catch (err) {
      throw new UnauthorizedException(
        err instanceof Error ? err.message : 'Unauthorized',
      );
    }
  }
}
