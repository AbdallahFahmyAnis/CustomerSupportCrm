import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CrmModalComponent, CrmRoleCardComponent, LanguageStore } from 'shared';
import { RoleSummary } from '../../users/users.models';
import { RolesStore } from '../roles.store';

/** SDD CRM-035 — roles list (Materio access-roles shape). */
@Component({
  selector: 'app-role-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink, CrmRoleCardComponent, CrmModalComponent],
  templateUrl: './role-list.html',
  styleUrls: ['./role-list.scss'],
})
export class RoleListPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(RolesStore);

  editOpen = false;
  pendingRole: RoleSummary | null = null;
  editSelected = new Set<string>();

  ngOnInit(): void {
    this.store.load();
    this.store.loadCatalog();
  }

  askEditRole(role: RoleSummary): void {
    this.pendingRole = role;
    this.editSelected = new Set(role.permissions);
    this.editOpen = true;
  }

  togglePermission(name: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const next = new Set(this.editSelected);
    if (checked) {
      next.add(name);
    } else {
      next.delete(name);
    }
    this.editSelected = next;
  }

  confirmEditRole(): void {
    const role = this.pendingRole;
    if (!role) {
      return;
    }
    const permissions = [...this.editSelected].sort();
    this.editOpen = false;
    this.pendingRole = null;
    this.store.setRolePermissions(role.name, permissions);
  }
}
