import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { LanguageStore, MessageKey } from 'shared';
import { UsersApi } from '../../users/users.api';
import { UserSummary } from '../../users/users.models';
import { AutoAssignRule } from '../sla.models';
import { SlaStore } from '../sla.store';

const ASSIGNABLE_ROLES = new Set(['Agent', 'Lead', 'Admin']);

/** SDD CRM-017 / CRM-018 / CRM-019 — admin SLA policies, assign rules, escalation. */
@Component({
  selector: 'app-sla-policies-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './sla-policies.html',
  styleUrls: ['./sla-policies.scss'],
})
export class SlaPoliciesPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(SlaStore);
  private readonly usersApi = inject(UsersApi);

  drafts: Record<string, { firstResponseMinutes: number; resolutionMinutes: number }> = {};
  ruleDrafts: AutoAssignRule[] = [];
  agents: UserSummary[] = [];
  readonly categories = ['Billing', 'Technical', 'General'] as const;
  readonly priorities = ['Low', 'Medium', 'High', 'Urgent'] as const;
  escalateOnFirstResponseBreach = true;
  escalateOnResolutionBreach = true;
  escalateUrgentAlways = true;
  assignToAgentId = '22222222-2222-2222-2222-222222222222';
  assignToAgentName = 'Lead Agent';

  constructor() {
    effect(() => {
      const rules = this.store.assignRules();
      this.ruleDrafts = rules.map((r) => ({
        ...r,
        category: r.category ?? '',
        priority: r.priority ?? '',
      }));
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
    this.usersApi.search('').subscribe({
      next: (rows) => {
        this.agents = rows.filter((u) => u.isActive && ASSIGNABLE_ROLES.has(u.role));
      },
      error: () => {
        this.agents = [];
      },
    });
  }

  enabledRuleCount(): number {
    return this.ruleDrafts.filter((r) => r.enabled).length;
  }

  agentOptionsFor(selectedId: string, selectedName: string): UserSummary[] {
    if (!selectedId) {
      return this.agents;
    }
    if (this.agents.some((a) => a.id === selectedId)) {
      return this.agents;
    }
    return [
      {
        id: selectedId,
        email: '',
        displayName: selectedName || selectedId,
        role: 'Agent',
        isActive: true,
      },
      ...this.agents,
    ];
  }

  agentLabel(u: UserSummary): string {
    return u.email ? `${u.displayName} (${u.role})` : u.displayName;
  }

  onRuleAgentChange(rule: AutoAssignRule, agentId: string): void {
    const agent = this.agentOptionsFor(rule.agentId, rule.agentName).find((a) => a.id === agentId);
    rule.agentId = agentId;
    rule.agentName = agent?.displayName ?? rule.agentName;
  }

  onEscalationAgentChange(agentId: string): void {
    const agent = this.agentOptionsFor(this.assignToAgentId, this.assignToAgentName).find(
      (a) => a.id === agentId,
    );
    this.assignToAgentId = agentId;
    this.assignToAgentName = agent?.displayName ?? this.assignToAgentName;
  }

  priorityLabel(priority: string): string {
    const key = ({
      Low: 'priorityLow',
      Medium: 'priorityMedium',
      High: 'priorityHigh',
      Urgent: 'priorityUrgent',
    } as Record<string, MessageKey>)[priority];
    return key ? this.lang.t(key) : priority;
  }

  categoryLabel(category: string): string {
    const key = ({
      Billing: 'categoryBilling',
      Technical: 'categoryTechnical',
      General: 'categoryGeneral',
    } as Record<string, MessageKey>)[category];
    return key ? this.lang.t(key) : category;
  }

  categoryOptionsFor(selected: string | null | undefined): string[] {
    const value = (selected ?? '').trim();
    if (value && !this.categories.includes(value as (typeof this.categories)[number])) {
      return [value, ...this.categories];
    }
    return [...this.categories];
  }

  priorityOptionsFor(selected: string | null | undefined): string[] {
    const value = (selected ?? '').trim();
    if (value && !this.priorities.includes(value as (typeof this.priorities)[number])) {
      return [value, ...this.priorities];
    }
    return [...this.priorities];
  }

  priorityTone(priority: string): string {
    const p = priority.toLowerCase();
    if (p === 'urgent') return 'urgent';
    if (p === 'high') return 'high';
    if (p === 'medium') return 'medium';
    return 'low';
  }

  errorText(): string {
    const key = this.store.error();
    if (!key) {
      return '';
    }
    const known: MessageKey[] = [
      'slaLoadFailed',
      'saveFailed',
      'slaRulesSaveFailed',
      'slaEscalationSaveFailed',
    ];
    return known.includes(key as MessageKey) ? this.lang.t(key as MessageKey) : key;
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
    const fallback = this.agents[0];
    this.ruleDrafts = [
      ...this.ruleDrafts,
      {
        id: crypto.randomUUID(),
        category: null,
        priority: null,
        agentId: fallback?.id ?? '11111111-1111-1111-1111-111111111111',
        agentName: fallback?.displayName ?? 'Demo Agent',
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
