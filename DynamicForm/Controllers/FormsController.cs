using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicForm.Data;
using DynamicForm.Models;
using DynamicForm.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DynamicForm.Controllers
{
    public class FormsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Forms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Forms.ToListAsync());
        }

        // GET: Forms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // GET: Forms/Create
        public IActionResult Create()
        {
            ViewBag.QuestionTypes = QuestionTypesSelectList();
            return View(new FormCreateViewModel());
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormCreateViewModel vm)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.QuestionTypes = QuestionTypesSelectList();
            //    return View(vm);
            //}

            // ---- 1. Persist the Form ----------------------------------------
            var form = new Form
            {
                CompanyId = vm.CompanyId,
                Title = vm.Title,
                Description = vm.Description,
                CreatedBy = User.Identity?.Name ?? "Anonymous",
                CreatedDate = DateTime.Now,
                EndDate = vm.EndDate,
                IsActive = vm.IsActive
            };

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();   // <-- form.Id is now populated

            // ---- 2. Persist Questions (if any) -------------------------------
            if (vm.Questions != null && vm.Questions.Any())
            {
                foreach (var qvm in vm.Questions)
                {
                    var question = new Question
                    {
                        FormId = form.Id,
                        Text = qvm.Text,
                        Type = qvm.Type,
                        IsRequired = qvm.IsRequired,
                        OrderIndex = qvm.OrderIndex,
                        MinValue = qvm.MinValue,
                        MaxValue = qvm.MaxValue
                    };
                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();   // <-- question.Id needed for options

                    // ---- Options (from textarea) --------------------------------
                    if (!string.IsNullOrWhiteSpace(qvm.OptionsText))
                    {
                        var lines = qvm.OptionsText
                                       .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => x.Trim())
                                       .Where(x => !string.IsNullOrEmpty(x));

                        int order = 0;
                        foreach (var line in lines)
                        {
                            _context.Options.Add(new Option
                            {
                                QuestionId = question.Id,
                                Text = line,
                                OrderIndex = order++
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------
        // Helper: SelectList for question types
        // --------------------------------------------------------------------
        private SelectList QuestionTypesSelectList() => new(
            new[] { "Text", "MultipleChoice", "Checkbox", "Rating", "Dropdown" });

        // GET: Forms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Title,Description,CreatedBy,CreatedDate,EndDate,IsActive")] Form form)
        {
            if (id != form.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(form);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormExists(form.Id))
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
            return View(form);
        }

        // GET: Forms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // POST: Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormExists(int id)
        {
            return _context.Forms.Any(e => e.Id == id);
        }
    }
}
