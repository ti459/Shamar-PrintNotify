using System.Management;

namespace Shamar_PrintNotify
{
    public class PrintJobInfo
    {
        public uint JobId { get; set; }
        public string Document { get; set; } = "";
        public string PrinterName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Status { get; set; } = "";
        public uint TotalPages { get; set; }
        public uint PagesPrinted { get; set; }
    }

    public class PrintJobService
    {
        public List<PrintJobInfo> GetPrintJobs()
        {
            var jobs = new List<PrintJobInfo>();

            using var searcher = new ManagementObjectSearcher(
                "SELECT JobId, Document, Name, Owner, Status, TotalPages, " +
                "PagesPrinted FROM Win32_PrintJob");

            foreach (ManagementObject job in searcher.Get())
            {
                jobs.Add(new PrintJobInfo
                {
                    JobId = Convert.ToUInt32(job["JobId"] ?? 0),
                    Document = job["Document"]?.ToString() ?? "",
                    PrinterName = job["Name"]?.ToString() ?? "",
                    UserName = job["Owner"]?.ToString() ?? "",
                    Status = job["Status"]?.ToString() ?? "",
                    TotalPages = Convert.ToUInt32(job["TotalPages"] ?? 0),
                    PagesPrinted = Convert.ToUInt32(job["PagesPrinted"] ?? 0)
                });
            }

            return jobs;
        }
    }
}