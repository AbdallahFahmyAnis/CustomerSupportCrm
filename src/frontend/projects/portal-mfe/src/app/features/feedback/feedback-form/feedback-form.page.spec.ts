import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { FormFeedbackStore, LanguageStore, SessionApi } from 'shared';
import { RequestsApi } from '../../requests/requests.api';
import { FeedbackApi } from '../feedback.api';
import { FeedbackStore } from '../feedback.store';
import { FeedbackFormPage } from './feedback-form.page';

/** SDD CRM-030 / specs/010-portal/053-feedback-read-only */
describe('FeedbackFormPage', () => {
  let fixture: ComponentFixture<FeedbackFormPage>;
  let component: FeedbackFormPage;
  let feedbackApi: jasmine.SpyObj<FeedbackApi>;
  let requestsApi: jasmine.SpyObj<RequestsApi>;
  let queryParams: Record<string, string | null>;

  beforeEach(async () => {
    queryParams = { ticket: 'TKT-1012', from: null };
    feedbackApi = jasmine.createSpyObj('FeedbackApi', ['getByTicketNumber']);
    requestsApi = jasmine.createSpyObj('RequestsApi', ['track']);

    feedbackApi.getByTicketNumber.and.returnValue(
      of({
        id: 'fb-1',
        ticketId: 't-1',
        rating: 4,
        comment: 'Great help',
        createdAt: '2026-08-29T12:00:00Z',
      }),
    );
    requestsApi.track.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [FeedbackFormPage],
      providers: [
        provideRouter([]),
        LanguageStore,
        FeedbackStore,
        FormFeedbackStore,
        { provide: FeedbackApi, useValue: feedbackApi },
        { provide: RequestsApi, useValue: requestsApi },
        {
          provide: SessionApi,
          useValue: { session: () => ({ email: 'c@example.com' }) },
        },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              queryParamMap: {
                get: (key: string) => queryParams[key] ?? null,
              },
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(FeedbackFormPage);
    component = fixture.componentInstance;
  });

  it('loads existing feedback for ticket query param', () => {
    component.ngOnInit();

    expect(feedbackApi.getByTicketNumber).toHaveBeenCalledWith('TKT-1012');
    expect(component.existingFeedback?.rating).toBe(4);
    expect(component.existingFeedback?.comment).toBe('Great help');
  });

  it('clears existing feedback when ticket number is cleared', () => {
    component.existingFeedback = {
      id: 'fb-1',
      ticketId: 't-1',
      rating: 5,
      createdAt: '2026-08-29T12:00:00Z',
    };
    component.ticketNumber = '';
    component.onTicketChange();

    expect(component.existingFeedback).toBeNull();
  });
});
