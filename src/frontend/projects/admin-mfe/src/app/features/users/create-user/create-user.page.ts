import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CrmWizardComponent, CrmWizardStep, CrmWizardStepDirective } from 'shared';
import { UsersStore } from '../users.store';

/** SDD CRM-035 — create user wizard. */
@Component({
  selector: 'app-create-user-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmWizardComponent, CrmWizardStepDirective],
  templateUrl: './create-user.html',
  styleUrls: ['./create-user.scss'],
})
export class UserCreatePage implements OnInit {
  readonly store = inject(UsersStore);
  private readonly router = inject(Router);

  step = 0;
  email = '';
  displayName = '';
  password = 'Crm!123';
  role = 'Agent';

  readonly steps: CrmWizardStep[] = [
    { title: 'Account', subtitle: 'Email and name' },
    { title: 'Access', subtitle: 'Password and role' },
    { title: 'Review', subtitle: 'Confirm and create' },
  ];

  ngOnInit(): void {
    this.store.loadRoles();
  }

  canAdvance(): boolean {
    if (this.step === 0) {
      return !!this.email.trim() && !!this.displayName.trim();
    }
    if (this.step === 1) {
      return !!this.password && !!this.role;
    }
    return true;
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
