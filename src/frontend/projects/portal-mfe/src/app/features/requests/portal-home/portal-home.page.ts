import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LanguageStore } from 'shared';

/** SDD CRM-027 / CRM-028 / CRM-029 — portal landing. */
@Component({
  selector: 'app-portal-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './portal-home.html',
  styleUrls: ['./portal-home.scss'],
})
export class PortalHomePage {
  readonly lang = inject(LanguageStore);
}
