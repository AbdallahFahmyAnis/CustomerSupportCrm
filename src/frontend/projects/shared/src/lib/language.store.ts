import { Injectable, signal } from '@angular/core';
import { AR, EN, Lang, MessageKey } from './i18n/index';

const STORAGE_KEY = 'crm.lang';
const EVENT_NAME = 'crm-lang-changed';

/** SDD CRM-041 — shared across shell + MFEs via localStorage + window event. */
@Injectable({ providedIn: 'root' })
export class LanguageStore {
  readonly lang = signal<Lang>(this.read());

  constructor() {
    this.apply(this.lang());
    if (typeof window !== 'undefined') {
      window.addEventListener(EVENT_NAME, ((e: Event) => {
        const next = (e as CustomEvent<Lang>).detail;
        if (next === 'en' || next === 'ar') {
          this.lang.set(next);
          this.apply(next);
        }
      }) as EventListener);
      window.addEventListener('storage', (e) => {
        if (e.key !== STORAGE_KEY) return;
        const next: Lang = e.newValue === 'ar' ? 'ar' : 'en';
        this.lang.set(next);
        this.apply(next);
      });
    }
  }

  t(key: MessageKey): string {
    // Read signal so templates re-render when language changes.
    const map = this.lang() === 'ar' ? AR : EN;
    return map[key];
  }

  toggle(): void {
    const next: Lang = this.lang() === 'en' ? 'ar' : 'en';
    this.setLang(next);
  }

  setLang(next: Lang): void {
    this.lang.set(next);
    localStorage.setItem(STORAGE_KEY, next);
    this.apply(next);
    window.dispatchEvent(new CustomEvent(EVENT_NAME, { detail: next }));
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
