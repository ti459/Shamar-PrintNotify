using System;

namespace Shamar_PrintNotify
{
    public class PrinterAlert
    {
        public string Oid { get; set; } = "";

    public string Description { get; set; } = "";

        public string Value { get; set; } = "";

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsError { get; set; }

        public bool IsWarning { get; set; }

        public string Message { get; set; } = "";
    }

}
