using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shamar_PrintNotify
{
    public class SnmpService
    {
        private const string Community = "public";
        private const int SnmpPort = 161;
        private const int Timeout = 3000;

        // =========================================================
        // CONSULTA BÁSICA
        // =========================================================

        public bool TestConnection(string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return false;
                }

                var endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(ipAddress),
                        SnmpPort);

                var variables =
                    new List<Variable>
                    {
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.1.1.0"))
                    };

                var result =
                    Messenger.Get(
                        VersionCode.V1,
                        endpoint,
                        new OctetString(Community),
                        variables,
                        Timeout);

                return result.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        // =========================================================
        // CONSULTAR IDENTIFICAÇÃO DA IMPRESSORA
        // =========================================================

        public string GetPrinterDescription(
            string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return "";
                }

                var endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(ipAddress),
                        SnmpPort);

                var variables =
                    new List<Variable>
                    {
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.1.1.0"))
                    };

                var result =
                    Messenger.Get(
                        VersionCode.V1,
                        endpoint,
                        new OctetString(Community),
                        variables,
                        Timeout);

                if (result.Count == 0)
                {
                    return "";
                }

                return result[0].Data.ToString();
            }
            catch
            {
                return "";
            }
        }

        // =========================================================
        // CONSULTAR STATUS DA IMPRESSORA
        // =========================================================

        public SnmpPrinterStatus GetStatus(
            string ipAddress)
        {
            var status =
                new SnmpPrinterStatus();

            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return status;
                }

                var endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(ipAddress),
                        SnmpPort);

                var variables =
                    new List<Variable>
                    {
                        // Descrição
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.1.1.0")),

                        // hrPrinterStatus
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.25.3.5.1.1.1")),

                        // hrDeviceStatus
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.25.3.2.1.5.1")),

                        // hrDeviceErrors
                        new Variable(
                            new ObjectIdentifier(
                                "1.3.6.1.2.1.25.3.2.1.6.1"))
                    };

                var result =
                    Messenger.Get(
                        VersionCode.V1,
                        endpoint,
                        new OctetString(Community),
                        variables,
                        Timeout);

                if (result.Count == 0)
                {
                    return status;
                }

                status.Respondeu = true;

                foreach (var item in result)
                {
                    string oid =
                        item.Id.ToString();

                    string valor =
                        item.Data.ToString();

                    // -------------------------------------------------
                    // DESCRIÇÃO
                    // -------------------------------------------------

                    if (oid ==
                        "1.3.6.1.2.1.1.1.0")
                    {
                        status.Description =
                            valor;
                    }

                    // -------------------------------------------------
                    // STATUS DA IMPRESSORA
                    // -------------------------------------------------

                    else if (oid ==
                        "1.3.6.1.2.1.25.3.5.1.1.1")
                    {
                        if (int.TryParse(
                            valor,
                            out int printerStatus))
                        {
                            status.PrinterStatus =
                                printerStatus;
                        }
                    }

                    // -------------------------------------------------
                    // STATUS DO DISPOSITIVO
                    // -------------------------------------------------

                    else if (oid ==
                        "1.3.6.1.2.1.25.3.2.1.5.1")
                    {
                        if (int.TryParse(
                            valor,
                            out int deviceStatus))
                        {
                            status.DeviceStatus =
                                deviceStatus;
                        }
                    }

                    // -------------------------------------------------
                    // ERROS DO DISPOSITIVO
                    // -------------------------------------------------

                    else if (oid ==
                        "1.3.6.1.2.1.25.3.2.1.6.1")
                    {
                        if (long.TryParse(
                            valor,
                            out long errors))
                        {
                            status.DeviceErrors =
                                errors;
                        }
                    }
                }

                return status;
            }
            catch
            {
                return status;
            }
        }
    }

    // =============================================================
    // RESULTADO DO SNMP
    // =============================================================

    public class SnmpPrinterStatus
    {
        public bool Respondeu { get; set; }

        public string Description { get; set; } = "";

        public int PrinterStatus { get; set; }

        public int DeviceStatus { get; set; }

        public long DeviceErrors { get; set; }
    }
}