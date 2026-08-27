import { DatePipe } from '@angular/common';
import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore } from 'shared';
import { ARTICLE_KINDS, ARTICLE_STATUSES } from '../articles.models';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-021 — create / edit article. */
@Component({
  selector: 'app-article-edit-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './article-edit.html',
  styleUrls: ['./article-edit.scss'],
})
export class ArticleEditPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(ArticlesStore);
  private readonly feedback = inject(FormFeedbackStore);
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

  save(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    const payload = {
      title: this.title.trim(),
      body: this.body.trim(),
      kind: this.kind,
      status: this.status,
    };
    if (this.isNew) {
      this.store.create(
        payload,
        (id) => {
          this.feedback.success('articleSaveSuccess');
          void this.router.navigateByUrl(`/knowledge/${id}`);
        },
        (msg) => this.feedback.errorText(msg),
      );
      return;
    }
    this.store.update(
      this.id,
      payload,
      () => this.feedback.success('articleSaveSuccess'),
      (msg) => this.feedback.errorText(msg),
    );
  }
}
