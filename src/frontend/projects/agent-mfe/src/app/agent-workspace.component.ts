import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD 002-customer-profiles — agent home links into customers. */
@Component({
  selector: 'app-agent-workspace',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="workspace">
      <h1>Agent workspace</h1>
      <p>Work customer records and tickets from here.</p>
      <div class="actions">
        <a routerLink="/agent/customers" class="btn">Open customers</a>
        <a routerLink="/agent/tickets" class="btn">Open tickets</a>
      </div>
    </section>
  `,
  styles: `
    .workspace { padding: 1.5rem; max-width: 40rem; }
    .actions { display: flex; flex-wrap: wrap; gap: 0.75rem; margin-top: 0.75rem; }
    .btn {
      display: inline-block;
      background: #2563eb;
      color: #fff;
      text-decoration: none;
      border-radius: 0.375rem;
      padding: 0.5rem 0.9rem;
    }
  `,
})
export class AgentWorkspaceComponent {}
