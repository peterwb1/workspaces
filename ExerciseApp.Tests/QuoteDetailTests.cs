using System;
using System.Linq;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteDetailTests
    {
        private static QuoteService CreateService() => new QuoteService(new FixedClock());

        [Fact]
        public void Makes_AreDerivedFromTheModelSpecs()
        {
            // The BMW bug was possible because makes and models were maintained
            // separately. Makes is now a projection of Models, so this holds by
            // construction rather than by anyone remembering to keep them in step.
            var detail = CreateService().GetQuoteDetail();

            Assert.Equal(detail.Models.Select(spec => spec.Make), detail.Makes);
        }

        [Fact]
        public void Makes_ContainNoDuplicates()
        {
            var detail = CreateService().GetQuoteDetail();

            Assert.Equal(detail.Makes.Distinct().Count(), detail.Makes.Count);
        }

        [Fact]
        public void EveryMakeWeAdvertise_HasModelsToChooseFrom()
        {
            var detail = CreateService().GetQuoteDetail();

            Assert.All(detail.Models, spec => Assert.NotEmpty(spec.Models));
        }

        [Theory]
        [InlineData("BMW", "X5")]
        [InlineData("BMW", "3 Series")]
        [InlineData("BMW", "5 Series")]
        [InlineData("Ford", "Focus")]
        [InlineData("Audi", "A3")]
        public void TheCatalogue_ListsTheExpectedVehicles(string make, string model)
        {
            var detail = CreateService().GetQuoteDetail();

            var spec = detail.Models.SingleOrDefault(s => s.Make == make);

            Assert.NotNull(spec);
            Assert.Contains(model, spec.Models);
        }

        [Fact]
        public void EveryVehicleWeAdvertise_CanActuallyBeQuoted()
        {
            // Guards the catalogue and the rate rows against drifting apart: anything
            // offered in the dropdowns must come back with a premium.
            var service = CreateService();
            var detail = service.GetQuoteDetail();

            var combinations =
                from spec in detail.Models
                from model in spec.Models
                from type in detail.InsuranceTypes
                select new QuoteRequest
                {
                    DateOfBirth = new DateTime(2000, 5, 1),
                    InsuranceType = type.Type,
                    Make = spec.Make,
                    Model = model
                };

            Assert.All(combinations, request =>
            {
                var response = service.PerformQuote(request);

                Assert.Equal(QuoteOutcome.Offered, response.Outcome);
                Assert.NotNull(response.Premium);
            });
        }

        private sealed class FixedClock : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() =>
                new DateTimeOffset(2026, 9, 14, 0, 0, 0, TimeSpan.Zero);

            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }
    }
}
