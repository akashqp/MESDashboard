using System.ComponentModel.DataAnnotations;

namespace MESDashboard.Models
{
    public class DowntimeReport
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Machine { get; set; }
        public string Reason { get; set; }
        public TimeSpan DowntimeDuration { get; set; }
    }

    public class ProductionReport
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Product { get; set; }
        public int QuantityProduced { get; set; }
        public int QuantityDefective { get; set; }
    }

    public class PipeProductionReport
    {
        [Key]
        public int Id { get; set; }
        public string PipeNo { get; set; }
        public string LadleNo { get; set; }
        public string State { get; set; }
        public DateTime ReportDate { get; set; }
        public string Shift { get; set; }
        public string MachineId { get; set; }
        public string OperatorId { get; set; }
        public string? Reason { get; set; }
        public string? SubReason { get; set; }

        // Navigation property
        public virtual LadleCompositionData LadleCompositionData { get; set; }
    }

    public class LadleCompositionData
    {
        [Key]
        public string LadleNo { get; set; }
        public decimal C { get; set; }
        public decimal Si { get; set; }
        public decimal Mn { get; set; }
        public decimal P { get; set; }
        public decimal S { get; set; }
        public decimal Ti { get; set; }
        public decimal Mg { get; set; }
        public decimal V { get; set; }
        public decimal Cr { get; set; }
        public decimal Cu { get; set; }
        public decimal Sn { get; set; }
        public decimal Pb { get; set; }
        public decimal Mo { get; set; }
        public decimal Al { get; set; }
        public decimal Ni { get; set; }
        public decimal Co { get; set; }
        public decimal Nb { get; set; }
        public decimal W { get; set; }
        public decimal As { get; set; }
        public decimal Bi { get; set; }
        public decimal Ca { get; set; }
        public decimal Ce { get; set; }
        public decimal Sb { get; set; }
        public decimal B { get; set; }
        public decimal N { get; set; }
        public decimal Zn { get; set; }
        public decimal Fe { get; set; }
        public decimal FMg { get; set; }

        // Navigation property
        public virtual ICollection<PipeProductionReport> PipeProductionReports { get; set; }
    }
}
