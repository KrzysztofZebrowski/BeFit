using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseStatistics
    {
        public int ExerciseId { get; set; }

        [Display(Name = "Nazwa ćwiczenia")]
        public string ExerciseName { get; set; }
        
        [Display(Name = "Łączna liczba powtórzeń")]
        public int TotalRepetitions { get; set; }
        
        [Display(Name = "Średnia waga")]
        public double AverageWeight { get; set; }

        [Display(Name = "Maksymalna waga")]
        public double MaxWeight { get; set; }
    }

    public class Statistics
    {
        [Display(Name = "Data rozpoczęcia")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime EndDate { get; set; }

        public List<ExerciseStatistics> ExerciseStats { get; set; } = new List<ExerciseStatistics>();
    }

}
