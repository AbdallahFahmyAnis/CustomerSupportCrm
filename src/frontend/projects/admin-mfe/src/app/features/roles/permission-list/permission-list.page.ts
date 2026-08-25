import { Component, OnInit, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CrmDataActionsDirective,
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
  CrmModalComponent,
} from 'shared';
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
    FormsModule,
    RouterLink,
    CrmDataViewComponent,
    CrmDataActionsDirective,
    CrmDataCellDirective,
    CrmDataCardDirective,
    CrmModalComponent,
  ],
  templateUrl: './permission-list.html',
  styleUrls: ['./permission-list.scss'],
})
export class PermissionListPage implements OnInit {
  readonly store = inject(RolesStore);

  viewMode: CrmDataViewMode = 'list';
  formOpen = false;
  deleteOpen = false;
  editingName: string | null = null;
  pendingName = '';
  formName = '';
  formDescription = '';

  readonly columns: CrmDataViewColumn[] = [
    { key: 'name', header: 'Name' },
    { key: 'roles', header: 'Assigned to' },
    { key: 'actions', header: 'Actions' },
  ];

  readonly rows = computed<PermissionRow[]>(() => {
    const roles = this.store.roles();
    const names = this.store.catalog().length
      ? this.store.catalog()
      : [...new Set(roles.flatMap((r) => r.permissions))].sort();
    return names.map((name) => ({
      name,
      roles: roles.filter((r) => r.permissions.includes(name)).map((r) => r.name),
    }));
  });

  ngOnInit(): void {
    this.store.load();
    this.store.loadCatalog();
  }

  askCreate(): void {
    this.editingName = null;
    this.formName = '';
    this.formDescription = '';
    this.formOpen = true;
  }

  askEdit(name: string): void {
    this.editingName = name;
    this.formName = name;
    this.formDescription = '';
    this.formOpen = true;
  }

  confirmForm(): void {
    const name = this.formName.trim();
    if (!name) {
      return;
    }
    const description = this.formDescription.trim();
    const current = this.editingName;
    this.formOpen = false;
    if (current) {
      this.store.updatePermission(current, name, description);
    } else {
      this.store.createPermission(name, description);
    }
    this.editingName = null;
  }

  askDelete(name: string): void {
    this.pendingName = name;
    this.deleteOpen = true;
  }

  confirmDelete(): void {
    const name = this.pendingName;
    this.deleteOpen = false;
    this.pendingName = '';
    if (name) {
      this.store.deletePermission(name);
    }
  }
}
