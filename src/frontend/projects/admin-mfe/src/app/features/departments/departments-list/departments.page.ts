import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DepartmentsStore } from '../departments.store';

/** SDD CRM-043 */
@Component({
  selector: 'app-departments-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './departments.html',
  styleUrls: ['./departments.scss'],
})
export class DepartmentsPage implements OnInit {
  readonly store = inject(DepartmentsStore);
  deptName = '';
  branchName = '';
  branchDeptId = '';

  ngOnInit(): void {
    this.store.refresh();
  }

  addDept(): void {
    if (!this.deptName.trim()) return;
    this.store.createDepartment(this.deptName.trim());
    this.deptName = '';
  }

  addBranch(): void {
    if (!this.branchName.trim() || !this.branchDeptId) return;
    this.store.createBranch(this.branchDeptId, this.branchName.trim());
    this.branchName = '';
  }
}
