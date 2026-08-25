import { Component, input } from '@angular/core';
import { CrmTimelineItem } from './timeline.models';

/**
 * Materio-inspired basic timeline (extended-ui-timeline-basic shape).
 * Original styles only — not ThemeSelection assets.
 */
@Component({
  selector: 'crm-timeline',
  standalone: true,
  templateUrl: './timeline.html',
  styleUrls: ['./timeline.scss'],
})
export class CrmTimelineComponent {
  readonly items = input.required<readonly CrmTimelineItem[]>();
  readonly emptyText = input('No timeline events yet.');
}
