import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore } from 'shared';
import { Branch } from '../departments.models';
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
  readonly lang = inject(LanguageStore);
  readonly store = inject(DepartmentsStore);
  private readonly feedback = inject(FormFeedbackStore);
  deptName = '';
  branchName = '';
  branchDeptId = '';

  ngOnInit(): void {
    this.store.refresh();
  }

  branchesFor(departmentId: string): Branch[] {
    return this.store.branches().filter((b) => b.departmentId === departmentId);
  }

  addDept(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    const name = this.deptName.trim();
    this.store.createDepartment(
      name,
      () => {
        this.feedback.success('departmentAddSuccess');
        this.deptName = '';
        f.resetForm();
      },
      (msg) => this.feedback.errorText(msg),
    );
  }

  addBranch(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    const name = this.branchName.trim();
    const deptId = this.branchDeptId;
    this.store.createBranch(
      deptId,
      name,
      () => {
        this.feedback.success('branchAddSuccess');
        this.branchName = '';
        f.resetForm({ branchDeptId: deptId });
        this.branchDeptId = deptId;
      },
      (msg) => this.feedback.errorText(msg),
    );
  }
}
