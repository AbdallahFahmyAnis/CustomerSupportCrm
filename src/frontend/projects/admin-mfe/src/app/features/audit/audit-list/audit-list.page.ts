import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuditStore } from '../audit.store';

/** SDD CRM-036 — audit list smart page. */
@Component({
  selector: 'app-audit-list-page',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './audit-list.html',
  styleUrls: ['./audit-list.scss'],
})
export class AuditListPage implements OnInit {
  readonly store = inject(AuditStore);
  q = '';

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.store.query.set(this.q);
    this.store.load();
  }
}
