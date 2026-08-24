import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LanguageStore, SessionApi } from 'shared';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
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
