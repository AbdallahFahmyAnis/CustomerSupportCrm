import { Component, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { LanguageStore } from 'shared';
import { FaqsStore } from '../faqs.store';

/** SDD CRM-029 — portal FAQ list (locale from shell language). */
@Component({
  selector: 'app-faq-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './faq-list.html',
  styleUrls: ['./faq-list.scss'],
})
export class FaqListPage {
  readonly lang = inject(LanguageStore);
  readonly store = inject(FaqsStore);
  q = '';

  constructor() {
    effect(() => {
      const locale = this.lang.lang();
      this.store.load(this.q, locale);
    });
  }

  search(): void {
    this.store.load(this.q, this.lang.lang());
  }
}
