import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { AutoCategorizeHandler } from '../features/categorize/auto-categorize/handler';
import { AutoCategorizeRoute } from '../features/categorize/auto-categorize/route';
import { GetHealthHandler } from '../features/health/get-health/handler';
import { GetHealthRoute } from '../features/health/get-health/route';
import { SuggestRepliesHandler } from '../features/suggestions/suggest-replies/handler';
import { SuggestRepliesRoute } from '../features/suggestions/suggest-replies/route';
import { GenerateSummaryHandler } from '../features/summaries/generate-summary/handler';
import { GenerateSummaryRoute } from '../features/summaries/generate-summary/route';
import { DownstreamClient } from '../infrastructure/http/downstream.client';

/** SDD CRM-023…026 — AI composition root. */
@Module({
  imports: [CqrsModule],
  controllers: [
    GetHealthRoute,
    GenerateSummaryRoute,
    SuggestRepliesRoute,
    AutoCategorizeRoute,
  ],
  providers: [
    DownstreamClient,
    GetHealthHandler,
    GenerateSummaryHandler,
    SuggestRepliesHandler,
    AutoCategorizeHandler,
  ],
})
export class AppModule {}
