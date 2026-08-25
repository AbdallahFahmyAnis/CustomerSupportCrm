/** SDD CRM-035 / specs/004-identity-admin */
export interface UserSummary {
  id: string;
  email: string;
  displayName: string;
  role: string;
  isActive: boolean;
  departmentId?: string | null;
  branchId?: string | null;
}

export interface RoleSummary {
  name: string;
  description: string;
  permissions: string[];
}
