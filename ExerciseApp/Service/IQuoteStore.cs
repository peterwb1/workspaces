using System;
using System.Collections.Generic;
using ExerciseApp.Model;

namespace ExerciseApp.Service
{
    public interface IQuoteStore
    {
        /// <summary>Records a quote and returns it with its new reference.</summary>
        StoredQuote Save(QuoteRequest request, QuoteResponse response);

        /// <summary>The quote with this reference, or null if there is none.</summary>
        StoredQuote Find(Guid reference);

        /// <summary>The most recently requested quotes, newest first.</summary>
        IReadOnlyList<StoredQuote> History(int limit = 50);
    }
}
