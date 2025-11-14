using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DynamicForm.Data;
using DynamicForm.Models;

namespace DynamicForm.Controllers
{
    public class ResponsesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Responses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Responses.Include(r => r.Form).Include(r => r.Question);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Responses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _context.Responses
                .Include(r => r.Form)
                .Include(r => r.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        // GET: Responses/Create
        //public IActionResult Create()
        //{
        //    ViewData["FormId"] = new SelectList(_context.Forms, "Id", "Title");
        //    ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text");
        //    return View();
        //}
        public async Task<IActionResult> Create(int formId)
        {
            var form = await _context.Forms
                .Include(f => f.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(f => f.Id == formId);

            if (form == null) return NotFound();

            ViewBag.Form = form;
            return View();
        }

        // POST: Responses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FormId,QuestionId,RespondentId,SubmittedDate,AnswerText,AnswerValue")] Response response)
        {
            if (ModelState.IsValid)
            {
                _context.Add(response);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FormId"] = new SelectList(_context.Forms, "Id", "Title", response.FormId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", response.QuestionId);
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int formId, IFormCollection answers)
        {
            var form = await _context.Forms.FirstOrDefaultAsync(f => f.Id == formId);
            if (form == null) return NotFound();

            foreach (var key in answers.Keys.Where(k => k.StartsWith("answers[")))
            {
                var qIdStr = key.Substring(8, key.Length - 9);
                if (!int.TryParse(qIdStr, out int questionId)) continue;

                var question = await _context.Questions.FindAsync(questionId);
                if (question == null) continue;

                var answerValues = answers[key].ToArray();

                foreach (var val in answerValues)
                {
                    var response = new Response
                    {
                        FormId = formId,
                        QuestionId = questionId,
                        RespondentId = 1,
                        SubmittedDate = DateTime.Now
                    };

                    if (question.Type == "Text" || question.Type == "Dropdown")
                    {
                        response.AnswerText = val;
                    }
                    else if (question.Type == "Rating" || question.Type == "MultipleChoice")
                    {
                        response.AnswerValue = int.TryParse(val, out int v) ? v : (int?)null;
                    }

                    _context.Responses.Add(response);
                    await _context.SaveChangesAsync();

                    // Handle multi-select (checkbox)
                    if (question.Type == "Checkbox" && int.TryParse(val, out int optId))
                    {
                        _context.ResponseOptions.Add(new ResponseOption
                        {
                            ResponseId = response.Id,
                            OptionId = optId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ThankYou");
        }

        // GET: Responses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _context.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            ViewData["FormId"] = new SelectList(_context.Forms, "Id", "Title", response.FormId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", response.QuestionId);
            return View(response);
        }

        // POST: Responses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FormId,QuestionId,RespondentId,SubmittedDate,AnswerText,AnswerValue")] Response response)
        {
            if (id != response.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(response);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResponseExists(response.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FormId"] = new SelectList(_context.Forms, "Id", "Title", response.FormId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", response.QuestionId);
            return View(response);
        }

        // GET: Responses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _context.Responses
                .Include(r => r.Form)
                .Include(r => r.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        // POST: Responses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResponseExists(int id)
        {
            return _context.Responses.Any(e => e.Id == id);
        }
    }
}
