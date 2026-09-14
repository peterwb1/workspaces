using System;
using ExerciseApp.Controllers;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ExerciseApp.Tests
{
    /// <summary>
    /// Exercises the controller against real collaborators. This is only possible
    /// because the controller takes its dependencies through its constructor - the
    /// original newed up its service internally and could not be tested at all.
    /// </summary>
    public class QuoteControllerTests
    {
        private static readonly DateOnly QuoteDate = new DateOnly(2026, 9, 14);

        private static QuoteController CreateController(out IQuoteStore store)
        {
            var clock = new FixedClock(QuoteDate);
            store = new InMemoryQuoteStore(clock);
            return new QuoteController(new QuoteService(clock), store);
        }

        private static QuoteRequest Request(string dateOfBirth = "2000-05-01") =>
            new QuoteRequest
            {
                DateOfBirth = DateTime.Parse(dateOfBirth),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

        [Fact]
        public void Post_ReturnsAQuoteAndAReferenceForIt()
        {
            var controller = CreateController(out _);

            var response = controller.Post(Request());

            Assert.Equal(QuoteOutcome.Offered, response.Outcome);
            Assert.Equal(200m, response.Premium);
            Assert.NotNull(response.QuoteReference);
        }

        [Fact]
        public void AQuoteJustIssued_CanBeRetrievedByItsReference()
        {
            var controller = CreateController(out _);

            var reference = controller.Post(Request()).QuoteReference!.Value;
            var retrieved = controller.GetByReference(reference);

            Assert.Equal(200m, retrieved.Value.Premium);
            Assert.Equal(reference, retrieved.Value.Reference);
        }

        [Fact]
        public void ADeclinedQuote_IsStoredAndRetrievableToo()
        {
            var controller = CreateController(out _);

            var response = controller.Post(Request(dateOfBirth: "2015-01-01"));
            var retrieved = controller.GetByReference(response.QuoteReference!.Value);

            Assert.Equal(QuoteOutcome.DeclinedTooYoung, response.Outcome);
            Assert.Equal(QuoteOutcome.DeclinedTooYoung, retrieved.Value.Outcome);
            Assert.Null(retrieved.Value.Premium);
        }

        [Fact]
        public void AnUnknownReference_ReturnsNotFound()
        {
            var controller = CreateController(out _);

            var retrieved = controller.GetByReference(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(retrieved.Result);
        }

        [Fact]
        public void History_ListsTheQuotesThatHaveBeenIssued()
        {
            var controller = CreateController(out _);

            controller.Post(Request());
            controller.Post(Request(dateOfBirth: "2015-01-01"));

            Assert.Equal(2, controller.History().Count);
        }

        [Fact]
        public void History_IsEmptyBeforeAnyQuoteIsRequested()
        {
            var controller = CreateController(out _);

            Assert.Empty(controller.History());
        }

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
