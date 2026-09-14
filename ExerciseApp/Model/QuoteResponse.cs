using System;

namespace ExerciseApp.Model
{
    public class QuoteResponse
    {
        /// <summary>What happened to the request. Always populated.</summary>
        public QuoteOutcome Outcome { get; set; }

        /// <summary>
        /// The premium, in pounds. Null unless <see cref="Outcome"/> is
        /// <see cref="QuoteOutcome.Offered"/>, and omitted from the JSON when null.
        /// </summary>
        public decimal? Premium { get; set; }

        /// <summary>A description of why no quote was offered. Null when one was.</summary>
        public string Message { get; set; }

        /// <summary>
        /// Identifies the stored copy of this quote, so it can be retrieved later
        /// from /Quote/{reference}. Set by the controller once the quote is saved.
        /// </summary>
        public Guid? QuoteReference { get; set; }

        public static QuoteResponse Offered(decimal premium) =>
            new QuoteResponse { Outcome = QuoteOutcome.Offered, Premium = premium };

        public static QuoteResponse Declined(QuoteOutcome outcome, string message) =>
            new QuoteResponse { Outcome = outcome, Message = message };
    }
}
