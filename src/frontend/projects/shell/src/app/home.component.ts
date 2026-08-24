import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LanguageStore, SessionApi } from 'shared';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="home">
      <p>{{ lang.t('homeLead') }}</p>
      @if (!session.session()) {
        <form (ngSubmit)="signIn()">
          <label>
            {{ lang.t('email') }}
            <input name="email" [(ngModel)]="email" autocomplete="username" />
          </label>
          <label>
            {{ lang.t('password') }}
            <input name="password" type="password" [(ngModel)]="password" autocomplete="current-password" />
          </label>
          <button type="submit">{{ lang.t('signIn') }}</button>
          @if (error) {
            <p class="error">{{ error }}</p>
          }
        </form>
      }
    </section>
  `,
})
export class HomeComponent {
  readonly lang = inject(LanguageStore);
  readonly session = inject(SessionApi);
  email = 'agent@crm.local';
  password = 'Crm!123';
  error = '';

  signIn(): void {
    this.error = '';
    this.session.login(this.email, this.password).subscribe({
      error: () => (this.error = 'Sign-in failed'),
    });
  }
}
