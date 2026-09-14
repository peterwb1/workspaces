using ExerciseApp.Model;

namespace ExerciseApp.Service
{
    public interface IQuoteService
    {
        /// <summary>The makes, models and insurance types offered to the customer.</summary>
        QuoteDetail GetQuoteDetail();

        /// <summary>
        /// Prices a request, or explains why it cannot be priced. Never throws for
        /// data the customer supplied - unquotable input comes back as an outcome.
        /// </summary>
        QuoteResponse PerformQuote(QuoteRequest request);
    }
}
