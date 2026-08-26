import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormFeedbackStore, LanguageStore, MessageKey, SessionApi } from 'shared';
import { RequestsApi } from '../../requests/requests.api';
import { PortalRequestSummary } from '../../requests/requests.models';
import { FeedbackStore } from '../feedback.store';

/** SDD CRM-030 — portal CSAT form. */
@Component({
  selector: 'app-feedback-form-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './feedback-form.html',
  styleUrls: ['./feedback-form.scss'],
})
export class FeedbackFormPage implements OnInit {
  readonly lang = inject(LanguageStore);
  readonly store = inject(FeedbackStore);
  private readonly feedback = inject(FormFeedbackStore);
  private readonly route = inject(ActivatedRoute);
  private readonly session = inject(SessionApi);
  private readonly requestsApi = inject(RequestsApi);

  ticketNumber = '';
  rating = 5;
  comment = '';
  fromAssistant = false;
  loadingTickets = false;
  eligibleTickets: PortalRequestSummary[] = [];

  readonly ratingOptions: { value: number; labelKey: MessageKey }[] = [
    { value: 5, labelKey: 'ratingExcellent' },
    { value: 4, labelKey: 'ratingGood' },
    { value: 3, labelKey: 'ratingOkay' },
    { value: 2, labelKey: 'ratingPoor' },
    { value: 1, labelKey: 'ratingVeryPoor' },
  ];

  ngOnInit(): void {
    const qp = this.route.snapshot.queryParamMap;
    const ticket = qp.get('ticket')?.trim() ?? '';
    this.fromAssistant = qp.get('from') === 'assistant';
    if (ticket) {
      this.ticketNumber = ticket;
    }
    this.loadEligibleTickets(ticket);
  }

  submit(f: NgForm): void {
    if (f.invalid || !this.ticketNumber.trim()) {
      this.feedback.error('formInvalid');
      return;
    }
    this.store.submit(
      this.ticketNumber,
      this.rating,
      this.comment,
      () => this.feedback.success('feedbackSuccess'),
      (msg) => this.feedback.errorText(msg),
    );
  }

  private loadEligibleTickets(preferredTicket: string): void {
    const email = this.session.session()?.email?.trim();
    if (!email) {
      return;
    }
    this.loadingTickets = true;
    this.requestsApi.track(email).subscribe({
      next: (rows) => {
        this.eligibleTickets = (rows ?? []).filter(
          (r) =>
            r.status === 'Resolved' ||
            r.status === 'Closed',
        );
        if (!this.ticketNumber.trim()) {
          const preferred = this.eligibleTickets.find(
            (r) => r.ticketNumber === preferredTicket,
          );
          this.ticketNumber =
            preferred?.ticketNumber ||
            this.eligibleTickets[0]?.ticketNumber ||
            '';
        }
        this.loadingTickets = false;
      },
      error: () => {
        this.loadingTickets = false;
      },
    });
  }
}
