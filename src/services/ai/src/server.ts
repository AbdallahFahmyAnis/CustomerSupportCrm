import 'reflect-metadata';
import { NestFactory } from '@nestjs/core';
import { AppModule } from './app/app.module';
import { aiConfig } from './app/config';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  await app.listen(aiConfig.port);
}

void bootstrap();
