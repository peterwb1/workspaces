using System;
using System.Collections.Generic;
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
        private readonly IQuoteStore _quoteStore;

        public QuoteController(IQuoteService quoteService, IQuoteStore quoteStore)
        {
            _quoteService = quoteService;
            _quoteStore = quoteStore;
        }

        [HttpGet]
        public QuoteDetail Get() => _quoteService.GetQuoteDetail();

        // [ApiController] validates the model and returns 400 with the details before
        // this runs, so the old TryValidateModel call here could never fail and its
        // "invalid" branch was unreachable.
        [HttpPost]
        public QuoteResponse Post(QuoteRequest request)
        {
            var response = _quoteService.PerformQuote(request);

            // Handing the reference back is what makes the quote retrievable later.
            response.QuoteReference = _quoteStore.Save(request, response).Reference;

            return response;
        }

        /// <summary>Quotes issued so far, newest first.</summary>
        [HttpGet("history")]
        public IReadOnlyList<StoredQuote> History() => _quoteStore.History();

        /// <summary>
        /// One previously issued quote. The guid constraint stops this route from
        /// swallowing /Quote/history.
        /// </summary>
        [HttpGet("{reference:guid}")]
        public ActionResult<StoredQuote> GetByReference(Guid reference)
        {
            var stored = _quoteStore.Find(reference);

            return stored == null ? NotFound() : stored;
        }
    }
}
