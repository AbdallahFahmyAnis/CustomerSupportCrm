export type CrmDataViewMode = 'list' | 'grid';

export interface CrmDataViewColumn {
  key: string;
  header: string;
  /** Optional class on th/td */
  className?: string;
}
