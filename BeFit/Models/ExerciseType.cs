using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proszę wprowadzić nazwę ćwiczenia")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Nazwa musi mieć od 5 do 50 znaków.")]
        [Display(Name = "Nazwa ćwiczenia")]
        public string Name { get; set; }

        [Display(Name = "Opis ćwiczenia")]
        public string? Description { get; set; }
    }
}
