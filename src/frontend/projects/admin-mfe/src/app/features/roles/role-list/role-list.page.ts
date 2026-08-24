import { Component, OnInit, inject } from '@angular/core';
import { RolesStore } from '../roles.store';

/** SDD CRM-035 — roles list smart page. */
@Component({
  selector: 'app-role-list-page',
  standalone: true,
  templateUrl: './role-list.html',
  styleUrls: ['./role-list.scss'],
})
export class RoleListPage implements OnInit {
  readonly store = inject(RolesStore);

  ngOnInit(): void {
    this.store.load();
  }
}
