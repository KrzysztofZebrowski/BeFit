using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exercises
        public async Task<IActionResult> Index(int? sessionId)
        {
            if (sessionId == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var applicationDbContext = _context.Exercise
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .Where(e => e.Session != null && e.Session.UserId == userId)
                .AsQueryable();

            if (sessionId.HasValue)
            {
                applicationDbContext = applicationDbContext
                    .Where(e => e.SessionId == sessionId.Value);
                ViewBag.CurrentSessionId = sessionId;

                // Pobieranie informacji o sesji, aby wyświetlić datę w widoku index
                var session = await _context.Session.FirstOrDefaultAsync(s => s.Id == sessionId.Value);
                
                if (session == null || session.UserId != userId)
                {
                    return Forbid();
                }

                if (session != null)
                {
                    ViewBag.CurrentSessionStart = session.Start;
                }
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (exercise == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (exercise.Session == null || exercise.Session.UserId != userId)
            {
                return Forbid();
            }

            return View(exercise);
        }

        // GET: Exercises/Create
        public async Task<IActionResult> Create(int? sessionId)
        {
            if (sessionId == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, 
                "Id", 
                "Name");

            var session = await _context.Session.FindAsync(sessionId);

            if (session == null || session.UserId != userId) return Forbid();

            ViewBag.CurrentSessionId = sessionId;
            ViewBag.SessionDate = session.Start;

            /*
            ViewData["SessionId"] = new SelectList(_context.Session
                .Where(s => s.UserId == userId), 
                "Id", 
                "Start");
            */

            return View();
            
        }

        // POST: Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise exercise)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // weryfikacja, czy wybrana sesja należy do użytkownika
            var session = await _context.Session.FindAsync(exercise.SessionId);
            
            if (session == null || session.UserId != userId)
            {
                ModelState.AddModelError("SessionId", "Wybrana sesja jest nieprawidłowa.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {sessionId = exercise.SessionId});
            }

            if (session != null)
            {
                ViewBag.CurrentSessionId = exercise.SessionId;
                ViewBag.SessionDate = session.Start;
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, 
                "Id", 
                "Name", 
                exercise.ExerciseTypeId);

            /*
            ViewData["SessionId"] = new SelectList(_context.Session
                .Where(s => s.UserId == userId), 
                "Id", 
                "Start", 
                exercise.SessionId);
            */
            
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(e => e.Session)
                .Include(e => e.ExerciseType)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (exercise == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session == null || exercise.Session.UserId != userId)
            {
                return Forbid();
            }

            ViewBag.CurrentSessionId = exercise.SessionId;
            ViewBag.SessionDate = exercise.Session.Start;

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, 
                "Id", 
                "Name", 
                exercise.ExerciseTypeId);

            /*
            ViewData["SessionId"] = new SelectList(_context.Session
                .Where(s => s.UserId == userId),
                "Id",
                "Start",
                exercise.SessionId);
            */

            return View(exercise);
        }

        // POST: Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingExercise = await _context.Exercise
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingExercise == null)
            {
                return NotFound();
            }
            
            if (existingExercise.Session == null || existingExercise.Session.UserId != userId)
            {
                return Forbid();
            }

            var newSession = await _context.Session.FindAsync(exercise.SessionId);
            if (newSession == null) 
            {
                return NotFound();
            } 
            
            if (newSession.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tylko dozwolone pola
                    existingExercise.Weight = exercise.Weight;
                    existingExercise.NumOfSeries = exercise.NumOfSeries;
                    existingExercise.NumOfReps = exercise.NumOfReps;
                    existingExercise.ExerciseTypeId = exercise.ExerciseTypeId;
                    // existingExercise.SessionId = exercise.SessionId;

                    _context.Update(existingExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { sessionId = existingExercise.SessionId });
            }

            ViewBag.CurrentSessionId = existingExercise.SessionId;
            ViewBag.SessionDate = existingExercise.Session.Start;

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, 
                "Id", 
                "Name", 
                exercise.ExerciseTypeId);
            
            /*
            ViewData["SessionId"] = new SelectList(_context.Session
                .Where(s => s.UserId == userId), 
                "Id", 
                "Start", 
                exercise.SessionId);
            */

            return View(exercise);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (exercise == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || exercise.Session.UserId != userId)
            {
                return Forbid();
            }

            ViewBag.CurrentSessionId = exercise.SessionId;
            ViewBag.SessionDate = exercise.Session.Start;

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (exercise == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (exercise.Session == null || exercise.Session.UserId != userId)
            {
                return Forbid();
            }

            var sessionIdToReturn = exercise.SessionId;

            _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { sessionId = sessionIdToReturn });
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.Id == id);
        }
    }
}
