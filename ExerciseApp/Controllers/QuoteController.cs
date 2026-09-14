using ExerciseApp.Model;
using ExerciseApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExerciseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService) => _quoteService = quoteService;

        [HttpGet]
        public QuoteDetail Get() => _quoteService.GetQuoteDetail();

        // [ApiController] validates the model and returns 400 with the details before
        // this runs, so the old TryValidateModel call here could never fail and its
        // "invalid" branch was unreachable.
        [HttpPost]
        public QuoteResponse Post(QuoteRequest request) => _quoteService.PerformQuote(request);
    }
}
