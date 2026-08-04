using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shamar_PrintNotify
{
    public class PrinterStatusService
    {
        private const string Community = "public";
        private const int SnmpPort = 161;
        private const int Timeout = 5000;

    private const string PrinterAlertRoot =
        "1.3.6.1.2.1.43.18.1.1";

        private const string PrinterDescriptionOid =
            "1.3.6.1.2.1.1.1.0";

        public PrinterStatusResult GetStatus(
            string ipAddress)
        {
            var result =
                new PrinterStatusResult();

            try
            {
                var endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(ipAddress),
                        SnmpPort);

                var variables =
                    new List<Variable>
                    {
                    new Variable(
                        new ObjectIdentifier(
                            PrinterDescriptionOid))
                    };

                var basicResult =
                    Messenger.Get(
                        VersionCode.V1,
                        endpoint,
                        new OctetString(
                            Community),
                        variables,
                        Timeout);

                if (basicResult.Count == 0)
                {
                    result.Status =
                        PrinterBasicStatus.Offline;

                    result.Message =
                        "A impressora não respondeu ao SNMP.";

                    return result;
                }

                result.Status =
                    PrinterBasicStatus.Normal;

                result.Message =
                    "Impressora respondendo normalmente ao SNMP.";

                ScanPrinterAlerts(
                    endpoint,
                    result);

                AnalyzeAlerts(result);

                return result;
            }
            catch (Exception ex)
            {
                result.Status =
                    PrinterBasicStatus.CommunicationError;

                result.Message =
                    "Erro de comunicação SNMP: " +
                    ex.Message;

                return result;
            }
        }

        private void ScanPrinterAlerts(
            IPEndPoint endpoint,
            PrinterStatusResult result)
        {
            try
            {
                var variables =
                    new List<Variable>();

                Messenger.Walk(
                    VersionCode.V1,
                    endpoint,
                    new OctetString(
                        Community),
                    new ObjectIdentifier(
                        PrinterAlertRoot),
                    variables,
                    Timeout,
                    WalkMode.WithinSubtree);

                foreach (var variable in variables)
                {
                    string oid =
                        variable.Id.ToString();

                    string value =
                        variable.Data.ToString();

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    result.Alerts.Add(
                        new PrinterAlert
                        {
                            Oid = oid,
                            Value = value
                        });
                }
            }
            catch
            {
                // A consulta dos alertas não deve
                // interromper o monitoramento.
            }
        }

        private void AnalyzeAlerts(
            PrinterStatusResult result)
        {
            foreach (var alert in result.Alerts)
            {
                string text =
                    alert.Value
                        .Trim()
                        .ToLowerInvariant();

                // =====================================================
                // CÓDIGO 543 - TONER SENSOR ERROR
                // =====================================================

                if (text.Contains("543") &&
                    text.Contains("toner"))
                {
                    result.Status =
                        PrinterBasicStatus.TonerError;

                    result.Message =
                        "Problema relacionado ao toner.";

                    continue;
                }

                // =====================================================
                // SEM TONER
                // =====================================================

                if (text.Contains("toner") &&
                    (
                        text.Contains("empty") ||
                        text.Contains("out") ||
                        text.Contains("low")
                    ))
                {
                    result.Status =
                        PrinterBasicStatus.TonerError;

                    result.Message =
                        "Toner ausente ou insuficiente.";

                    continue;
                }

                // =====================================================
                // SEM TINTA
                // =====================================================

                if (text.Contains("ink") &&
                    (
                        text.Contains("empty") ||
                        text.Contains("out") ||
                        text.Contains("low")
                    ))
                {
                    result.Status =
                        PrinterBasicStatus.InkError;

                    result.Message =
                        "Tinta ausente ou insuficiente.";

                    continue;
                }

                // =====================================================
                // SEM PAPEL
                // =====================================================

                if (
                    text.Contains("paper") &&
                    (
                        text.Contains("out") ||
                        text.Contains("empty") ||
                        text.Contains("no paper")
                    ))
                {
                    result.Status =
                        PrinterBasicStatus.NoPaper;

                    result.Message =
                        "Sem papel.";

                    continue;
                }

                // =====================================================
                // PAPEL PRESO
                // =====================================================

                if (
                    text.Contains("jam") ||
                    text.Contains("paper jam") ||
                    text.Contains("paper-jam") ||
                    text.Contains("atol")
                )
                {
                    result.Status =
                        PrinterBasicStatus.PaperJam;

                    result.Message =
                        "Papel preso na impressora.";

                    continue;
                }

                // =====================================================
                // TAMPA ABERTA
                // =====================================================

                if (
                    text.Contains("cover") &&
                    (
                        text.Contains("open") ||
                        text.Contains("opened")
                    ))
                {
                    result.Status =
                        PrinterBasicStatus.CoverOpen;

                    result.Message =
                        "Tampa da impressora aberta.";

                    continue;
                }

                // =====================================================
                // ERRO GENÉRICO
                // =====================================================

                if (
                    text.Contains("error") ||
                    text.Contains("failure") ||
                    text.Contains("failed") ||
                    text.Contains("erro") ||
                    text.Contains("falha")
                )
                {
                    if (result.Status ==
                        PrinterBasicStatus.Normal)
                    {
                        result.Status =
                            PrinterBasicStatus.GeneralError;

                        result.Message =
                            alert.Value;
                    }
                }
            }
        }
    }

    public enum PrinterBasicStatus
    {
        Normal,
        NoPaper,
        TonerError,
        InkError,
        CoverOpen,
        PaperJam,
        Offline,
        CommunicationError,
        GeneralError
    }

    public class PrinterStatusResult
    {
        public PrinterBasicStatus Status { get; set; }

        public string Message { get; set; } =
            string.Empty;

        public List<PrinterAlert> Alerts { get; } =
            new List<PrinterAlert>();
    }

}
