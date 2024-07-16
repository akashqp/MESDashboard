using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MESDashboard.Data;
using MESDashboard.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MESDashboard.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.SubmittedAt = DateTime.Now;
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("SubmissionSuccess", new { id = ticket.Id });
            }
            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> SubmittedTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return View(tickets);
        }

        [HttpGet]
        public async Task<IActionResult> SubmissionSuccess(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }
    }
}
