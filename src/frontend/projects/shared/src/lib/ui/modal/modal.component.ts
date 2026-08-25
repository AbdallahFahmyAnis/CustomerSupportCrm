import { Component, input, model, output } from '@angular/core';

/**
 * Materio-inspired modal dialog (modal-examples shape).
 * Original styles only — not ThemeSelection assets.
 */
@Component({
  selector: 'crm-modal',
  standalone: true,
  templateUrl: './modal.html',
  styleUrls: ['./modal.scss'],
})
export class CrmModalComponent {
  readonly open = model(false);
  readonly title = input('Dialog');
  readonly size = input<'sm' | 'md' | 'lg'>('md');
  readonly closed = output<void>();

  close(): void {
    this.open.set(false);
    this.closed.emit();
  }

  onBackdrop(event: MouseEvent): void {
    if (event.target === event.currentTarget) {
      this.close();
    }
  }
}
