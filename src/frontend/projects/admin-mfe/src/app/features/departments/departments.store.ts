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

  createDepartment(name: string, onDone?: () => void, onError?: (msg: string) => void): void {
    this.api.createDepartment(name).subscribe({
      next: () => {
        this.refresh();
        onDone?.();
      },
      error: (err) => {
        const msg = err?.error?.error ?? 'Create department failed.';
        this.error.set(msg);
        onError?.(msg);
      },
    });
  }

  createBranch(
    departmentId: string,
    name: string,
    onDone?: () => void,
    onError?: (msg: string) => void,
  ): void {
    this.api.createBranch(departmentId, name).subscribe({
      next: () => {
        this.refresh();
        onDone?.();
      },
      error: (err) => {
        const msg = err?.error?.error ?? 'Create branch failed.';
        this.error.set(msg);
        onError?.(msg);
      },
    });
  }
}
