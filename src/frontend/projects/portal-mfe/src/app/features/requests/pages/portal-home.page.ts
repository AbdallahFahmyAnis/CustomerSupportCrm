import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD CRM-027 / CRM-028 — portal landing. */
@Component({
  selector: 'app-portal-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './portal-home.page.html',
  styleUrls: ['./portal-home.page.scss'],
})
export class PortalHomePage {}
