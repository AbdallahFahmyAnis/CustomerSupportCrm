import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { HealthController } from './health/health.controller';
import { GetHealthHandler } from './health/get-health.handler';
import { SubmitWebFormController } from './features/intake/submit-web-form/submit-web-form.controller';
import { SubmitWebFormHandler } from './features/intake/submit-web-form/submit-web-form.handler';
import { ListPortalRequestsController } from './features/portal/list-requests/list-portal-requests.controller';
import { ListPortalRequestsHandler } from './features/portal/list-requests/list-portal-requests.handler';
import { ListTicketMessagesController } from './features/messages/list-ticket-messages/list-ticket-messages.controller';
import { ListTicketMessagesHandler } from './features/messages/list-ticket-messages/list-ticket-messages.handler';
import { ChannelsStore } from './persistence/channels.store';
import { DownstreamClient } from './persistence/downstream.client';

@Module({
  imports: [CqrsModule],
  controllers: [
    HealthController,
    SubmitWebFormController,
    ListPortalRequestsController,
    ListTicketMessagesController,
  ],
  providers: [
    GetHealthHandler,
    SubmitWebFormHandler,
    ListPortalRequestsHandler,
    ListTicketMessagesHandler,
    ChannelsStore,
    DownstreamClient,
  ],
})
export class AppModule {}
