import 'reflect-metadata';
import { NestFactory } from '@nestjs/core';
import { AppModule } from './app/app.module';
import {
  channelsConfig,
  resolveEmailProviderKind,
  resolveSmsProviderKind,
  resolveWhatsAppProviderKind,
} from './app/config';

async function bootstrap() {
  const app = await NestFactory.create(AppModule, { bodyParser: true });
  console.log(
    `[channels] Email=${resolveEmailProviderKind()} SMS=${resolveSmsProviderKind()} WhatsApp=${resolveWhatsAppProviderKind()} publicUrl=${channelsConfig.publicUrl}`,
  );
  await app.listen(channelsConfig.port);
}

void bootstrap();
