import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LanguageStore, SessionApi } from 'shared';

/** Shell home / sign-in. */
@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class HomePage {
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
