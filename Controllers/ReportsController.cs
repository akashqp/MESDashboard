using Microsoft.AspNetCore.Mvc;
using MESDashboard.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MESDashboard.Controllers
{
    // [Route("reports")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ReportsController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public IActionResult Production()
        {
            var reports = _context.ProductionReports.ToList();
            return View(reports);
        }

        public IActionResult Downtime()
        {
            var reports = _context.DowntimeReports.ToList();
            return View(reports);
        }

        public async Task<IActionResult> ProductionAnalysis()
        {

            // var reports = await _context.PipeProductionReports.ToListAsync();

            // Console.WriteLine("Fetching rejection reasons from API");
            // var rejectionReasons = await _httpClient.GetAsync("http://127.0.0.1:5000/rejection_reasons");

            Console.WriteLine("Fetching monthly rejection reasons from API");
            var monthlyRejectionReasons = await _httpClient.GetAsync("http://127.0.0.1:5000/monthly_rejection_reasons/months");

            // Console.WriteLine("Fetching monthly rejection rates from API");
            // var monthlyRejectionRates = await _httpClient.GetAsync("http://127.0.0.1:5000/monthly_rejection_rates");

            // Console.WriteLine("Fetching correlation analysis from API");
            // var Correlation_analysis = await _httpClient.GetAsync("http://127.0.0.1:5000/correlation_analysis");
            
            // Console.WriteLine("Fetching feature importance from API");
            // var Feature_importance = await _httpClient.GetAsync("http://127.0.0.1:5000/feature_importance");

            // ViewBag.RejectionReasons = rejectionReasons.Content.ReadAsStringAsync().Result;
            ViewBag.MonthlyRejectionReasons = monthlyRejectionReasons.Content.ReadAsStringAsync().Result;
            // ViewBag.MonthlyRejectionRate = monthlyRejectionRates.Content.ReadAsStringAsync().Result;
            // ViewBag.CorrelationAnalysis = Correlation_analysis.Content.ReadAsStringAsync().Result;
            // ViewBag.FeatureImportance = Feature_importance.Content.ReadAsStringAsync().Result;

            return View();
        }

        public async Task<IActionResult> MonthlyRejectionRates()
        {
            Console.WriteLine("Fetching monthly rejection rates from API");
            var monthlyRejectionRates = await _httpClient.GetAsync("http://127.0.0.1:5000/monthly_rejection_rates/months");

            ViewBag.MonthlyRejectionRate = monthlyRejectionRates.Content.ReadAsStringAsync().Result;

            return View();
        }
    }
}
