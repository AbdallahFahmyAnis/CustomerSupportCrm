import 'reflect-metadata';
import { NestFactory } from '@nestjs/core';
import { AppModule } from './app/app.module';
import { channelsConfig } from './app/config';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  await app.listen(channelsConfig.port);
}

void bootstrap();
