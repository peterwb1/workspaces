import { FormControl } from '@angular/forms';
import { ageInYears, ageRangeValidator, parseDateInput } from './age-range.validator';

describe('age-range validator', () => {

  // Pinned so these cases keep their meaning as real time passes.
  const quoteDate = new Date(2026, 8, 14); // 14 September 2026
  const validator = ageRangeValidator(17, 80, () => quoteDate);

  const validate = (value: string) => validator(new FormControl(value));

  describe('ageInYears', () => {
    it('counts a birthday falling on the day itself', () => {
      expect(ageInYears(new Date(2009, 8, 14), quoteDate)).toBe(17);
    });

    it('does not count a birthday that has not arrived yet', () => {
      expect(ageInYears(new Date(2009, 8, 15), quoteDate)).toBe(16);
    });
  });

  describe('parseDateInput', () => {
    it('reads the yyyy-mm-dd value a date input produces as a local date', () => {
      const parsed = parseDateInput('2009-09-14')!;

      expect(parsed.getFullYear()).toBe(2009);
      expect(parsed.getMonth()).toBe(8);
      expect(parsed.getDate()).toBe(14);
    });

    it('rejects a date that does not exist', () => {
      expect(parseDateInput('2026-02-31')).toBeNull();
    });

    it('rejects text that is not a date at all', () => {
      expect(parseDateInput('not a date')).toBeNull();
    });
  });

  it('accepts a driver inside the age range', () => {
    expect(validate('2000-05-01')).toBeNull();
  });

  it('accepts a driver on their seventeenth birthday', () => {
    expect(validate('2009-09-14')).toBeNull();
  });

  it('accepts a driver on their eightieth birthday', () => {
    expect(validate('1946-09-14')).toBeNull();
  });

  it('rejects a driver one day short of seventeen', () => {
    expect(validate('2009-09-15')?.['tooYoung']).toBeTruthy();
  });

  it('rejects a driver who has turned eighty one', () => {
    expect(validate('1945-09-14')?.['tooOld']).toBeTruthy();
  });

  it('rejects a date of birth in the future', () => {
    expect(validate('2030-01-01')?.['futureDate']).toBeTrue();
  });

  it('leaves an empty value to the required validator', () => {
    expect(validate('')).toBeNull();
  });
});
