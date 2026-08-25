import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
} from 'shared';
import { UsersApi } from '../../users/users.api';
import { RolesStore } from '../roles.store';

interface PermissionRow {
  name: string;
  roles: string[];
}

/** Materio-inspired permissions table (app-access-permission). */
@Component({
  selector: 'app-permission-list-page',
  standalone: true,
  imports: [
    RouterLink,
    CrmDataViewComponent,
    CrmDataCellDirective,
    CrmDataCardDirective,
  ],
  templateUrl: './permission-list.html',
  styleUrls: ['./permission-list.scss'],
})
export class PermissionListPage implements OnInit {
  private readonly api = inject(UsersApi);
  private readonly rolesStore = inject(RolesStore);

  readonly catalog = signal<string[]>([]);
  readonly error = signal('');
  viewMode: CrmDataViewMode = 'list';

  readonly columns: CrmDataViewColumn[] = [
    { key: 'name', header: 'Name' },
    { key: 'roles', header: 'Assigned to' },
  ];

  readonly rows = computed<PermissionRow[]>(() => {
    const roles = this.rolesStore.roles();
    const names = this.catalog().length
      ? this.catalog()
      : [...new Set(roles.flatMap((r) => r.permissions))].sort();
    return names.map((name) => ({
      name,
      roles: roles.filter((r) => r.permissions.includes(name)).map((r) => r.name),
    }));
  });

  ngOnInit(): void {
    this.rolesStore.load();
    this.api.permissions().subscribe({
      next: (dto) => this.catalog.set(dto.permissions ?? []),
      error: () => this.error.set('Could not load permission catalog.'),
    });
  }
}
