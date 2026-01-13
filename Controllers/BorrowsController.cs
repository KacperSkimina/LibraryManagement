using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class BorrowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrows
        // Admin widzi wszystkich, klient tylko swoje wypożyczenia
        public async Task<IActionResult> Index()
        {
            var query = _context.Borrows
                .Include(b => b.Book).ThenInclude(book => book.Author)
                .Include(b => b.Patron)
                .AsQueryable();

            if (!User.IsInRole("Administrator"))
            {
                var userEmail = User.Identity.Name;
                query = query.Where(b => b.Patron.Email == userEmail);
            }

            var borrows = await query.OrderByDescending(b => b.BorrowDate).ToListAsync();
            return View(borrows);
        }

        // GET: Borrows/Create
        public async Task<IActionResult> Create()
        {
            var availableBooks = await _context.Books
                .Where(b => b.Borrows.All(br => br.ReturnDate != null) || !b.Borrows.Any())
                .Include(b => b.Author).ToListAsync();

            List<Patron> patrons;

            if (User.IsInRole("Administrator"))
            {
                patrons = await _context.Patrons.ToListAsync();
            }
            else
            {
                var userEmail = User.Identity.Name;
                patrons = await _context.Patrons.Where(p => p.Email == userEmail).ToListAsync();

                // ZABEZPIECZENIE: Jeśli klient nie ma profilu Patron o tym samym mailu
                if (!patrons.Any())
                {
                    // Używamy TempData do wyświetlenia komunikatu (upewnij się, że masz to w widoku lub Home)
                    return Content("Błąd: Twoje konto nie jest powiązane z profilem Czytelnika. Poproś Admina o dodanie Twojego maila w zakładce Czytelnicy.");
                }
            }

            ViewBag.Books = availableBooks;
            ViewBag.Patrons = patrons;
            ViewBag.DueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");

            return View();
        }

        // POST: Borrows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Borrow borrow)
        {
            borrow.BorrowDate = DateTime.Now;

            if (borrow.DueDate <= borrow.BorrowDate)
                ModelState.AddModelError("DueDate", "Data zwrotu musi być w przyszłości.");

            var isBookAvailable = !await _context.Borrows
                .AnyAsync(b => b.BookId == borrow.BookId && b.ReturnDate == null);

            if (!isBookAvailable)
                ModelState.AddModelError("BookId", "Ta książka jest już wypożyczona.");

            ModelState.Remove("Book");
            ModelState.Remove("Patron");

            if (ModelState.IsValid)
            {
                _context.Add(borrow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // W razie błędu przeładuj listy
            ViewBag.Books = await _context.Books.Include(b => b.Author).ToListAsync();
            ViewBag.Patrons = User.IsInRole("Administrator")
                ? await _context.Patrons.ToListAsync()
                : await _context.Patrons.Where(p => p.Email == User.Identity.Name).ToListAsync();

            return View(borrow);
        }

        // GET: Borrows/Return/5
        [Authorize(Roles = "Administrator")] // TYLKO ADMIN MOŻE ZROBIĆ ZWROT
        public async Task<IActionResult> Return(int id)
        {
            var borrow = await _context.Borrows
                .Include(b => b.Book)
                .Include(b => b.Patron)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrow == null || borrow.ReturnDate != null) return NotFound();

            return View(borrow);
        }

        // POST: Borrows/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")] // TYLKO ADMIN MOŻE POTWIERDZIĆ
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