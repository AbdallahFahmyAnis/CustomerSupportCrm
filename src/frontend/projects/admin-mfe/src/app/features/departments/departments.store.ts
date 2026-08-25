import { Injectable, inject, signal } from '@angular/core';
import { Branch, Department } from './departments.models';
import { DepartmentsApi } from './departments.api';

/** SDD CRM-043 */
@Injectable({ providedIn: 'root' })
export class DepartmentsStore {
  private readonly api = inject(DepartmentsApi);

  readonly departments = signal<Department[]>([]);
  readonly branches = signal<Branch[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');

  refresh(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.listDepartments().subscribe({
      next: (rows) => {
        this.departments.set(rows ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load departments.');
        this.loading.set(false);
      },
    });
    this.api.listBranches().subscribe({
      next: (rows) => this.branches.set(rows ?? []),
      error: () => undefined,
    });
  }

  createDepartment(name: string): void {
    this.api.createDepartment(name).subscribe({
      next: () => this.refresh(),
      error: (err) => this.error.set(err?.error?.error ?? 'Create department failed.'),
    });
  }

  createBranch(departmentId: string, name: string): void {
    this.api.createBranch(departmentId, name).subscribe({
      next: () => this.refresh(),
      error: (err) => this.error.set(err?.error?.error ?? 'Create branch failed.'),
    });
  }
}
