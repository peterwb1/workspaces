namespace ExerciseApp.Model
{
    /// <summary>
    /// The result of a quote request. Any outcome other than <see cref="Offered"/>
    /// means no premium was produced and <see cref="QuoteResponse.Premium"/> is null.
    /// </summary>
    public enum QuoteOutcome
    {
        /// <summary>A premium was calculated and is available on the response.</summary>
        Offered,

        /// <summary>The driver is below the minimum age we will insure.</summary>
        DeclinedTooYoung,

        /// <summary>The driver is above the maximum age we will insure.</summary>
        DeclinedTooOld,

        /// <summary>
        /// The request was well formed but could not be quoted, for example a date of
        /// birth in the future or a vehicle we hold no rate for.
        /// </summary>
        InvalidRequest
    }
}
