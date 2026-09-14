export type QuoteOutcome =
    | 'Offered'
    | 'DeclinedTooYoung'
    | 'DeclinedTooOld'
    | 'InvalidRequest';

export class quoteResponse {
    outcome: QuoteOutcome = 'InvalidRequest';
    // Both are omitted by the API when they do not apply: premium is present only
    // when the outcome is Offered, message only when it is not.
    premium?: number;
    message?: string;
}
