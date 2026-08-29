import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { FormFeedbackStore, LanguageStore, SessionApi } from 'shared';
import { RegisterPage } from './register.page';

/** SDD CRM-045 */
describe('RegisterPage', () => {
  let fixture: ComponentFixture<RegisterPage>;
  let component: RegisterPage;
  let session: jasmine.SpyObj<SessionApi>;
  let feedback: jasmine.SpyObj<FormFeedbackStore>;
  let router: Router;

  beforeEach(async () => {
    session = jasmine.createSpyObj('SessionApi', ['register']);
    feedback = jasmine.createSpyObj('FormFeedbackStore', ['error', 'errorText', 'success']);

    await TestBed.configureTestingModule({
      imports: [RegisterPage],
      providers: [
        provideRouter([]),
        LanguageStore,
        { provide: SessionApi, useValue: session },
        { provide: FormFeedbackStore, useValue: feedback },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterPage);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.returnValue(Promise.resolve(true));
  });

  it('rejects mismatched passwords without calling register', () => {
    component.displayName = 'Test User';
    component.email = 'new@example.com';
    component.password = 'secret1';
    component.confirmPassword = 'secret2';

    const form = { invalid: false } as never;
    component.submit(form);

    expect(session.register).not.toHaveBeenCalled();
    expect(feedback.error).toHaveBeenCalledWith('passwordMismatch');
  });

  it('registers and navigates to portal for Customer role', () => {
    session.register.and.returnValue(
      of({ authenticated: true, role: 'Customer', email: 'new@example.com' }),
    );
    component.displayName = 'Test User';
    component.email = 'new@example.com';
    component.password = 'secret1';
    component.confirmPassword = 'secret1';

    component.submit({ invalid: false } as never);

    expect(session.register).toHaveBeenCalledWith({
      email: 'new@example.com',
      displayName: 'Test User',
      password: 'secret1',
    });
    expect(feedback.success).toHaveBeenCalledWith('registerSuccess');
    expect(router.navigateByUrl).toHaveBeenCalledWith('/portal');
  });

  it('surfaces API error text when register fails', () => {
    session.register.and.returnValue(throwError(() => ({ error: { error: 'Email already used' } })));
    component.displayName = 'Test User';
    component.email = 'dup@example.com';
    component.password = 'secret1';
    component.confirmPassword = 'secret1';

    component.submit({ invalid: false } as never);

    expect(feedback.errorText).toHaveBeenCalledWith('Email already used');
    expect(component.saving).toBeFalse();
  });
});
