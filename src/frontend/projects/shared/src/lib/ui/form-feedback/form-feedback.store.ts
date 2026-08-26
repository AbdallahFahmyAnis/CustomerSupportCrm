import { Injectable, inject, signal } from '@angular/core';
import { MessageKey } from '../../i18n/index';
import { LanguageStore } from '../../language.store';

export type FormFeedbackKind = 'success' | 'error';

/** Shared success/fail dialog for forms across shell + MFEs. */
@Injectable({ providedIn: 'root' })
export class FormFeedbackStore {
  private readonly lang = inject(LanguageStore);

  readonly open = signal(false);
  readonly kind = signal<FormFeedbackKind>('success');
  readonly title = signal('');
  readonly message = signal('');

  success(messageKey: MessageKey = 'successGeneric', titleKey: MessageKey = 'successTitle'): void {
    this.show('success', titleKey, messageKey);
  }

  error(messageKey: MessageKey = 'failGeneric', titleKey: MessageKey = 'failTitle'): void {
    this.show('error', titleKey, messageKey);
  }

  successText(message: string, titleKey: MessageKey = 'successTitle'): void {
    this.kind.set('success');
    this.title.set(this.lang.t(titleKey));
    this.message.set(message);
    this.open.set(true);
  }

  errorText(message: string, titleKey: MessageKey = 'failTitle'): void {
    this.kind.set('error');
    this.title.set(this.lang.t(titleKey));
    this.message.set(message || this.lang.t('failGeneric'));
    this.open.set(true);
  }

  close(): void {
    this.open.set(false);
  }

  private show(kind: FormFeedbackKind, titleKey: MessageKey, messageKey: MessageKey): void {
    this.kind.set(kind);
    this.title.set(this.lang.t(titleKey));
    this.message.set(this.lang.t(messageKey));
    this.open.set(true);
  }
}
