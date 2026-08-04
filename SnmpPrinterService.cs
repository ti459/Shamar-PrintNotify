using System.Collections.Generic;

namespace Shamar_PrintNotify
{
    public class SnmpPrinterService
    {
        private readonly SnmpService _snmpService;

        public SnmpPrinterService()
        {
            _snmpService = new SnmpService();
        }

        // =========================================================
        // CONSULTAR UMA IMPRESSORA
        // =========================================================

        public SnmpPrinterStatus Consultar(
            PrinterInfo printer)
        {
            if (printer == null)
            {
                return new SnmpPrinterStatus();
            }

            if (string.IsNullOrWhiteSpace(
                printer.IPAddress))
            {
                return new SnmpPrinterStatus();
            }

            return _snmpService.GetStatus(
                printer.IPAddress);
        }

        // =========================================================
        // CONSULTAR TODAS AS IMPRESSORAS
        // =========================================================

        public Dictionary<string, SnmpPrinterStatus>
            ConsultarTodas(
                List<PrinterInfo> impressoras)
        {
            var resultados =
                new Dictionary<string, SnmpPrinterStatus>();

            if (impressoras == null)
            {
                return resultados;
            }

            foreach (var printer in impressoras)
            {
                if (printer == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(
                    printer.IPAddress))
                {
                    continue;
                }

                var resultado =
                    Consultar(printer);

                resultados[printer.Name] =
                    resultado;
            }

            return resultados;
        }
    }
}