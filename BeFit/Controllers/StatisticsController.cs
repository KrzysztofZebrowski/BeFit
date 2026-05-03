using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Statistics
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-28);

            var recentExercises = await _context.Exercise
                .Include(e => e.Session)
                .Include(e => e.ExerciseType)
                .Where(e => e.Session != null
                         && e.Session.UserId == userId
                         && e.Session.Start >= startDate)
                .ToListAsync();


            var groupedStats = recentExercises
                .GroupBy(e => new { e.ExerciseTypeId, Name = e.ExerciseType?.Name ?? "Nieznane" })
                .Select(g => new ExerciseStatistics
                {
                    ExerciseId = g.Key.ExerciseTypeId,
                    ExerciseName = g.Key.Name,

                    TotalRepetitions = g.Sum(e => e.NumOfSeries * e.NumOfReps),

                    MaxWeight = g.Max(e => e.Weight),

                    AverageWeight = Math.Round(g.Average(e => e.Weight), 2)
                })
                .OrderByDescending(e => e.TotalRepetitions)
                .ToList();

            var view = new Statistics
            {
                StartDate = startDate,
                EndDate = endDate,
                ExerciseStats = groupedStats
            };

            return View(view);
        }
    }
}