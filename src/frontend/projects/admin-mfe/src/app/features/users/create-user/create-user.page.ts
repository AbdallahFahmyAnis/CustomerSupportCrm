import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UsersStore } from '../users.store';

/** SDD CRM-035 — create user smart page. */
@Component({
  selector: 'app-create-user-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './create-user.html',
  styleUrls: ['./create-user.scss'],
})
export class UserCreatePage implements OnInit {
  readonly store = inject(UsersStore);
  private readonly router = inject(Router);

  email = '';
  displayName = '';
  password = 'Crm!123';
  role = 'Agent';

  ngOnInit(): void {
    this.store.loadRoles();
  }

  submit(): void {
    if (!this.email.trim() || !this.displayName.trim() || !this.password) return;
    this.store.create(
      {
        email: this.email.trim(),
        displayName: this.displayName.trim(),
        password: this.password,
        role: this.role,
      },
      () => void this.router.navigate(['/admin/users']),
    );
  }
}
