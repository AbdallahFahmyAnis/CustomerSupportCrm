import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  canAccessAdmin,
  canAccessAgentWorkspace,
  homePathForRole,
  LanguageStore,
  SessionApi,
} from 'shared';

/** Shell home / sign-in. */
@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class HomePage {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);
  private readonly router = inject(Router);
  email = 'agent@crm.local';
  password = 'Crm!123';
  error = '';

  get showAgent(): boolean {
    return canAccessAgentWorkspace(this.session.session()?.role);
  }

  get showAdmin(): boolean {
    return canAccessAdmin(this.session.session()?.role);
  }

  signIn(): void {
    this.error = '';
    this.session.login(this.email, this.password).subscribe({
      next: (s) => {
        void this.router.navigateByUrl(homePathForRole(s.role));
      },
      error: () => (this.error = 'Sign-in failed'),
    });
  }
}
