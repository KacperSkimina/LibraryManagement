using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class PatronsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatronsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patrons
        public async Task<IActionResult> Index()
        {
            var patrons = await _context.Patrons
                .Include(p => p.Borrows)
                    .ThenInclude(b => b.Book)
                        .ThenInclude(book => book.Author)
                .OrderBy(p => p.LastName)
                .ToListAsync();

            return View(patrons);
        }

        // GET: Patrons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patrons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patron patron)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patron);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patron);
        }
    }
}