import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { GetHealthRoute } from '../features/health/get-health/route';
import { GetHealthHandler } from '../features/health/get-health/handler';
import { SubmitWebFormRoute } from '../features/intake/submit-web-form/route';
import { SubmitWebFormHandler } from '../features/intake/submit-web-form/handler';
import { SubmitWebFormService } from '../features/intake/submit-web-form/service';
import { IngestEmailRoute } from '../features/intake/ingest-email/route';
import { IngestEmailHandler } from '../features/intake/ingest-email/handler';
import { IngestEmailService } from '../features/intake/ingest-email/service';
import { ListPortalRequestsRoute } from '../features/portal/list-requests/route';
import { ListPortalRequestsHandler } from '../features/portal/list-requests/handler';
import { ListPortalRequestsService } from '../features/portal/list-requests/service';
import { ListTicketMessagesRoute } from '../features/messages/list-ticket-messages/route';
import { ListTicketMessagesHandler } from '../features/messages/list-ticket-messages/handler';
import { ListTicketMessagesService } from '../features/messages/list-ticket-messages/service';
import { ChannelsStore } from '../infrastructure/database/channels.store';
import { DownstreamClient } from '../infrastructure/http/downstream.client';
import { DevEmailProvider } from '../infrastructure/email/dev-email.provider';

@Module({
  imports: [CqrsModule],
  controllers: [
    GetHealthRoute,
    SubmitWebFormRoute,
    IngestEmailRoute,
    ListPortalRequestsRoute,
    ListTicketMessagesRoute,
  ],
  providers: [
    GetHealthHandler,
    SubmitWebFormHandler,
    SubmitWebFormService,
    IngestEmailHandler,
    IngestEmailService,
    ListPortalRequestsHandler,
    ListPortalRequestsService,
    ListTicketMessagesHandler,
    ListTicketMessagesService,
    ChannelsStore,
    DownstreamClient,
    DevEmailProvider,
  ],
})
export class AppModule {}
