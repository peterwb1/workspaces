import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { quoteResponse } from 'src/app/Model/QuoteResponse';
import { QuoteService } from 'src/app/service/quote.service';
import { ageRangeValidator, MAXIMUM_AGE, MINIMUM_AGE } from 'src/app/validators/age-range.validator';

@Component({
  selector: 'app-quote',
  templateUrl: './quote.component.html',
  styleUrls: ['./quote.component.scss'],
  standalone: false
})
export class QuoteComponent implements OnInit {

  readonly minimumAge = MINIMUM_AGE;
  readonly maximumAge = MAXIMUM_AGE;

  details: quoteDetail = new quoteDetail();
  models: Array<string> = [];

  submitted = false;
  loading = false;
  result: quoteResponse | null = null;
  detailsFailed = false;
  requestFailed = false;

  quoteForm = new FormGroup({
    insuranceType: new FormControl('', Validators.required),
    dateOfBirth: new FormControl('', [Validators.required, ageRangeValidator()]),
    make: new FormControl('', Validators.required),
    model: new FormControl('', Validators.required)
  });

  constructor(private quoteService: QuoteService) { }

  get insuranceTypeCtrl() { return this.quoteForm.controls.insuranceType; }
  get dateOfBirthCtrl() { return this.quoteForm.controls.dateOfBirth; }
  get makeCtrl() { return this.quoteForm.controls.make; }
  get modelCtrl() { return this.quoteForm.controls.model; }

  /** The date input must not offer a date that cannot be a date of birth. */
  get latestDateOfBirth(): string {
    return new Date().toISOString().slice(0, 10);
  }

  ngOnInit(): void {
    this.quoteService.getDetails<quoteDetail>().subscribe({
      next: details => {
        this.details = details;
        this.setModels();
      },
      error: () => this.detailsFailed = true
    });

    // Changing make invalidates whatever model was chosen for the previous one,
    // which the original left stale - you could quote a Ford X5.
    this.makeCtrl.valueChanges.subscribe(() => {
      this.modelCtrl.setValue('');
      this.setModels();
    });
  }

  setModels(): void {
    const currentMake = this.makeCtrl.value;
    const spec = this.details.models.find(m => m.make === currentMake);
    this.models = spec ? spec.models : [];
  }

  getQuote(): void {
    this.submitted = true;
    this.result = null;
    this.requestFailed = false;

    if (this.quoteForm.invalid) {
      this.quoteForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    this.quoteService.GetQuote<quoteResponse>(this.quoteForm.value).subscribe({
      next: response => {
        this.result = response;
        this.loading = false;
      },
      // Covers both a network failure and the 400 the API returns for a request
      // that fails model validation. The original silently did nothing.
      error: () => {
        this.requestFailed = true;
        this.loading = false;
      }
    });
  }

  reset(): void {
    this.quoteForm.reset({ insuranceType: '', dateOfBirth: '', make: '', model: '' });
    this.submitted = false;
    this.result = null;
    this.requestFailed = false;
    this.models = [];
  }

  /** True once a control should start showing its error, not before. */
  showError(control: { invalid: boolean; touched: boolean; dirty: boolean }): boolean {
    return control.invalid && (control.touched || control.dirty || this.submitted);
  }
}
