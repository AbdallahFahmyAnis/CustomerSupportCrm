import { Routes } from '@angular/router';

export const ARTICLES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./article-list/article-list.page').then((m) => m.ArticleListPage),
  },
  {
    path: 'search',
    loadComponent: () =>
      import('./article-search/article-search.page').then((m) => m.ArticleSearchPage),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./article-edit/article-edit.page').then((m) => m.ArticleEditPage),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./article-edit/article-edit.page').then((m) => m.ArticleEditPage),
  },
];
