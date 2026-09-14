import { CommonModule } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { QuoteComponent } from './quote.component';

describe('QuoteComponent', () => {
  let component: QuoteComponent;
  let fixture: ComponentFixture<QuoteComponent>;
  let http: HttpTestingController;

  const details = {
    makes: ['Ford', 'BMW'],
    models: [
      { make: 'Ford', models: ['Fiesta', 'Focus'] },
      { make: 'BMW', models: ['X5', '3 Series'] }
    ],
    insuranceTypes: [{ type: 'FullyComprehensive', description: 'Fully Comprehensive' }]
  };

  /** A date of birth that is comfortably inside the age range. */
  const validDateOfBirth = '2000-05-01';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [QuoteComponent],
      imports: [CommonModule, ReactiveFormsModule],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    fixture = TestBed.createComponent(QuoteComponent);
    component = fixture.componentInstance;
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  /** Runs ngOnInit and answers the details request the component makes. */
  function initialise(payload: object = details): void {
    fixture.detectChanges();
    http.expectOne(request => request.method === 'GET').flush(payload);
    fixture.detectChanges();
  }

  function fillInValidForm(): void {
    component.quoteForm.setValue({
      insuranceType: 'FullyComprehensive',
      dateOfBirth: validDateOfBirth,
      make: 'Ford',
      model: 'Focus'
    });
  }

  it('should create', () => {
    initialise();

    expect(component).toBeTruthy();
  });

  it('loads the makes and models on startup', () => {
    initialise();

    expect(component.details.makes).toEqual(['Ford', 'BMW']);
  });

  it('reports a failure to load the makes rather than showing an empty form', () => {
    fixture.detectChanges();
    http.expectOne(request => request.method === 'GET')
      .flush('boom', { status: 500, statusText: 'Server Error' });
    fixture.detectChanges();

    expect(component.detailsFailed).toBeTrue();
  });

  it('offers only the models belonging to the chosen make', () => {
    initialise();

    component.quoteForm.controls.make.setValue('BMW');

    expect(component.models).toEqual(['X5', '3 Series']);
  });

  it('clears a model that belonged to the previous make', () => {
    initialise();
    component.quoteForm.controls.make.setValue('Ford');
    component.quoteForm.controls.model.setValue('Focus');

    component.quoteForm.controls.make.setValue('BMW');

    expect(component.quoteForm.controls.model.value).toBe('');
  });

  it('does not send a request when the form is incomplete', () => {
    initialise();

    component.getQuote();

    http.expectNone(request => request.method === 'POST');
    expect(component.submitted).toBeTrue();
  });

  it('does not send a request for a driver who is too young', () => {
    initialise();
    fillInValidForm();
    component.quoteForm.controls.dateOfBirth.setValue('2020-01-01');

    component.getQuote();

    http.expectNone(request => request.method === 'POST');
    expect(component.quoteForm.controls.dateOfBirth.errors?.['tooYoung']).toBeTruthy();
  });

  it('does not send a request for a driver who is too old', () => {
    initialise();
    fillInValidForm();
    component.quoteForm.controls.dateOfBirth.setValue('1900-01-01');

    component.getQuote();

    http.expectNone(request => request.method === 'POST');
    expect(component.quoteForm.controls.dateOfBirth.errors?.['tooOld']).toBeTruthy();
  });

  it('shows the premium when a quote is offered', () => {
    initialise();
    fillInValidForm();

    component.getQuote();
    http.expectOne(request => request.method === 'POST').flush({ outcome: 'Offered', premium: 200 });
    fixture.detectChanges();

    expect(component.result?.premium).toBe(200);
    expect(fixture.nativeElement.textContent).toContain('£200.00');
  });

  it('shows the reason when the API declines to quote', () => {
    initialise();
    fillInValidForm();

    component.getQuote();
    http.expectOne(request => request.method === 'POST')
      .flush({ outcome: 'DeclinedTooYoung', message: 'We can only insure drivers aged 17 or over.' });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('We can only insure drivers aged 17 or over.');
  });

  it('reports a failed request rather than silently doing nothing', () => {
    initialise();
    fillInValidForm();

    component.getQuote();
    http.expectOne(request => request.method === 'POST')
      .flush('bad request', { status: 400, statusText: 'Bad Request' });
    fixture.detectChanges();

    expect(component.requestFailed).toBeTrue();
    expect(component.loading).toBeFalse();
  });

  it('clears the previous result when starting again', () => {
    initialise();
    fillInValidForm();
    component.getQuote();
    http.expectOne(request => request.method === 'POST').flush({ outcome: 'Offered', premium: 200 });

    component.reset();

    expect(component.result).toBeNull();
    expect(component.quoteForm.controls.make.value).toBe('');
  });
});
