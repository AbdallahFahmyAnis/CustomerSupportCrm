import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { GetHealthRoute } from '../features/health/get-health/route';
import { GetHealthHandler } from '../features/health/get-health/handler';

@Module({
  imports: [CqrsModule],
  controllers: [GetHealthRoute],
  providers: [GetHealthHandler],
})
export class AppModule {}
