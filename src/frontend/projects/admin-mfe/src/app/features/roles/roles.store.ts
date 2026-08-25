import { Injectable, inject, signal } from '@angular/core';
import { RoleSummary } from '../users/users.models';
import { UsersApi } from '../users/users.api';

/** SDD CRM-035 — roles feature store (signals). */
@Injectable({ providedIn: 'root' })
export class RolesStore {
  private readonly api = inject(UsersApi);
  readonly roles = signal<RoleSummary[]>([]);
  readonly catalog = signal<string[]>([]);
  readonly error = signal('');

  load(): void {
    this.error.set('');
    this.api.roles().subscribe({
      next: (rows) => this.roles.set(rows),
      error: () => this.error.set('Could not load roles.'),
    });
  }

  loadCatalog(): void {
    this.api.permissions().subscribe({
      next: (dto) => this.catalog.set(dto.permissions ?? []),
      error: () => this.error.set('Could not load permission catalog.'),
    });
  }

  setRolePermissions(roleName: string, permissions: string[], onDone?: () => void): void {
    this.error.set('');
    this.api.updateRolePermissions(roleName, permissions).subscribe({
      next: () => {
        this.load();
        onDone?.();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Role permissions update failed.'),
    });
  }

  createPermission(name: string, description: string, onDone?: () => void): void {
    this.error.set('');
    this.api.createPermission(name, description || undefined).subscribe({
      next: () => {
        this.loadCatalog();
        this.load();
        onDone?.();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Create permission failed.'),
    });
  }

  updatePermission(
    currentName: string,
    name: string,
    description: string,
    onDone?: () => void,
  ): void {
    this.error.set('');
    this.api.updatePermission(currentName, name, description || undefined).subscribe({
      next: () => {
        this.loadCatalog();
        this.load();
        onDone?.();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Update permission failed.'),
    });
  }

  deletePermission(name: string, onDone?: () => void): void {
    this.error.set('');
    this.api.deletePermission(name).subscribe({
      next: () => {
        this.loadCatalog();
        this.load();
        onDone?.();
      },
      error: (err) => this.error.set(err?.error?.error ?? 'Delete permission failed.'),
    });
  }
}
