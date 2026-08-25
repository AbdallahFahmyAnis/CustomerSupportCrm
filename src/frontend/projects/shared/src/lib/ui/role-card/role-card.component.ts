import { Component, input, output } from '@angular/core';

/** Materio-inspired role card (app-access-roles shape). */
@Component({
  selector: 'crm-role-card',
  standalone: true,
  templateUrl: './role-card.html',
  styleUrls: ['./role-card.scss'],
})
export class CrmRoleCardComponent {
  readonly name = input.required<string>();
  readonly description = input('');
  readonly permissions = input<readonly string[]>([]);
  readonly userCountLabel = input('');
  readonly edit = output<void>();
}
