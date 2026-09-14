import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const MINIMUM_AGE = 17;
export const MAXIMUM_AGE = 80;

/** Completed years lived at `asAt`, counting a birthday falling on that day. */
export function ageInYears(dateOfBirth: Date, asAt: Date): number {
    let age = asAt.getFullYear() - dateOfBirth.getFullYear();

    const birthdayThisYear = new Date(
        asAt.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate());

    if (asAt < birthdayThisYear) {
        age--;
    }

    return age;
}

/**
 * Parses the yyyy-mm-dd value an <input type="date"> produces into a local date.
 * Passing that string to `new Date()` would parse it as UTC midnight, which reads
 * as the previous day for anyone behind UTC.
 */
export function parseDateInput(value: string): Date | null {
    const parts = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value);

    if (!parts) {
        return null;
    }

    const [year, month, day] = [+parts[1], +parts[2], +parts[3]];
    const date = new Date(year, month - 1, day);

    // Rejects impossible dates such as 2026-02-31, which would otherwise roll over.
    return date.getFullYear() === year && date.getMonth() === month - 1 && date.getDate() === day
        ? date
        : null;
}

/**
 * Mirrors the age rules the API enforces, so the customer is told before
 * submitting rather than after. The server remains the authority - this is a
 * convenience, not the enforcement point.
 */
export function ageRangeValidator(
    minimumAge: number = MINIMUM_AGE,
    maximumAge: number = MAXIMUM_AGE,
    today: () => Date = () => new Date()): ValidatorFn {

    return (control: AbstractControl): ValidationErrors | null => {
        if (!control.value) {
            return null; // An empty value is the required validator's business.
        }

        const dateOfBirth = parseDateInput(control.value);

        if (dateOfBirth === null) {
            return { invalidDate: true };
        }

        const asAt = today();

        if (dateOfBirth > asAt) {
            return { futureDate: true };
        }

        const age = ageInYears(dateOfBirth, asAt);

        if (age < minimumAge) {
            return { tooYoung: { minimumAge, actualAge: age } };
        }

        if (age > maximumAge) {
            return { tooOld: { maximumAge, actualAge: age } };
        }

        return null;
    };
}
