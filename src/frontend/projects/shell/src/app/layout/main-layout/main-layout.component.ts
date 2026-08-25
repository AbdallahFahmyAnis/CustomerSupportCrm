import { Component, HostListener, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import {
  canAccessAdmin,
  canAccessAgentWorkspace,
  LanguageStore,
  SessionApi,
} from 'shared';
import { CrmNotification, NotificationsApi } from '../../core/notifications.api';

type Branding = {
  productTitle: string;
  primaryColor: string;
  logoUrl: string;
  organizationName: string;
};

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
  private readonly notificationsApi = inject(NotificationsApi);
  private readonly http = inject(HttpClient);
  readonly sidebarCollapsed = signal(false);
  readonly inboxOpen = signal(false);
  readonly unreadCount = signal(0);
  readonly notifications = signal<CrmNotification[]>([]);
  /** SDD CRM-044 */
  readonly branding = signal<Branding>({
    productTitle: 'Customer Support CRM',
    primaryColor: '#2563eb',
    logoUrl: '/brand/azm-squad.png',
    organizationName: 'Customer Support CRM',
  });

  ngOnInit(): void {
    this.http.get<Branding>('/api/identity/branding').subscribe({
      next: (row) => {
        this.branding.set(row);
        document.documentElement.style.setProperty('--crm-brand-primary', row.primaryColor || '#2563eb');
      },
      error: () => undefined,
    });
    this.session.load().subscribe({
      next: () => {
        if (this.session.session()) {
          this.refreshNotifications();
        }
      },
    });
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

  toggleInbox(event: Event): void {
    event.stopPropagation();
    const next = !this.inboxOpen();
    this.inboxOpen.set(next);
    if (next) {
      this.refreshNotifications();
    }
  }

  markRead(item: CrmNotification, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    if (item.readAt) {
      return;
    }
    this.notificationsApi.markRead(item.id).subscribe({
      next: (row) => {
        this.notifications.update((list) =>
          list.map((n) => (n.id === row.id ? row : n)),
        );
        this.unreadCount.update((c) => Math.max(0, c - 1));
      },
    });
  }

  @HostListener('document:click')
  closeInbox(): void {
    this.inboxOpen.set(false);
  }

  private refreshNotifications(): void {
    this.notificationsApi.unreadCount().subscribe({
      next: (r) => this.unreadCount.set(r.count),
      error: () => this.unreadCount.set(0),
    });
    this.notificationsApi.list().subscribe({
      next: (rows) => this.notifications.set(rows),
      error: () => this.notifications.set([]),
    });
  }
}
