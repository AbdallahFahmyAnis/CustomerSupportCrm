import {
  Injectable,
  UnauthorizedException,
} from '@nestjs/common';
import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { NotificationsStore } from '../../../infrastructure/database/notifications.store';
import { ListInboxQuery } from '../inbox.types';
import { requireUserId } from './schema';

/** SDD CRM-020 — list inbox for current user. */
@Injectable()
@QueryHandler(ListInboxQuery)
export class ListInboxHandler implements IQueryHandler<ListInboxQuery> {
  constructor(private readonly store: NotificationsStore) {}

  execute(query: ListInboxQuery) {
    try {
      const userId = requireUserId(query.userId);
      return Promise.resolve(this.store.listForUser(userId));
    } catch (err) {
      throw new UnauthorizedException(
        err instanceof Error ? err.message : 'Unauthorized',
      );
    }
  }
}
