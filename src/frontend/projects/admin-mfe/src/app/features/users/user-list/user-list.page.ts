import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { UsersStore } from '../users.store';

/** SDD CRM-035 — users list smart page. */
@Component({
  selector: 'app-user-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.scss'],
})
export class UserListPage implements OnInit {
  readonly store = inject(UsersStore);
  q = '';

  ngOnInit(): void {
    this.store.loadRoles();
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.loadUsers();
  }

  onRoleChange(id: string, role: string): void {
    this.store.setRole(id, role);
  }

  deactivate(id: string): void {
    this.store.deactivate(id);
  }
}
