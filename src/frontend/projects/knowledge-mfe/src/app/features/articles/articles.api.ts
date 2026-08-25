import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ArticleDetail, ArticleSummary } from './articles.models';

/** SDD CRM-021 — knowledge articles via gateway. */
@Injectable({ providedIn: 'root' })
export class ArticlesApi {
  private readonly http = inject(HttpClient);

  search(q = ''): Observable<ArticleSummary[]> {
    const qs = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    return this.http.get<ArticleSummary[]>(`/api/knowledge/articles${qs}`);
  }

  get(id: string): Observable<ArticleDetail> {
    return this.http.get<ArticleDetail>(`/api/knowledge/articles/${id}`);
  }

  create(body: {
    title: string;
    body: string;
    kind: string;
    status: string;
  }): Observable<ArticleDetail> {
    return this.http.post<ArticleDetail>('/api/knowledge/articles', body);
  }

  update(
    id: string,
    body: { title: string; body: string; kind: string; status: string },
  ): Observable<ArticleDetail> {
    return this.http.put<ArticleDetail>(`/api/knowledge/articles/${id}`, body);
  }
}
