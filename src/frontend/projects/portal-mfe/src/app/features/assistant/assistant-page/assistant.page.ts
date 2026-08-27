import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { LanguageStore } from 'shared';
import { AssistantStore } from '../assistant.store';

/** SDD CRM-026 — portal AI Q&A assistant. */
@Component({
  selector: 'app-assistant-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './assistant.html',
  styleUrls: ['./assistant.scss'],
})
export class AssistantPage {
  readonly lang = inject(LanguageStore);
  readonly store = inject(AssistantStore);
  draft = '';

  submit(): void {
    const text = this.draft.trim();
    if (!text) {
      return;
    }
    this.draft = '';
    this.store.ask(text);
  }

  askHint(text: string): void {
    this.draft = text;
    this.submit();
  }
}
