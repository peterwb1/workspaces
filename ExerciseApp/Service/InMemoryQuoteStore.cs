using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExerciseApp.Model;

namespace ExerciseApp.Service
{
    /// <summary>
    /// Keeps quotes in process memory.
    ///
    /// This is deliberately the simplest thing that satisfies "store quotes for later
    /// retrieval" without adding a dependency, and it has the limits that implies:
    /// everything is lost when the process stops, and nothing is shared between
    /// instances if the API is ever scaled out. Swapping in a database means writing
    /// another IQuoteStore - no caller changes.
    ///
    /// Registered as a singleton, so it is reached concurrently and uses a concurrent
    /// collection rather than a plain Dictionary.
    /// </summary>
    public class InMemoryQuoteStore : IQuoteStore
    {
        private readonly ConcurrentDictionary<Guid, StoredQuote> _quotes = new();
        private readonly TimeProvider _timeProvider;

        public InMemoryQuoteStore(TimeProvider timeProvider) =>
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

        public StoredQuote Save(QuoteRequest request, QuoteResponse response)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var stored = new StoredQuote
            {
                Reference = Guid.NewGuid(),
                RequestedAt = _timeProvider.GetUtcNow(),
                DateOfBirth = request.DateOfBirth ?? default,
                Make = request.Make,
                Model = request.Model,
                InsuranceType = request.InsuranceType ?? default,
                Outcome = response.Outcome,
                Premium = response.Premium
            };

            _quotes[stored.Reference] = stored;

            return stored;
        }

        public StoredQuote Find(Guid reference) =>
            _quotes.TryGetValue(reference, out var stored) ? stored : null;

        public IReadOnlyList<StoredQuote> History(int limit = 50) =>
            _quotes.Values
                .OrderByDescending(quote => quote.RequestedAt)
                .Take(limit)
                .ToList();
    }
}
