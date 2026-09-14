using System;
using System.Collections.Generic;
using System.Linq;
using ExerciseApp.Helpers;

namespace ExerciseApp.Model
{
    public class QuoteDetail
    {
        public QuoteDetail()
        {
            Models = new List<ModelSpec>();
            InsuranceTypes = Enum.GetValues(typeof(InsuranceType)).Cast<InsuranceType>()
                .Select(it => new InsuranceTypePair { Type = it, Description = it.GetEnumDescription() })
                .ToList();
        }

        public List<InsuranceTypePair> InsuranceTypes { get; set; }

        /// <summary>
        /// Derived from <see cref="Models"/> rather than maintained alongside it.
        /// The two were previously independent lists, which is how a make could be
        /// advertised while its models were filed under a misspelling and the model
        /// dropdown silently came back empty. They can no longer disagree.
        /// </summary>
        public IReadOnlyList<string> Makes => Models.Select(spec => spec.Make).ToList();

        public List<ModelSpec> Models { get; private set; }
    }
}
