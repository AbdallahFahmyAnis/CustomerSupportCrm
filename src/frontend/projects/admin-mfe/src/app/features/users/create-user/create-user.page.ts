import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  CrmWizardComponent,
  CrmWizardStep,
  CrmWizardStepDirective,
  FormFeedbackStore,
  LanguageStore,
} from 'shared';
import { DepartmentsApi } from '../../departments/departments.api';
import { Branch, Department } from '../../departments/departments.models';
import { UsersStore } from '../users.store';

/** SDD CRM-035 / CRM-043 — create user wizard. */
@Component({
  selector: 'app-create-user-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmWizardComponent, CrmWizardStepDirective],
  templateUrl: './create-user.html',
  styleUrls: ['./create-user.scss'],
})
export class UserCreatePage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(UsersStore);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly router = inject(Router);
  private readonly departmentsApi = inject(DepartmentsApi);

  step = 0;
  attempted = false;
  email = '';
  displayName = '';
  password = 'Crm!123';
  role = 'Agent';
  departmentId = '';
  branchId = '';
  departments: Department[] = [];
  branches: Branch[] = [];

  readonly steps = computed<CrmWizardStep[]>(() => [
    { title: this.lang.t('stepAccount'), subtitle: this.lang.t('accountAccess') },
    { title: this.lang.t('stepReview'), subtitle: this.lang.t('confirmCreateAccount') },
  ]);

  get avatarLetter(): string {
    return (this.displayName.trim() || this.email.trim() || '?').charAt(0).toUpperCase();
  }

  get filteredBranches(): Branch[] {
    return this.branches.filter((b) => !this.departmentId || b.departmentId === this.departmentId);
  }

  ngOnInit(): void {
    this.store.loadRoles();
    this.departmentsApi.listDepartments().subscribe({
      next: (rows) => (this.departments = rows ?? []),
      error: () => undefined,
    });
    this.departmentsApi.listBranches().subscribe({
      next: (rows) => (this.branches = rows ?? []),
      error: () => undefined,
    });
  }

  canAdvance(): boolean {
    return (
      !!this.email.trim() &&
      !!this.displayName.trim() &&
      !!this.password &&
      !!this.role
    );
  }

  onAdvanceBlocked(): void {
    this.attempted = true;
    this.feedback.error('formInvalid');
  }

  onDeptChange(): void {
    this.branchId = '';
  }

  submit(): void {
    this.attempted = true;
    if (!this.canAdvance()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.create(
      {
        email: this.email.trim(),
        displayName: this.displayName.trim(),
        password: this.password,
        role: this.role,
        departmentId: this.departmentId || null,
        branchId: this.branchId || null,
      },
      () => {
        this.feedback.success('createUserSuccess');
        void this.router.navigate(['/admin/users']);
      },
      (msg) => this.feedback.errorText(msg),
    );
  }
}
