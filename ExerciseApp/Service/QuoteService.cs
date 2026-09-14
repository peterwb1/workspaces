using System;
using System.Linq;
using ExerciseApp.Model;

namespace ExerciseApp.Service
{
    public class QuoteService : IQuoteService
    {
        public const int MinimumAge = 17;
        public const int MaximumAge = 80;

        /// <summary>
        /// Pricing held as data rather than nested ifs. A null make or model is a
        /// wildcard and the most specific matching row wins, so adding a vehicle or
        /// a whole insurance type is a new row rather than another branch.
        /// These reproduce exactly the premiums the original if-ladder produced.
        /// </summary>
        private static readonly Rate[] Rates =
        {
            new Rate(InsuranceType.FullyComprehensive,     "Ford", null, 200m),
            new Rate(InsuranceType.FullyComprehensive,     "BMW",  "X5", 500m),
            new Rate(InsuranceType.FullyComprehensive,     "BMW",  null, 400m),
            new Rate(InsuranceType.FullyComprehensive,     null,   null, 300m),

            new Rate(InsuranceType.ThirdPartyFireAndTheft, "Ford", null, 180m),
            new Rate(InsuranceType.ThirdPartyFireAndTheft, "BMW",  "X5", 510m),
            new Rate(InsuranceType.ThirdPartyFireAndTheft, "BMW",  null, 400m),
            new Rate(InsuranceType.ThirdPartyFireAndTheft, null,   null, 300m),

            new Rate(InsuranceType.ThirdPartyOnly,         "Ford", null, 180m),
            new Rate(InsuranceType.ThirdPartyOnly,         "Audi", null, 250m),
            new Rate(InsuranceType.ThirdPartyOnly,         null,   null, 300m)
        };

        private readonly TimeProvider _timeProvider;

        // Injected rather than calling DateTime.Now so age boundary tests stay fixed
        // as real time passes.
        public QuoteService(TimeProvider timeProvider) =>
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

        /// <summary>
        /// The vehicles we insure, as one list. Makes are derived from this, so adding
        /// a car is a single edit here rather than two that have to agree.
        /// </summary>
        private static readonly (string Make, string[] Models)[] Catalogue =
        {
            ("Ford", new[] { "Fiesta", "Focus", "Puma", "S Max" }),
            ("Audi", new[] { "A3", "A4", "A5" }),
            ("BMW",  new[] { "X5", "3 Series", "5 Series" })
        };

        public QuoteDetail GetQuoteDetail()
        {
            var quoteDetail = new QuoteDetail();

            foreach (var (make, models) in Catalogue)
            {
                var modelSpec = new ModelSpec { Make = make };
                modelSpec.Models.AddRange(models);
                quoteDetail.Models.Add(modelSpec);
            }

            return quoteDetail;
        }

        public QuoteResponse PerformQuote(QuoteRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // [ApiController] rejects missing fields with a 400 before we are reached,
            // but this method is public and must not rely on that to stay safe.
            if (request.DateOfBirth == null || request.InsuranceType == null)
            {
                return QuoteResponse.Declined(
                    QuoteOutcome.InvalidRequest, "Date of birth and insurance type are both required.");
            }

            var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
            var dateOfBirth = DateOnly.FromDateTime(request.DateOfBirth.Value);

            if (dateOfBirth > today)
            {
                return QuoteResponse.Declined(
                    QuoteOutcome.InvalidRequest, "Date of birth cannot be in the future.");
            }

            var age = AgeInYears(dateOfBirth, today);

            if (age < MinimumAge)
            {
                return QuoteResponse.Declined(
                    QuoteOutcome.DeclinedTooYoung, $"We can only insure drivers aged {MinimumAge} or over.");
            }

            if (age > MaximumAge)
            {
                return QuoteResponse.Declined(
                    QuoteOutcome.DeclinedTooOld, $"We can only insure drivers aged {MaximumAge} or under.");
            }

            var premium = FindPremium(request.InsuranceType.Value, request.Make, request.Model);

            // The old code silently returned 0 for anything it did not recognise,
            // which the client would have displayed as a free policy.
            return premium == null
                ? QuoteResponse.Declined(QuoteOutcome.InvalidRequest, "We do not currently insure that vehicle.")
                : QuoteResponse.Offered(premium.Value);
        }

        /// <summary>
        /// Completed years lived at <paramref name="asAt"/>. Counts a birthday falling
        /// on that day, and relies on DateOnly.AddYears clamping 29 February to the
        /// 28th in non-leap years.
        /// </summary>
        private static int AgeInYears(DateOnly dateOfBirth, DateOnly asAt)
        {
            var age = asAt.Year - dateOfBirth.Year;

            // Their birthday has not come round yet this year.
            if (asAt < dateOfBirth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        /// <summary>The premium for this vehicle, or null if no row covers it.</summary>
        private static decimal? FindPremium(InsuranceType insuranceType, string make, string model) =>
            Rates
                .Where(rate => rate.InsuranceType == insuranceType)
                .Where(rate => Matches(rate.Make, make) && Matches(rate.Model, model))
                .OrderByDescending(rate => rate.Specificity)
                .Select(rate => (decimal?)rate.Premium)
                .FirstOrDefault();

        private static bool Matches(string pattern, string value) =>
            pattern == null || string.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);

        private sealed record Rate(InsuranceType InsuranceType, string Make, string Model, decimal Premium)
        {
            /// <summary>A model beats a make, which beats the fallback row.</summary>
            public int Specificity => (Make == null ? 0 : 1) + (Model == null ? 0 : 2);
        }
    }
}
