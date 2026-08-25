import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FaqsStore } from '../faqs.store';

/** SDD CRM-029 — portal FAQ list. */
@Component({
  selector: 'app-faq-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './faq-list.html',
  styleUrls: ['./faq-list.scss'],
})
export class FaqListPage implements OnInit {
  readonly store = inject(FaqsStore);
  q = '';

  ngOnInit(): void {
    this.store.load();
  }

  search(): void {
    this.store.load(this.q);
  }
}
