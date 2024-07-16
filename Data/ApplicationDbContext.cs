using Microsoft.EntityFrameworkCore;
using MESDashboard.Models;

namespace MESDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<DowntimeReport> DowntimeReports { get; set; }
        public DbSet<ProductionReport> ProductionReports { get; set; }
        public DbSet<PipeProductionReport> PipeProductionReports { get; set; }
        public DbSet<LadleCompositionData> LadleCompositionData { get; set; }
    }
}