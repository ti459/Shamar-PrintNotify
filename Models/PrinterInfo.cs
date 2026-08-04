namespace Shamar_PrintNotify
{
    public class PrinterInfo
    {
        public string Name { get; set; } = "";
        public string PortName { get; set; } = "";
        public string IPAddress { get; set; } = "";
        public bool IsOffline { get; set; }
        public bool IsTcpIp { get; set; }
    }
}