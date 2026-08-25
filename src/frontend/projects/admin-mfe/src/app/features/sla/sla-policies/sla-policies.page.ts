import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AutoAssignRule } from '../sla.models';
import { SlaStore } from '../sla.store';

/** SDD CRM-017 / CRM-018 / CRM-019 — admin SLA policies, assign rules, escalation. */
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
  ruleDrafts: AutoAssignRule[] = [];
  escalateOnFirstResponseBreach = true;
  escalateOnResolutionBreach = true;
  escalateUrgentAlways = true;
  assignToAgentId = '22222222-2222-2222-2222-222222222222';
  assignToAgentName = 'Lead Agent';

  constructor() {
    effect(() => {
      const rules = this.store.assignRules();
      this.ruleDrafts = rules.map((r) => ({ ...r }));
      const esc = this.store.escalation();
      if (!esc) {
        return;
      }
      this.escalateOnFirstResponseBreach = esc.escalateOnFirstResponseBreach;
      this.escalateOnResolutionBreach = esc.escalateOnResolutionBreach;
      this.escalateUrgentAlways = esc.escalateUrgentAlways;
      this.assignToAgentId = esc.assignToAgentId;
      this.assignToAgentName = esc.assignToAgentName;
    });
  }

  ngOnInit(): void {
    this.store.load();
  }

  draftFor(priority: string, first: number, resolution: number) {
    if (!this.drafts[priority]) {
      this.drafts[priority] = { firstResponseMinutes: first, resolutionMinutes: resolution };
    }
    return this.drafts[priority];
  }

  savePolicy(priority: string): void {
    const draft = this.drafts[priority];
    if (!draft) {
      return;
    }
    this.store.savePolicy(priority, Number(draft.firstResponseMinutes), Number(draft.resolutionMinutes));
  }

  saveRules(): void {
    this.store.saveAssignRules(
      this.ruleDrafts.map((r) => ({
        ...r,
        category: r.category?.trim() || null,
        priority: r.priority?.trim() || null,
      })),
    );
  }

  addRule(): void {
    this.ruleDrafts = [
      ...this.ruleDrafts,
      {
        id: crypto.randomUUID(),
        category: null,
        priority: null,
        agentId: '11111111-1111-1111-1111-111111111111',
        agentName: 'Demo Agent',
        enabled: true,
      },
    ];
  }

  removeRule(id: string): void {
    this.ruleDrafts = this.ruleDrafts.filter((r) => r.id !== id);
  }

  saveEscalation(): void {
    this.store.saveEscalation({
      escalateOnFirstResponseBreach: this.escalateOnFirstResponseBreach,
      escalateOnResolutionBreach: this.escalateOnResolutionBreach,
      escalateUrgentAlways: this.escalateUrgentAlways,
      assignToAgentId: this.assignToAgentId.trim(),
      assignToAgentName: this.assignToAgentName.trim(),
    });
  }
}
