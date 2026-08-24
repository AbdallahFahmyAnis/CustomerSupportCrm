import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { RequestsStore } from '../requests.store';

/** SDD CRM-028 — track portal requests by email. */
@Component({
  selector: 'app-track-requests-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './track-requests.html',
  styleUrls: ['./track-requests.scss'],
})
export class TrackRequestsPage {
  readonly store = inject(RequestsStore);
  email = 'portal.customer@example.com';

  search(): void {
    this.store.track(this.email);
  }
}
