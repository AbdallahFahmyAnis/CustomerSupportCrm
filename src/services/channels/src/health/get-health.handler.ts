import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { GetHealthQuery } from './get-health.query';

@QueryHandler(GetHealthQuery)
export class GetHealthHandler implements IQueryHandler<GetHealthQuery> {
  execute(): Promise<{ service: string; status: string }> {
    return Promise.resolve({ service: 'channels', status: 'ok' });
  }
}
