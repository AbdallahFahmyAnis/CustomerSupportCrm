import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CrmRoleCardComponent } from 'shared';
import { RolesStore } from '../roles.store';

/** SDD CRM-035 — roles list (Materio access-roles shape). */
@Component({
  selector: 'app-role-list-page',
  standalone: true,
  imports: [RouterLink, CrmRoleCardComponent],
  templateUrl: './role-list.html',
  styleUrls: ['./role-list.scss'],
})
export class RoleListPage implements OnInit {
  readonly store = inject(RolesStore);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.store.load();
  }

  openPermissions(): void {
    void this.router.navigate(['/admin/roles/permissions']);
  }
}
