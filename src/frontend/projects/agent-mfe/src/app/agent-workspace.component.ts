import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD 002–003 — agent home links into customers and tickets. */
@Component({
  selector: 'app-agent-workspace',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './agent-workspace.component.html',
  styleUrls: ['./agent-workspace.component.scss'],
})
export class AgentWorkspaceComponent {}
