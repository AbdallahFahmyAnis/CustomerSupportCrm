import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlaStore } from '../sla.store';

/** SDD CRM-017 — admin SLA policy targets. */
@Component({
  selector: 'app-sla-policies-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './sla-policies.html',
  styleUrls: ['./sla-policies.scss'],
})
export class SlaPoliciesPage implements OnInit {
  readonly store = inject(SlaStore);

  drafts: Record<string, { firstResponseMinutes: number; resolutionMinutes: number }> = {};

  ngOnInit(): void {
    this.store.load();
  }

  draftFor(priority: string, first: number, resolution: number) {
    if (!this.drafts[priority]) {
      this.drafts[priority] = { firstResponseMinutes: first, resolutionMinutes: resolution };
    }
    return this.drafts[priority];
  }

  save(priority: string): void {
    const draft = this.drafts[priority];
    if (!draft) {
      return;
    }
    this.store.save(priority, Number(draft.firstResponseMinutes), Number(draft.resolutionMinutes));
  }
}
