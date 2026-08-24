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
      <p>Work customer records and (soon) tickets from here.</p>
      <a routerLink="/agent/customers" class="btn">Open customers</a>
    </section>
  `,
  styles: `
    .workspace { padding: 1.5rem; max-width: 40rem; }
    .btn {
      display: inline-block;
      margin-top: 0.75rem;
      background: #2563eb;
      color: #fff;
      text-decoration: none;
      border-radius: 0.375rem;
      padding: 0.5rem 0.9rem;
    }
  `,
})
export class AgentWorkspaceComponent {}
