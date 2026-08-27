import { Routes } from '@angular/router';
import { ASSISTANT_ROUTES } from './features/assistant/assistant.routes';
import { FAQS_ROUTES } from './features/faqs/faqs.routes';
import { FEEDBACK_ROUTES } from './features/feedback/feedback.routes';
import { REQUESTS_ROUTES } from './features/requests/requests.routes';

export const PORTAL_ROUTES: Routes = [
  ...REQUESTS_ROUTES,
  ...FAQS_ROUTES,
  ...FEEDBACK_ROUTES,
  ...ASSISTANT_ROUTES,
];
