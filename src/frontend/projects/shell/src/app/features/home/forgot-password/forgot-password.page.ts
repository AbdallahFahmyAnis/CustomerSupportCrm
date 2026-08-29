import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore } from 'shared';
import { HttpClient } from '@angular/common/http';

/** SDD CRM-046 — request password reset email / dev token. */
@Component({
  selector: 'app-forgot-password-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrls: ['../home/home.scss'],
})
export class ForgotPasswordPage {
  readonly lang = inject(LanguageStore);
  private readonly http = inject(HttpClient);
  private readonly feedback = inject(FormFeedbackStore);

  email = '';
  saving = false;
  done = false;
  /** Local/dev only — shown when Identity returns a token for UAT without email. */
  devResetToken = '';

  submit(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    this.saving = true;
    this.http
      .post<{ message: string; devResetToken?: string }>('/api/identity/forgot-password', {
        email: this.email.trim(),
      })
      .subscribe({
        next: (res) => {
          this.saving = false;
          this.done = true;
          this.devResetToken = res.devResetToken ?? '';
          this.feedback.success('forgotPasswordSent');
        },
        error: () => {
          this.saving = false;
          // Still show generic success to avoid enumeration if misconfigured
          this.done = true;
          this.feedback.success('forgotPasswordSent');
        },
      });
  }
}
