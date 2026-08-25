import {
  BadRequestException,
  Controller,
  Headers,
  Post,
  Req,
  UnauthorizedException,
} from '@nestjs/common';
import { CommandBus } from '@nestjs/cqrs';
import { Request } from 'express';
import { channelsConfig } from '../../../app/config';
import { IngestWhatsAppCommand } from '../../intake/ingest-whatsapp/whatsapp.types';
import { flattenFormBody, parseTwilioInboundForm } from '../../../infrastructure/twilio/parse-twilio-inbound';
import {
  buildTwilioWebhookUrl,
  validateTwilioSignature,
} from '../../../infrastructure/twilio/validate-twilio-signature';

/** SDD CRM-040 — POST /api/channels/webhooks/twilio/whatsapp */
@Controller()
export class TwilioWhatsAppWebhookRoute {
  constructor(private readonly commandBus: CommandBus) {}

  @Post('api/channels/webhooks/twilio/whatsapp')
  async ingest(
    @Req() req: Request,
    @Headers('x-twilio-signature') signature?: string,
  ) {
    const params = flattenFormBody(req.body);
    const url = buildTwilioWebhookUrl(
      channelsConfig.publicUrl,
      '/api/channels/webhooks/twilio/whatsapp',
    );
    const check = validateTwilioSignature({
      authToken: channelsConfig.twilioAuthToken,
      url,
      params,
      signature,
    });
    if (!check.ok) {
      throw new UnauthorizedException('Invalid Twilio signature.');
    }

    const parsed = parseTwilioInboundForm(params);
    if (!parsed.from || !parsed.body) {
      throw new BadRequestException('From and Body are required.');
    }

    return this.commandBus.execute(
      new IngestWhatsAppCommand(parsed.from, parsed.body),
    );
  }
}
