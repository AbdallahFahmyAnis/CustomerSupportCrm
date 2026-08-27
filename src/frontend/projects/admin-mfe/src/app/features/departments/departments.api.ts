import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Branch, Department } from './departments.models';

/** SDD CRM-043 */
@Injectable({ providedIn: 'root' })
export class DepartmentsApi {
  private readonly http = inject(HttpClient);

  listDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>('/api/identity/departments');
  }

  createDepartment(name: string): Observable<Department> {
    return this.http.post<Department>('/api/identity/departments', { name });
  }

  listBranches(departmentId?: string): Observable<Branch[]> {
    const q = departmentId ? `?departmentId=${encodeURIComponent(departmentId)}` : '';
    return this.http.get<Branch[]>(`/api/identity/branches${q}`);
  }

  createBranch(departmentId: string, name: string): Observable<Branch> {
    return this.http.post<Branch>('/api/identity/branches', { departmentId, name });
  }
}
