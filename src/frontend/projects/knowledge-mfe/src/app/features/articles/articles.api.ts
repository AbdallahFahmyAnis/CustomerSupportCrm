import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ArticleDetail, ArticleSummary, KnowledgeSearchHit } from './articles.models';

/** SDD CRM-021 — knowledge articles via gateway. */
@Injectable({ providedIn: 'root' })
export class ArticlesApi {
  private readonly http = inject(HttpClient);

  search(q = '', locale?: string): Observable<ArticleSummary[]> {
    const qs = new URLSearchParams();
    if (q.trim()) qs.set('q', q.trim());
    if (locale) qs.set('locale', locale);
    const suffix = qs.toString() ? `?${qs.toString()}` : '';
    return this.http.get<ArticleSummary[]>(`/api/knowledge/articles${suffix}`);
  }

  /** SDD CRM-022 — ranked search. */
  rankedSearch(params: {
    q: string;
    kind?: string;
    status?: string;
    publishedOnly?: boolean;
    locale?: string;
  }): Observable<KnowledgeSearchHit[]> {
    const qs = new URLSearchParams();
    qs.set('q', params.q);
    if (params.kind) qs.set('kind', params.kind);
    if (params.status) qs.set('status', params.status);
    if (params.publishedOnly) qs.set('publishedOnly', 'true');
    if (params.locale) qs.set('locale', params.locale);
    return this.http.get<KnowledgeSearchHit[]>(`/api/knowledge/search?${qs.toString()}`);
  }

  get(id: string): Observable<ArticleDetail> {
    return this.http.get<ArticleDetail>(`/api/knowledge/articles/${id}`);
  }

  create(body: {
    title: string;
    body: string;
    kind: string;
    status: string;
    locale?: string;
  }): Observable<ArticleDetail> {
    return this.http.post<ArticleDetail>('/api/knowledge/articles', body);
  }

  update(
    id: string,
    body: { title: string; body: string; kind: string; status: string; locale?: string },
  ): Observable<ArticleDetail> {
    return this.http.put<ArticleDetail>(`/api/knowledge/articles/${id}`, body);
  }
}
