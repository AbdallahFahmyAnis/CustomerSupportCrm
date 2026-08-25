import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import {
  canAccessAdmin,
  canAccessAgentWorkspace,
  LanguageStore,
  SessionApi,
} from 'shared';

/** Shell chrome — Materio-like vertical sidebar + content. */
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.html',
  styleUrls: ['./main-layout.scss'],
})
export class MainLayoutComponent implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);
  readonly sidebarCollapsed = signal(false);

  ngOnInit(): void {
    this.session.load().subscribe();
  }

  get showAgentNav(): boolean {
    return canAccessAgentWorkspace(this.session.session()?.role);
  }

  get showAdminNav(): boolean {
    return canAccessAdmin(this.session.session()?.role);
  }

  toggleSidebar(): void {
    this.sidebarCollapsed.update((v) => !v);
  }

  userInitial(name: string): string {
    return (name?.trim()?.charAt(0) || '?').toUpperCase();
  }

  signOut(): void {
    this.session.logout().subscribe();
  }
}
