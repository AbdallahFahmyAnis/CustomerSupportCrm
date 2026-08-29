import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FormFeedbackStore, homePathForRole, LanguageStore, SessionApi } from 'shared';

/** SDD CRM-045 — customer self-registration. */
@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['../home/home.scss'],
})
export class RegisterPage {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly router = inject(Router);

  displayName = '';
  email = '';
  password = '';
  confirmPassword = '';
  saving = false;

  submit(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    if (this.password !== this.confirmPassword) {
      this.feedback.error('passwordMismatch');
      return;
    }
    this.saving = true;
    this.session
      .register({
        email: this.email.trim(),
        displayName: this.displayName.trim(),
        password: this.password,
      })
      .subscribe({
        next: (s) => {
          this.saving = false;
          this.feedback.success('registerSuccess');
          void this.router.navigateByUrl(homePathForRole(s.role) || '/portal');
        },
        error: (err) => {
          this.saving = false;
          const msg = err?.error?.error;
          this.feedback.errorText(
            typeof msg === 'string' && msg.trim() ? msg : this.lang.t('registerFailed'),
          );
        },
      });
  }
}
