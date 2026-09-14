using System;
using System.Linq;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteDetailTests
    {
        [Fact]
        public void EveryAdvertisedMake_HasModelsAvailable()
        {
            var detail = new QuoteService(TimeProvider.System).GetQuoteDetail();

            var makesWithoutModels = detail.Makes
                .Where(make => !detail.Models.Any(spec => spec.Make == make))
                .ToList();

            Assert.Empty(makesWithoutModels);
        }

        [Fact]
        public void EveryModelSpec_BelongsToAnAdvertisedMake()
        {
            var detail = new QuoteService(TimeProvider.System).GetQuoteDetail();

            var orphanedSpecs = detail.Models
                .Where(spec => !detail.Makes.Contains(spec.Make))
                .Select(spec => spec.Make)
                .ToList();

            Assert.Empty(orphanedSpecs);
        }

        [Theory]
        [InlineData("BMW", "X5")]
        [InlineData("BMW", "3 Series")]
        [InlineData("BMW", "5 Series")]
        public void BmwModels_AreAvailable(string make, string model)
        {
            var detail = new QuoteService(TimeProvider.System).GetQuoteDetail();

            var spec = detail.Models.SingleOrDefault(s => s.Make == make);

            Assert.NotNull(spec);
            Assert.Contains(model, spec.Models);
        }
    }
}
