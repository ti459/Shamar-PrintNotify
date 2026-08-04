namespace Shamar_PrintNotify
{
    public class PrintHistoryItem
    {
        public DateTime DateTime { get; set; }

        public uint JobId { get; set; }

        public string Document { get; set; } = "";

        public string PrinterName { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Status { get; set; } = "";
    }
}