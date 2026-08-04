using System;
using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;

namespace Shamar_PrintNotify
{
    public class PrinterService
    {
        public List<PrinterInfo> GetPrinters()
        {
            var printers = new List<PrinterInfo>();

            try
            {
                using var searcher =
                    new ManagementObjectSearcher(
                        "SELECT Name, DriverName, PortName, WorkOffline FROM Win32_Printer");

                using ManagementObjectCollection results =
                    searcher.Get();

                foreach (ManagementObject printer in results)
                {
                    string name = "";
                    string driverName = "";
                    string portName = "";
                    bool offline = false;

                    // =====================================================
                    // NOME
                    // =====================================================

                    try
                    {
                        name =
                            printer["Name"]?.ToString() ?? "";
                    }
                    catch
                    {
                        name = "";
                    }

                    // =====================================================
                    // DRIVER
                    // =====================================================

                    try
                    {
                        driverName =
                            printer["DriverName"]?.ToString() ?? "";
                    }
                    catch
                    {
                        driverName = "";
                    }

                    // =====================================================
                    // PORTA
                    // =====================================================

                    try
                    {
                        portName =
                            printer["PortName"]?.ToString() ?? "";
                    }
                    catch
                    {
                        portName = "";
                    }

                    // =====================================================
                    // OFFLINE
                    // =====================================================

                    try
                    {
                        if (printer["WorkOffline"] != null)
                        {
                            offline =
                                Convert.ToBoolean(
                                    printer["WorkOffline"]);
                        }
                    }
                    catch
                    {
                        offline = false;
                    }

                    // =====================================================
                    // IP
                    // =====================================================

                    string ipAddress =
                        ExtractIpAddress(
                            portName);

                    // =====================================================
                    // IMPRESSORA
                    // =====================================================

                    var printerInfo =
                        new PrinterInfo
                        {
                            Name = name,
                            PortName = portName,
                            IPAddress = ipAddress,
                            IsOffline = offline,
                            IsTcpIp =
                                !string.IsNullOrWhiteSpace(
                                    ipAddress)
                        };

                    printers.Add(
                        printerInfo);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Não foi possível consultar as impressoras instaladas no Windows.\r\n\r\n" +
                    ex.Message,
                    ex);
            }

            return printers;
        }

        // =============================================================
        // LOCALIZAR ENDEREÇO IP
        // =============================================================

        private string ExtractIpAddress(
            string portName)
        {
            if (string.IsNullOrWhiteSpace(
                portName))
            {
                return "";
            }

            // =========================================================
            // PORTA PADRÃO DO WINDOWS
            //
            // Exemplo:
            // IP_192.168.200.116
            // =========================================================

            var match =
                Regex.Match(
                    portName,
                    @"IP_(\d{1,3}(?:\.\d{1,3}){3})",
                    RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // =========================================================
            // PORTA COM IP DIRETO
            //
            // Exemplo:
            // 192.168.200.116
            // =========================================================

            match =
                Regex.Match(
                    portName,
                    @"\b(\d{1,3}(?:\.\d{1,3}){3})\b");

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }
    }
}