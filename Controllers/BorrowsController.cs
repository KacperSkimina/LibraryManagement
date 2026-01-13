using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrows
        public async Task<IActionResult> Index()
        {
            var borrows = await _context.Borrows
                .Include(b => b.Book)
                    .ThenInclude(book => book.Author)
                .Include(b => b.Patron)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            // DEBUG
            foreach (var borrow in borrows)
            {
                Console.WriteLine($"Borrow #{borrow.Id}: Book: {borrow.Book?.Title}, Patron: {borrow.Patron?.FullName}");
            }

            return View(borrows);
        }

        // GET: Borrows/Create
        public async Task<IActionResult> Create()
        {
            // Pobierz dostępne książki (nie wypożyczone)
            var availableBooks = await _context.Books
                .Where(b => b.Borrows.All(br => br.ReturnDate != null) || !b.Borrows.Any())
                .Include(b => b.Author)
                .ToListAsync();

            var patrons = await _context.Patrons.ToListAsync();

            ViewBag.Books = availableBooks;
            ViewBag.Patrons = patrons;
            ViewBag.DueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"); // Domyślnie 2 tygodnie

            return View();
        }

        // POST: Borrows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Borrow borrow)
        {
            // DEBUG: Sprawdź co przychodzi
            Console.WriteLine($"BookId: {borrow.BookId}");
            Console.WriteLine($"PatronId: {borrow.PatronId}");
            Console.WriteLine($"DueDate: {borrow.DueDate}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Ustaw datę wypożyczenia
            borrow.BorrowDate = DateTime.Now;

            // Walidacja
            if (borrow.DueDate <= borrow.BorrowDate)
            {
                ModelState.AddModelError("DueDate", "Due date must be after today");
            }

            // Sprawdź czy książka jest dostępna
            var isBookAvailable = !await _context.Borrows
                .AnyAsync(b => b.BookId == borrow.BookId && b.ReturnDate == null);

            if (!isBookAvailable)
            {
                ModelState.AddModelError("BookId", "This book is currently borrowed");
            }

            // Wyłącz walidację dla właściwości nawigacyjnych
            ModelState.Remove("Book");
            ModelState.Remove("Patron");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(borrow);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Borrow saved with ID: {borrow.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving borrow: {ex.Message}");
                    ModelState.AddModelError("", "Error saving borrow. Please try again.");
                }
            }
            else
            {
                // Pokaż błędy walidacji
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            // Załaduj dane ponownie dla widoku
            ViewBag.Books = await _context.Books
                .Include(b => b.Author)
                .ToListAsync();
            ViewBag.Patrons = await _context.Patrons.ToListAsync();
            ViewBag.DueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");

            return View(borrow);
        }

        // GET: Borrows/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var borrow = await _context.Borrows
                .Include(b => b.Book)
                .Include(b => b.Patron)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrow == null || borrow.ReturnDate != null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // POST: Borrows/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var borrow = await _context.Borrows.FindAsync(id);
            if (borrow != null && borrow.ReturnDate == null)
            {
                borrow.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}