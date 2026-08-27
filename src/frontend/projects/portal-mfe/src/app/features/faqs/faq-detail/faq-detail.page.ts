import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { LanguageStore } from 'shared';
import { FaqsStore } from '../faqs.store';

/** SDD CRM-029 — portal FAQ detail. */
@Component({
  selector: 'app-faq-detail-page',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './faq-detail.html',
  styleUrls: ['./faq-detail.scss'],
})
export class FaqDetailPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(FaqsStore);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.store.loadDetail(id);
    }
  }
}
