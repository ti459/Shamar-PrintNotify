using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Shamar_PrintNotify
{
    public class SnmpPrinterAlertTestService : IDisposable
    {
        private readonly string _ip;
        private readonly string _filePath;

        private const int SnmpPort = 161;
        private const string Community = "public";
        private const int Timeout = 5000;

        // Printer-MIB
        private const string PrinterMibRoot =
            "1.3.6.1.2.1.43";

        // Printer-MIB / prtAlertTable
        private const string PrinterAlertTable =
            "1.3.6.1.2.1.43.18";

        public bool IsRunning
        {
            get;
            private set;
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public SnmpPrinterAlertTestService(
            string ip,
            string filePath)
        {
            _ip = ip;
            _filePath = filePath;
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            try
            {
                CriarCabecalho();

                ExecutarConsulta();
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void CriarCabecalho()
        {
            string pasta =
                Path.GetDirectoryName(_filePath)
                ?? AppDomain.CurrentDomain.BaseDirectory;

            Directory.CreateDirectory(pasta);

            using StreamWriter writer =
                new StreamWriter(
                    _filePath,
                    false);

            writer.WriteLine(
                "PRINTNOTIFY - TESTE PRINTER-MIB");

            writer.WriteLine(
                "Arquivo criado em: " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm:ss"));

            writer.WriteLine();

            writer.WriteLine(
                "============================================================");

            writer.WriteLine(
                "INÍCIO DO TESTE PRINTER-MIB");

            writer.WriteLine(
                "Impressora: Kyocera ECOSYS P3145dn");

            writer.WriteLine(
                "IP: " +
                _ip);

            writer.WriteLine(
                "Printer-MIB: " +
                PrinterMibRoot);

            writer.WriteLine(
                "Tabela de alertas: " +
                PrinterAlertTable);

            writer.WriteLine(
                "============================================================");

            writer.WriteLine();
        }

        private void ExecutarConsulta()
        {
            try
            {
                DateTime agora =
                    DateTime.Now;

                using StreamWriter writer =
                    new StreamWriter(
                        _filePath,
                        true);

                writer.WriteLine(
                    "CONSULTA PRINTER-MIB - " +
                    agora.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff"));

                writer.WriteLine(
                    "IP: " +
                    _ip);

                writer.WriteLine();

                IPEndPoint endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(_ip),
                        SnmpPort);

                OctetString community =
                    new OctetString(
                        Community);

                // =====================================================
                // CONSULTA TODA A PRINTER-MIB
                // =====================================================

                List<Variable> resultados =
                    new List<Variable>();

                ObjectIdentifier root =
                    new ObjectIdentifier(
                        PrinterMibRoot);

                Messenger.Walk(
                    VersionCode.V1,
                    endpoint,
                    community,
                    root,
                    resultados,
                    Timeout,
                    WalkMode.WithinSubtree);

                writer.WriteLine(
                    "TOTAL DE OIDs ENCONTRADOS: " +
                    resultados.Count);

                writer.WriteLine();

                foreach (
                    Variable item
                    in resultados)
                {
                    EscreverVariavel(
                        writer,
                        item);
                }

                writer.WriteLine();

                writer.WriteLine(
                    "============================================================");

                writer.WriteLine(
                    "CONSULTA DA TABELA DE ALERTAS");

                writer.WriteLine(
                    "Raiz: " +
                    PrinterAlertTable);

                writer.WriteLine(
                    "============================================================");

                writer.WriteLine();

                // =====================================================
                // CONSULTA ESPECÍFICA DA TABELA DE ALERTAS
                // =====================================================

                List<Variable> alertas =
                    new List<Variable>();

                ObjectIdentifier alertRoot =
                    new ObjectIdentifier(
                        PrinterAlertTable);

                Messenger.Walk(
                    VersionCode.V1,
                    endpoint,
                    community,
                    alertRoot,
                    alertas,
                    Timeout,
                    WalkMode.WithinSubtree);

                writer.WriteLine(
                    "TOTAL DE OIDs DE ALERTA: " +
                    alertas.Count);

                writer.WriteLine();

                foreach (
                    Variable item
                    in alertas)
                {
                    EscreverVariavel(
                        writer,
                        item);
                }

                writer.WriteLine();

                writer.WriteLine(
                    "============================================================");

                writer.WriteLine(
                    "FIM DA CONSULTA");

                writer.WriteLine(
                    "Data/Hora: " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss"));

                writer.WriteLine(
                    "============================================================");
            }
            catch (Exception ex)
            {
                using StreamWriter writer =
                    new StreamWriter(
                        _filePath,
                        true);

                writer.WriteLine();

                writer.WriteLine(
                    "ERRO DURANTE A CONSULTA");

                writer.WriteLine(
                    ex.ToString());

                writer.WriteLine();
            }
        }

        private void EscreverVariavel(
            StreamWriter writer,
            Variable item)
        {
            try
            {
                writer.WriteLine(
                    "OID: " +
                    item.Id);

                writer.WriteLine(
                    "VALOR: " +
                    item.Data);

                writer.WriteLine(
                    "TIPO: " +
                    item.Data.TypeCode);

                writer.WriteLine(
                    "------------------------------------------------------------");
            }
            catch
            {
                writer.WriteLine(
                    "OID: " +
                    item.Id);

                writer.WriteLine(
                    "VALOR: [erro ao ler valor]");

                writer.WriteLine(
                    "------------------------------------------------------------");
            }
        }

        public void Dispose()
        {
            IsRunning = false;
        }
    }
}