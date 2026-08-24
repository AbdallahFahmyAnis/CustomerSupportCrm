import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD 002–003 — agent home links into customers and tickets. */
@Component({
  selector: 'app-agent-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class AgentHomePage {}
