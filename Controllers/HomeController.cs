using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.BooksCount = _context.Books.Count();
            ViewBag.AuthorsCount = _context.Authors.Count();
            ViewBag.ActiveBorrowsCount = _context.Borrows.Count(b => b.ReturnDate == null);
            ViewBag.PatronsCount = _context.Patrons.Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ApiTest()
        {
            return View();
        }
    }
}