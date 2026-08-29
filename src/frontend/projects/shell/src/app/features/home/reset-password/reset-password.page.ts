import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore } from 'shared';
import { HttpClient } from '@angular/common/http';

/** SDD CRM-046 — set a new password with reset token. */
@Component({
  selector: 'app-reset-password-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './reset-password.html',
  styleUrls: ['../home/home.scss'],
})
export class ResetPasswordPage implements OnInit {
  readonly lang = inject(LanguageStore);
  private readonly http = inject(HttpClient);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  email = '';
  token = '';
  password = '';
  confirmPassword = '';
  saving = false;

  ngOnInit(): void {
    const q = this.route.snapshot.queryParamMap;
    this.email = q.get('email') ?? '';
    this.token = q.get('token') ?? '';
  }

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
    this.http
      .post('/api/identity/reset-password', {
        email: this.email.trim(),
        token: this.token.trim(),
        newPassword: this.password,
      })
      .subscribe({
        next: () => {
          this.saving = false;
          this.feedback.success('resetPasswordSuccess');
          void this.router.navigateByUrl('/login');
        },
        error: (err) => {
          this.saving = false;
          const msg = err?.error?.error;
          this.feedback.errorText(
            typeof msg === 'string' && msg.trim() ? msg : this.lang.t('resetPasswordFailed'),
          );
        },
      });
  }
}
