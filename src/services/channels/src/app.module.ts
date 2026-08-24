import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { HealthController } from './health/health.controller';
import { GetHealthHandler } from './health/get-health.handler';

@Module({
  imports: [CqrsModule],
  controllers: [HealthController],
  providers: [GetHealthHandler],
})
export class AppModule {}
