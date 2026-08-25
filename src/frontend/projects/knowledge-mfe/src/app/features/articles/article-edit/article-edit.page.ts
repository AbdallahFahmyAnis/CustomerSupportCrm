import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ARTICLE_KINDS, ARTICLE_STATUSES } from '../articles.models';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-021 — create / edit article. */
@Component({
  selector: 'app-article-edit-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './article-edit.html',
  styleUrls: ['./article-edit.scss'],
})
export class ArticleEditPage implements OnInit {
  readonly store = inject(ArticlesStore);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly kinds = ARTICLE_KINDS;
  readonly statuses = ARTICLE_STATUSES;

  id = '';
  title = '';
  body = '';
  kind = 'Faq';
  status = 'Draft';
  isNew = true;

  constructor() {
    effect(() => {
      const row = this.store.selected();
      if (!row || this.isNew || row.id !== this.id) {
        return;
      }
      this.title = row.title;
      this.body = row.body;
      this.kind = row.kind;
      this.status = row.status;
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    this.isNew = !this.id || this.route.snapshot.routeConfig?.path === 'new';
    if (!this.isNew) {
      this.store.loadDetail(this.id);
    }
  }

  save(): void {
    const payload = {
      title: this.title.trim(),
      body: this.body.trim(),
      kind: this.kind,
      status: this.status,
    };
    if (this.isNew) {
      this.store.create(payload, (id) => void this.router.navigateByUrl(`/knowledge/${id}`));
      return;
    }
    this.store.update(this.id, payload, () => undefined);
  }
}
