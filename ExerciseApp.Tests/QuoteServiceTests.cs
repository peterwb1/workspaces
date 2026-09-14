using System;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteServiceTests
    {
        // Every test quotes as at this date, so the ages below keep their meaning
        // rather than drifting as real time passes.
        private static readonly DateOnly QuoteDate = new DateOnly(2026, 9, 14);

        private static QuoteService CreateService() => new QuoteService(new FixedClock(QuoteDate));

        private static QuoteRequest Request(
            string dateOfBirth,
            InsuranceType insuranceType = InsuranceType.FullyComprehensive,
            string make = "Ford",
            string model = "Focus") =>
            new QuoteRequest
            {
                DateOfBirth = DateTime.Parse(dateOfBirth),
                InsuranceType = insuranceType,
                Make = make,
                Model = model
            };

        [Fact]
        public void WhenDetailsAreProvided_AQuoteIsProduced()
        {
            var response = CreateService().PerformQuote(Request("2000-05-01"));

            Assert.Equal(QuoteOutcome.Offered, response.Outcome);
            Assert.Equal(200m, response.Premium);
            Assert.Null(response.Message);
        }

        [Theory]
        // Turns 17 on the day of the quote - the first day they can be insured.
        [InlineData("2009-09-14")]
        // Turns 80 on the day of the quote, and 80 is still inside the range.
        [InlineData("1946-09-14")]
        // 80 years old, one day short of their 81st birthday.
        [InlineData("1945-09-15")]
        public void DriversInsideTheAgeRange_ReceiveAQuote(string dateOfBirth)
        {
            var response = CreateService().PerformQuote(Request(dateOfBirth));

            Assert.Equal(QuoteOutcome.Offered, response.Outcome);
            Assert.Equal(200m, response.Premium);
        }

        [Theory]
        // One day short of their 17th birthday.
        [InlineData("2009-09-15")]
        [InlineData("2015-01-01")]
        public void DriversUnderSeventeen_AreDeclined(string dateOfBirth)
        {
            var response = CreateService().PerformQuote(Request(dateOfBirth));

            Assert.Equal(QuoteOutcome.DeclinedTooYoung, response.Outcome);
            Assert.Null(response.Premium);
            Assert.NotNull(response.Message);
        }

        [Theory]
        // Turns 81 on the day of the quote.
        [InlineData("1945-09-14")]
        [InlineData("1920-01-01")]
        public void DriversOverEighty_AreDeclined(string dateOfBirth)
        {
            var response = CreateService().PerformQuote(Request(dateOfBirth));

            Assert.Equal(QuoteOutcome.DeclinedTooOld, response.Outcome);
            Assert.Null(response.Premium);
            Assert.NotNull(response.Message);
        }

        [Theory]
        // A 29 February birthday is treated as falling on the 28th in non-leap years,
        // so this driver reaches 17 during February 2026 either way.
        [InlineData("2009-02-28", "2026-02-28")]
        [InlineData("2008-02-29", "2026-02-28")]
        public void ALeapDayBirthday_IsHandled(string dateOfBirth, string quoteDate)
        {
            var service = new QuoteService(new FixedClock(DateOnly.Parse(quoteDate)));

            Assert.Equal(QuoteOutcome.Offered, service.PerformQuote(Request(dateOfBirth)).Outcome);
        }

        [Fact]
        public void ADateOfBirthInTheFuture_IsRejectedRatherThanThrowing()
        {
            var response = CreateService().PerformQuote(Request("2030-01-01"));

            Assert.Equal(QuoteOutcome.InvalidRequest, response.Outcome);
            Assert.Null(response.Premium);
        }

        [Fact]
        public void AMissingDateOfBirth_IsRejectedRatherThanThrowing()
        {
            var request = Request("2000-05-01");
            request.DateOfBirth = null;

            Assert.Equal(QuoteOutcome.InvalidRequest, CreateService().PerformQuote(request).Outcome);
        }

        [Fact]
        public void AnUnlistedVehicle_IsPricedAtTheFallbackRate()
        {
            // Every insurance type has a wildcard row, so an unknown make is priced at
            // the fallback rather than refused. This pins that intent.
            var response = CreateService().PerformQuote(Request("2000-05-01", make: "Tesla", model: "Model 3"));

            Assert.Equal(QuoteOutcome.Offered, response.Outcome);
            Assert.Equal(300m, response.Premium);
        }

        // Locks in every premium the original if-ladder produced, so replacing the
        // branching with a lookup is provably behaviour preserving.
        [Theory]
        [InlineData(InsuranceType.FullyComprehensive,     "Ford", "Focus",    200)]
        [InlineData(InsuranceType.FullyComprehensive,     "BMW",  "X5",       500)]
        [InlineData(InsuranceType.FullyComprehensive,     "BMW",  "3 Series", 400)]
        [InlineData(InsuranceType.FullyComprehensive,     "Audi", "A3",       300)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "Ford", "Fiesta",   180)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "BMW",  "X5",       510)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "BMW",  "5 Series", 400)]
        [InlineData(InsuranceType.ThirdPartyFireAndTheft, "Audi", "A4",       300)]
        [InlineData(InsuranceType.ThirdPartyOnly,         "Ford", "Puma",     180)]
        [InlineData(InsuranceType.ThirdPartyOnly,         "Audi", "A5",       250)]
        [InlineData(InsuranceType.ThirdPartyOnly,         "BMW",  "X5",       300)]
        public void Pricing_MatchesTheOriginalRates(
            InsuranceType insuranceType, string make, string model, decimal expected)
        {
            var response = CreateService().PerformQuote(Request("2000-05-01", insuranceType, make, model));

            Assert.Equal(expected, response.Premium);
        }

        [Fact]
        public void Pricing_IsCaseInsensitive()
        {
            var response = CreateService().PerformQuote(
                Request("2000-05-01", InsuranceType.FullyComprehensive, "bmw", "x5"));

            Assert.Equal(500m, response.Premium);
        }

        /// <summary>A clock pinned to a known date.</summary>
        private sealed class FixedClock : TimeProvider
        {
            private readonly DateTimeOffset _now;

            public FixedClock(DateOnly today) =>
                _now = new DateTimeOffset(today.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

            public override DateTimeOffset GetUtcNow() => _now;

            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }
    }
}
