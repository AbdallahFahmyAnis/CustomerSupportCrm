/** SDD CRM-021 — knowledge article models. */
export interface ArticleSummary {
  id: string;
  title: string;
  kind: string;
  status: string;
  updatedAt: string;
}

export interface ArticleDetail extends ArticleSummary {
  body: string;
  createdBy: string;
  createdAt: string;
}

export const ARTICLE_KINDS = ['Faq', 'Article', 'Solution', 'Guide'] as const;
export const ARTICLE_STATUSES = ['Draft', 'Published'] as const;

/** SDD CRM-022 — ranked search hit. */
export interface KnowledgeSearchHit {
  id: string;
  title: string;
  kind: string;
  status: string;
  score: number;
  snippet: string;
  updatedAt: string;
}
