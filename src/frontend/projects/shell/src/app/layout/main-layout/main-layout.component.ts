import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LanguageStore, SessionApi } from 'shared';

/** Shell chrome — header + main outlet. */
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './main-layout.html',
  styleUrls: ['./main-layout.scss'],
})
export class MainLayoutComponent implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);

  ngOnInit(): void {
    this.session.load().subscribe();
  }

  signOut(): void {
    this.session.logout().subscribe();
  }
}
