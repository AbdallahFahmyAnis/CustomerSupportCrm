import { Injectable, signal } from '@angular/core';
import { AR, EN, Lang, MessageKey } from './i18n';

const STORAGE_KEY = 'crm.lang';

@Injectable({ providedIn: 'root' })
export class LanguageStore {
  readonly lang = signal<Lang>(this.read());

  constructor() {
    this.apply(this.lang());
  }

  t(key: MessageKey): string {
    return this.lang() === 'ar' ? AR[key] : EN[key];
  }

  toggle(): void {
    const next: Lang = this.lang() === 'en' ? 'ar' : 'en';
    this.lang.set(next);
    localStorage.setItem(STORAGE_KEY, next);
    this.apply(next);
  }

  private read(): Lang {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'ar' ? 'ar' : 'en';
  }

  private apply(lang: Lang): void {
    document.documentElement.lang = lang;
    document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
  }
}
