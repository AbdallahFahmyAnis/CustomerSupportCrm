import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  canAccessAdmin,
  canAccessAgentWorkspace,
  FormFeedbackStore,
  homePathForRole,
  isCustomerRole,
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
  private readonly feedback = inject(FormFeedbackStore);
  private readonly router = inject(Router);
  email = 'customer@crm.local';
  password = 'Crm!123';

  get showAgent(): boolean {
    return canAccessAgentWorkspace(this.session.session()?.role);
  }

  get showAdmin(): boolean {
    return canAccessAdmin(this.session.session()?.role);
  }

  get showPortal(): boolean {
    return isCustomerRole(this.session.session()?.role);
  }

  useDemo(kind: 'customer' | 'agent' | 'admin'): void {
    if (kind === 'customer') {
      this.email = 'customer@crm.local';
    } else if (kind === 'admin') {
      this.email = 'admin@crm.local';
    } else {
      this.email = 'agent@crm.local';
    }
    this.password = 'Crm!123';
  }

  signIn(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    this.session.login(this.email, this.password).subscribe({
      next: (s) => {
        this.feedback.success('signInSuccess');
        void this.router.navigateByUrl(homePathForRole(s.role));
      },
      error: () => this.feedback.error('signInFailed'),
    });
  }
}
