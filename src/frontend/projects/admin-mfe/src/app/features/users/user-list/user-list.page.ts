import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CrmDataActionsDirective,
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataToolbarDirective,
  CrmDataViewColumn,
  CrmDataViewComponent,
  CrmDataViewMode,
  CrmModalComponent,
} from 'shared';
import { UserSummary } from '../users.models';
import { UsersStore } from '../users.store';

/** SDD CRM-035 — users list smart page. */
@Component({
  selector: 'app-user-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    CrmDataViewComponent,
    CrmDataActionsDirective,
    CrmDataToolbarDirective,
    CrmDataCellDirective,
    CrmDataCardDirective,
    CrmModalComponent,
  ],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.scss'],
})
export class UserListPage implements OnInit {
  readonly store = inject(UsersStore);
  q = '';
  viewMode: CrmDataViewMode = 'list';
  confirmOpen = false;
  editOpen = false;
  editRole = '';
  pendingUser: UserSummary | null = null;

  readonly columns: CrmDataViewColumn[] = [
    { key: 'displayName', header: 'User' },
    { key: 'email', header: 'Email' },
    { key: 'role', header: 'Role' },
    { key: 'isActive', header: 'Status' },
    { key: 'actions', header: 'Actions' },
  ];

  ngOnInit(): void {
    this.store.loadRoles();
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.loadUsers();
  }

  askEdit(user: UserSummary): void {
    this.pendingUser = user;
    this.editRole = user.role;
    this.editOpen = true;
  }

  confirmEdit(): void {
    const id = this.pendingUser?.id;
    const role = this.editRole.trim();
    this.editOpen = false;
    this.pendingUser = null;
    if (id && role) {
      this.store.setRole(id, role);
    }
  }

  askDeactivate(user: UserSummary): void {
    this.pendingUser = user;
    this.confirmOpen = true;
  }

  confirmDeactivate(): void {
    const id = this.pendingUser?.id;
    this.confirmOpen = false;
    this.pendingUser = null;
    if (id) {
      this.store.deactivate(id);
    }
  }
}
