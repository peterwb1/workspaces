using System;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class InMemoryQuoteStoreTests
    {
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 9, 14, 10, 0, 0, TimeSpan.Zero);

        private static InMemoryQuoteStore CreateStore(DateTimeOffset? now = null) =>
            new InMemoryQuoteStore(new MovableClock { Now = now ?? Now });

        private static QuoteRequest Request(string make = "Ford", string model = "Focus") =>
            new QuoteRequest
            {
                DateOfBirth = new DateTime(2000, 5, 1),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = make,
                Model = model
            };

        [Fact]
        public void ASavedQuote_CanBeFoundByItsReference()
        {
            var store = CreateStore();

            var saved = store.Save(Request(), QuoteResponse.Offered(200m));
            var found = store.Find(saved.Reference);

            Assert.NotNull(found);
            Assert.Equal(200m, found.Premium);
            Assert.Equal("Ford", found.Make);
            Assert.Equal(QuoteOutcome.Offered, found.Outcome);
        }

        [Fact]
        public void ASavedQuote_RecordsWhatWasAskedFor()
        {
            var store = CreateStore();

            var saved = store.Save(Request("BMW", "X5"), QuoteResponse.Offered(500m));

            Assert.Equal("BMW", saved.Make);
            Assert.Equal("X5", saved.Model);
            Assert.Equal(new DateTime(2000, 5, 1), saved.DateOfBirth);
            Assert.Equal(InsuranceType.FullyComprehensive, saved.InsuranceType);
            Assert.Equal(Now, saved.RequestedAt);
        }

        [Fact]
        public void ADeclinedQuote_IsStoredToo()
        {
            var store = CreateStore();

            var saved = store.Save(
                Request(), QuoteResponse.Declined(QuoteOutcome.DeclinedTooYoung, "Too young."));

            var found = store.Find(saved.Reference);

            Assert.Equal(QuoteOutcome.DeclinedTooYoung, found.Outcome);
            Assert.Null(found.Premium);
        }

        [Fact]
        public void EachSavedQuote_GetsItsOwnReference()
        {
            var store = CreateStore();

            var first = store.Save(Request(), QuoteResponse.Offered(200m));
            var second = store.Save(Request(), QuoteResponse.Offered(200m));

            Assert.NotEqual(first.Reference, second.Reference);
            Assert.Equal(2, store.History().Count);
        }

        [Fact]
        public void AnUnknownReference_IsNotFoundRatherThanThrowing()
        {
            Assert.Null(CreateStore().Find(Guid.NewGuid()));
        }

        [Fact]
        public void History_IsEmptyBeforeAnythingIsSaved()
        {
            Assert.Empty(CreateStore().History());
        }

        [Fact]
        public void History_ReturnsTheNewestFirst()
        {
            var clock = new MovableClock { Now = Now };
            var store = new InMemoryQuoteStore(clock);

            var oldest = store.Save(Request("Ford"), QuoteResponse.Offered(200m));
            clock.Now = Now.AddHours(1);
            var newest = store.Save(Request("BMW"), QuoteResponse.Offered(400m));

            var history = store.History();

            Assert.Equal(newest.Reference, history[0].Reference);
            Assert.Equal(oldest.Reference, history[1].Reference);
        }

        [Fact]
        public void History_CapsHowManyItReturns()
        {
            var store = CreateStore();

            for (var i = 0; i < 5; i++)
            {
                store.Save(Request(), QuoteResponse.Offered(200m));
            }

            Assert.Equal(3, store.History(limit: 3).Count);
            Assert.Equal(5, store.History().Count);
        }

        [Fact]
        public void Save_RejectsMissingArguments()
        {
            var store = CreateStore();

            Assert.Throws<ArgumentNullException>(() => store.Save(null, QuoteResponse.Offered(200m)));
            Assert.Throws<ArgumentNullException>(() => store.Save(Request(), null));
        }

        /// <summary>A clock that stays put until a test moves it on.</summary>
        private sealed class MovableClock : TimeProvider
        {
            public DateTimeOffset Now { get; set; }

            public override DateTimeOffset GetUtcNow() => Now;
        }
    }
}
