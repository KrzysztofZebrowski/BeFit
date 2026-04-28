using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;

namespace BeFit.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Podaj ciężar.")]
        [Range(0, 1000, ErrorMessage = "Ciężar musi być wartością od {1} do {2}.")]
        [Display(Name = "Ciężar (kg)")]
        public string Weight { get; set; }

        [Required(ErrorMessage = "Podaj liczbę serii.")]
        [Range(1, 50, ErrorMessage = "Liczba serii musi być wartością od {1} do {2}.")]
        [Display(Name = "Liczba Serii")]
        public string NumOfSeries { get; set; }

        [Required(ErrorMessage = "Podaj liczbę powtórzeń.")]
        [Range(1, 100, ErrorMessage = "Liczba powtórzeń musi być wartością od {1} do {2}.")]
        [Display(Name = "Liczba Powtórzeń")]
        public string NumOfReps { get; set; }

        [Required(ErrorMessage = "Wybierz ćwiczenie.")]   
        [Display(Name = "Nazwa Ćwiczenia")]
        public int ExerciseTypeId { get; set; }
        
        [Display(Name = "Nazwa Ćwiczenia")]
        public virtual ExerciseType? ExerciseType { get; set; }

        [Required]
        public int SessionId { get; set; }  
        public virtual Session? Session { get; set; }


    }
}
