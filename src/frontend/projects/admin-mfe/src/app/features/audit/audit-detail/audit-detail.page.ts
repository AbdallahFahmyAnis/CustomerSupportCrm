import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LanguageStore, MessageKey } from 'shared';
import { AuditStore } from '../audit.store';

/** SDD CRM-036 / specs/051 — audit event detail. */
@Component({
  selector: 'app-audit-detail-page',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './audit-detail.html',
  styleUrls: ['./audit-detail.scss'],
})
export class AuditDetailPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(AuditStore);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')?.trim() ?? '';
    if (id) {
      this.store.loadDetail(id);
    } else {
      this.store.error.set('auditDetailLoadFailed');
    }
  }

  errorText(): string {
    const key = this.store.error();
    if (!key) {
      return '';
    }
    return this.lang.t(key as MessageKey);
  }
}
