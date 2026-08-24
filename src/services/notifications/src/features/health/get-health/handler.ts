import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { GetHealthQuery } from './schema';

@QueryHandler(GetHealthQuery)
export class GetHealthHandler implements IQueryHandler<GetHealthQuery> {
  execute(): Promise<{ service: string; status: string }> {
    return Promise.resolve({ service: 'notifications', status: 'ok' });
  }
}
