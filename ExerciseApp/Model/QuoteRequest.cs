using System;
using System.ComponentModel.DataAnnotations;

namespace ExerciseApp.Model
{
    public class QuoteRequest
    {
        [DataType(DataType.Date)]
        [Required]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        [Required(AllowEmptyStrings =false)]
        public string Make { get; set; }

        [StringLength(10)]
        [Required(AllowEmptyStrings = false)]
        public string Model { get; set; }

        [Required]
        public InsuranceType? InsuranceType { get; set; }
    }
}
