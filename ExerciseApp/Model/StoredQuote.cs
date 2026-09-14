using System;

namespace ExerciseApp.Model
{
    /// <summary>
    /// A quote as it was answered, kept so it can be retrieved later. Declined
    /// requests are stored too - knowing who was turned away, and why, is as useful
    /// as knowing who was quoted.
    /// </summary>
    public class StoredQuote
    {
        /// <summary>Identifies this quote to the customer and in the history.</summary>
        public Guid Reference { get; set; }

        public DateTimeOffset RequestedAt { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public InsuranceType InsuranceType { get; set; }

        public QuoteOutcome Outcome { get; set; }

        /// <summary>Null unless the quote was offered.</summary>
        public decimal? Premium { get; set; }
    }
}
