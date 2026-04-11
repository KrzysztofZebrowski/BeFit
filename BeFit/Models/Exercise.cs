using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;

namespace BeFit.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Display(Name ="Ciężar")]
        public string Weight { get; set; }
        [Display(Name = "Liczba Serii")]
        public string NumOfSeries { get; set; }
        [Display(Name = "Liczba Powtórzeń")]
        public string NumOfReps { get; set; }
        [Display(Name = "Nazwa Ćwiczenia")]
        public int ExerciseTypeId { get; set; }
        [Display(Name = "Nazwa Ćwiczenia")]
        public virtual ExerciseType? ExerciseType { get; set; }

        public int SessionId { get; set; }  
        public virtual Session? Session { get; set; }


    }
}
