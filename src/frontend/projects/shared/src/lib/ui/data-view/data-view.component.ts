import { NgTemplateOutlet } from '@angular/common';
import {
  Component,
  ContentChild,
  ContentChildren,
  QueryList,
  inject,
  input,
  model,
} from '@angular/core';
import { LanguageStore } from '../../language.store';
import {
  CrmDataActionsDirective,
  CrmDataCardDirective,
  CrmDataCellDirective,
  CrmDataToolbarDirective,
} from './data-view.directives';
import { CrmDataViewColumn, CrmDataViewMode } from './data-view.models';

/**
 * Materio-inspired list/grid data view (shared).
 * Shape inspired by ThemeSelection Materio user-list — original styles only.
 */
@Component({
  selector: 'crm-data-view',
  standalone: true,
  imports: [NgTemplateOutlet],
  templateUrl: './data-view.html',
  styleUrls: ['./data-view.scss'],
})
export class CrmDataViewComponent {
  readonly lang = inject(LanguageStore);
  readonly items = input.required<readonly unknown[]>();
  readonly columns = input.required<CrmDataViewColumn[]>();
  readonly trackKey = input<string>('id');
  readonly emptyText = input('');
  readonly title = input('');
  readonly subtitle = input('');
  readonly viewMode = model<CrmDataViewMode>('list');

  @ContentChild(CrmDataToolbarDirective) toolbar?: CrmDataToolbarDirective;
  @ContentChild(CrmDataActionsDirective) actions?: CrmDataActionsDirective;
  @ContentChild(CrmDataCardDirective) cardTpl?: CrmDataCardDirective;
  @ContentChildren(CrmDataCellDirective) cells!: QueryList<CrmDataCellDirective>;

  emptyLabel(): string {
    return this.emptyText() || this.lang.t('noRecordsFound');
  }

  cellFor(key: string): CrmDataCellDirective | undefined {
    return this.cells?.find((c) => c.column() === key);
  }

  trackOf = (_: number, row: unknown) => {
    const key = this.trackKey();
    if (row && typeof row === 'object' && key in (row as object)) {
      return String((row as Record<string, unknown>)[key]);
    }
    return _;
  };

  valueOf(row: unknown, key: string): unknown {
    if (!row || typeof row !== 'object') {
      return '';
    }
    return (row as Record<string, unknown>)[key] ?? '';
  }

  setMode(mode: CrmDataViewMode): void {
    this.viewMode.set(mode);
  }
}
