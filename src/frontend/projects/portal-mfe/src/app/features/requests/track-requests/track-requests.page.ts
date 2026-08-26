import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormFeedbackStore, LanguageStore, SessionApi } from 'shared';
import { RequestsStore } from '../requests.store';

/** SDD CRM-028 — track portal requests by email (auto for signed-in customer). */
@Component({
  selector: 'app-track-requests-page',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './track-requests.html',
  styleUrls: ['./track-requests.scss'],
})
export class TrackRequestsPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(RequestsStore);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly session = inject(SessionApi);
  email = '';
  signedIn = false;

  ngOnInit(): void {
    const s = this.session.session();
    if (s?.email) {
      this.email = s.email;
      this.signedIn = true;
      this.store.track(this.email, undefined, (msg) => this.feedback.errorText(msg));
    }
  }

  search(f: NgForm): void {
    if (f.invalid) {
      this.feedback.error('formInvalid');
      return;
    }
    this.refresh();
  }

  refresh(): void {
    if (!this.email.trim()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.track(this.email, undefined, (msg) => this.feedback.errorText(msg));
  }
}
