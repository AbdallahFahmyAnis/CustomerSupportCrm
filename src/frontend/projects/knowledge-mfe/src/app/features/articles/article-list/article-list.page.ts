import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ArticlesStore } from '../articles.store';

/** SDD CRM-021 — article list. */
@Component({
  selector: 'app-article-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './article-list.html',
  styleUrls: ['./article-list.scss'],
})
export class ArticleListPage implements OnInit {
  readonly store = inject(ArticlesStore);
  q = '';

  ngOnInit(): void {
    this.store.load();
  }

  search(): void {
    this.store.load(this.q);
  }
}
