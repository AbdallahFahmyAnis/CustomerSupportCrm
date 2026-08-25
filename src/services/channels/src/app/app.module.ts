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
import { IngestWhatsAppRoute } from '../features/intake/ingest-whatsapp/route';
import { IngestWhatsAppHandler } from '../features/intake/ingest-whatsapp/handler';
import { IngestWhatsAppService } from '../features/intake/ingest-whatsapp/service';
import { ListPortalRequestsRoute } from '../features/portal/list-requests/route';
import { ListPortalRequestsHandler } from '../features/portal/list-requests/handler';
import { ListPortalRequestsService } from '../features/portal/list-requests/service';
import { ListTicketMessagesRoute } from '../features/messages/list-ticket-messages/route';
import { ListTicketMessagesHandler } from '../features/messages/list-ticket-messages/handler';
import { ListTicketMessagesService } from '../features/messages/list-ticket-messages/service';
import { ReplyEmailRoute } from '../features/messages/reply-email/route';
import { ReplyEmailHandler } from '../features/messages/reply-email/handler';
import { ReplyEmailService } from '../features/messages/reply-email/service';
import { ReplyWhatsAppRoute } from '../features/messages/reply-whatsapp/route';
import { ReplyWhatsAppHandler } from '../features/messages/reply-whatsapp/handler';
import { ReplyWhatsAppService } from '../features/messages/reply-whatsapp/service';
import { ChannelsStore } from '../infrastructure/database/channels.store';
import { DownstreamClient } from '../infrastructure/http/downstream.client';
import { DevEmailProvider } from '../infrastructure/email/dev-email.provider';
import { SmtpEmailProvider } from '../infrastructure/email/smtp-email.provider';
import { EMAIL_PROVIDER } from '../infrastructure/email/email-provider';
import { DevWhatsAppProvider } from '../infrastructure/whatsapp/dev-whatsapp.provider';
import { WHATSAPP_PROVIDER } from '../infrastructure/whatsapp/whatsapp-provider';
import { channelsConfig } from './config';

@Module({
  imports: [CqrsModule],
  controllers: [
    GetHealthRoute,
    SubmitWebFormRoute,
    IngestEmailRoute,
    IngestWhatsAppRoute,
    ListPortalRequestsRoute,
    ListTicketMessagesRoute,
    ReplyEmailRoute,
    ReplyWhatsAppRoute,
  ],
  providers: [
    GetHealthHandler,
    SubmitWebFormHandler,
    SubmitWebFormService,
    IngestEmailHandler,
    IngestEmailService,
    IngestWhatsAppHandler,
    IngestWhatsAppService,
    ListPortalRequestsHandler,
    ListPortalRequestsService,
    ListTicketMessagesHandler,
    ListTicketMessagesService,
    ReplyEmailHandler,
    ReplyEmailService,
    ReplyWhatsAppHandler,
    ReplyWhatsAppService,
    ChannelsStore,
    DownstreamClient,
    DevEmailProvider,
    SmtpEmailProvider,
    DevWhatsAppProvider,
    {
      provide: EMAIL_PROVIDER,
      useFactory: (dev: DevEmailProvider, smtp: SmtpEmailProvider) =>
        channelsConfig.smtpHost ? smtp : dev,
      inject: [DevEmailProvider, SmtpEmailProvider],
    },
    {
      provide: WHATSAPP_PROVIDER,
      useExisting: DevWhatsAppProvider,
    },
  ],
})
export class AppModule {}
