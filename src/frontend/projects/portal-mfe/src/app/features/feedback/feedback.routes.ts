import { Routes } from '@angular/router';

/** SDD CRM-030 — portal feedback routes. */
export const FEEDBACK_ROUTES: Routes = [
  {
    path: 'feedback',
    loadComponent: () =>
      import('./feedback-form/feedback-form.page').then((m) => m.FeedbackFormPage),
  },
];
