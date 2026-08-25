import { Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';

/**
 * Materio-inspired calendar date field (app-calendar chrome on native date input).
 * Original styles only — not ThemeSelection assets.
 */
@Component({
  selector: 'crm-date-field',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './date-field.html',
  styleUrls: ['./date-field.scss'],
})
export class CrmDateFieldComponent {
  readonly label = input('');
  readonly value = model('');
  readonly name = input('date');
  readonly min = input<string | null>(null);
  readonly max = input<string | null>(null);
}
