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
import { IngestChatRoute } from '../features/intake/ingest-chat/route';
import { IngestChatHandler } from '../features/intake/ingest-chat/handler';
import { IngestChatService } from '../features/intake/ingest-chat/service';
import { IngestSmsRoute } from '../features/intake/ingest-sms/route';
import { IngestSmsHandler } from '../features/intake/ingest-sms/handler';
import { IngestSmsService } from '../features/intake/ingest-sms/service';
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
import { ReplyChatRoute } from '../features/messages/reply-chat/route';
import { ReplyChatHandler } from '../features/messages/reply-chat/handler';
import { ReplyChatService } from '../features/messages/reply-chat/service';
import { ReplySmsRoute } from '../features/messages/reply-sms/route';
import { ReplySmsHandler } from '../features/messages/reply-sms/handler';
import { ReplySmsService } from '../features/messages/reply-sms/service';
import { ChannelsStore } from '../infrastructure/database/channels.store';
import { DownstreamClient } from '../infrastructure/http/downstream.client';
import { DevEmailProvider } from '../infrastructure/email/dev-email.provider';
import { SmtpEmailProvider } from '../infrastructure/email/smtp-email.provider';
import { EMAIL_PROVIDER } from '../infrastructure/email/email-provider';
import { DevWhatsAppProvider } from '../infrastructure/whatsapp/dev-whatsapp.provider';
import { WHATSAPP_PROVIDER } from '../infrastructure/whatsapp/whatsapp-provider';
import { DevChatProvider } from '../infrastructure/chat/dev-chat.provider';
import { CHAT_PROVIDER } from '../infrastructure/chat/chat-provider';
import { DevSmsProvider } from '../infrastructure/sms/dev-sms.provider';
import { SMS_PROVIDER } from '../infrastructure/sms/sms-provider';
import { channelsConfig } from './config';

@Module({
  imports: [CqrsModule],
  controllers: [
    GetHealthRoute,
    SubmitWebFormRoute,
    IngestEmailRoute,
    IngestWhatsAppRoute,
    IngestChatRoute,
    IngestSmsRoute,
    ListPortalRequestsRoute,
    ListTicketMessagesRoute,
    ReplyEmailRoute,
    ReplyWhatsAppRoute,
    ReplyChatRoute,
    ReplySmsRoute,
  ],
  providers: [
    GetHealthHandler,
    SubmitWebFormHandler,
    SubmitWebFormService,
    IngestEmailHandler,
    IngestEmailService,
    IngestWhatsAppHandler,
    IngestWhatsAppService,
    IngestChatHandler,
    IngestChatService,
    IngestSmsHandler,
    IngestSmsService,
    ListPortalRequestsHandler,
    ListPortalRequestsService,
    ListTicketMessagesHandler,
    ListTicketMessagesService,
    ReplyEmailHandler,
    ReplyEmailService,
    ReplyWhatsAppHandler,
    ReplyWhatsAppService,
    ReplyChatHandler,
    ReplyChatService,
    ReplySmsHandler,
    ReplySmsService,
    ChannelsStore,
    DownstreamClient,
    DevEmailProvider,
    SmtpEmailProvider,
    DevWhatsAppProvider,
    DevChatProvider,
    DevSmsProvider,
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
    {
      provide: CHAT_PROVIDER,
      useExisting: DevChatProvider,
    },
    {
      provide: SMS_PROVIDER,
      useExisting: DevSmsProvider,
    },
  ],
})
export class AppModule {}
