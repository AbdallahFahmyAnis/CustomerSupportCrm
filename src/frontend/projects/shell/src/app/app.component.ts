import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LanguageStore, SessionApi } from 'shared';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);

  ngOnInit(): void {
    this.session.load().subscribe();
  }

  signOut(): void {
    this.session.logout().subscribe();
  }
}
