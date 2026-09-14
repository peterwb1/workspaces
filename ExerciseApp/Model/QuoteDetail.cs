using System;
using System.Linq;
using System.Collections.Generic;
using ExerciseApp.Helpers;

namespace ExerciseApp.Model
{
    public class QuoteDetail
    {
        public QuoteDetail()
        {
            Makes = new List<string>();
            Models = new List<ModelSpec>();
            InsuranceTypes = Enum.GetValues(typeof(InsuranceType)).Cast<InsuranceType>().Select(it => new InsuranceTypePair{Type=it, Description=it.GetEnumDescription()}).ToList();

        }
        public List<InsuranceTypePair> InsuranceTypes { get; set; }
        public List<string> Makes { get; private set; }
        public List<ModelSpec> Models { get; private set; }
    }
}


