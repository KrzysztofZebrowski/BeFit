using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class Session : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proszę podać datę rozpoczęcia")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime Start { get; set; } 
        
        [Required(ErrorMessage = "Proszę podać datę zakończenia")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data zakończenia")]
        public DateTime End { get; set; }   
        
        public string? UserId { get; set; }

        [Display(Name = "Użytkownik")]
        public IdentityUser? User { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (End <= Start)
            {
                errors.Add(new ValidationResult("Data zakończenia musi być późniejsza niż data rozpoczęcia.", new[] { nameof(End) }));
            }

            if (End - Start > TimeSpan.FromHours(12))
            {
                errors.Add(new ValidationResult("Sesja nie może trwać dłużej niż 12 godzin.", new[] { nameof(End) }));
            }

            return errors;
        }

    }
}
