import { Module, OnModuleInit } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { GetHealthRoute } from '../features/health/get-health/route';
import { GetHealthHandler } from '../features/health/get-health/handler';
import { ListInboxRoute } from '../features/inbox/list-inbox/route';
import { ListInboxHandler } from '../features/inbox/list-inbox/handler';
import { UnreadCountRoute } from '../features/inbox/unread-count/route';
import { UnreadCountHandler } from '../features/inbox/unread-count/handler';
import { MarkReadRoute } from '../features/inbox/mark-read/route';
import { MarkReadHandler } from '../features/inbox/mark-read/handler';
import { CreateNotificationRoute } from '../features/inbox/create-notification/route';
import { CreateNotificationHandler } from '../features/inbox/create-notification/handler';
import { NotificationsStore } from '../infrastructure/database/notifications.store';

/** SDD CRM-020 / CRM-016 — notifications composition root. */
@Module({
  imports: [CqrsModule],
  controllers: [
    GetHealthRoute,
    ListInboxRoute,
    UnreadCountRoute,
    MarkReadRoute,
    CreateNotificationRoute,
  ],
  providers: [
    NotificationsStore,
    GetHealthHandler,
    ListInboxHandler,
    UnreadCountHandler,
    MarkReadHandler,
    CreateNotificationHandler,
  ],
})
export class AppModule implements OnModuleInit {
  constructor(private readonly store: NotificationsStore) {}

  onModuleInit(): void {
    this.store.ensureSeeded();
  }
}
