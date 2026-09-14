using System;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteServiceTests
    {
        [Fact]
        public void WhenDetailsAreProvided_AQuoteIsProduced()
        {
            var qs = new QuoteService();
            var quoteResult = qs.PerformQuote(new Model.QuoteRequest 
                { DateOfBirth = new DateTime(2000, 05, 01),
                    InsuranceType = Model.InsuranceType.FullyComprehensive, 
                    Make= "Ford", 
                    Model="Focus"});
            Assert.Equal(200, quoteResult);
        }

    }
}
