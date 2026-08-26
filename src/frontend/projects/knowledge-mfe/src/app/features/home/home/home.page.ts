import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LanguageStore } from 'shared';

@Component({
  selector: 'app-knowledge-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class KnowledgeHomePage {
  readonly lang = inject(LanguageStore);
}
