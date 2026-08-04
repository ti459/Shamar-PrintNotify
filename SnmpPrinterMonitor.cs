using System;

namespace Shamar_PrintNotify
{
    public class SnmpPrinterMonitor : IDisposable
    {
        private readonly SnmpService _snmpService;
        private readonly string _ipAddress;

        private System.Threading.Timer? _timer;

        private int _ultimoPrinterStatus = -1;
        private int _ultimoDeviceStatus = -1;

        public bool IsRunning { get; private set; }

        public event Action<int>? PrinterStatusChanged;
        public event Action? PrinterStarted;
        public event Action? PrinterStopped;

        public SnmpPrinterMonitor(
            string ipAddress)
        {
            _ipAddress =
                ipAddress ?? string.Empty;

            _snmpService =
                new SnmpService();
        }

        // =============================================================
        // INICIAR MONITOR
        // =============================================================

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(
                _ipAddress))
            {
                return;
            }

            IsRunning = true;

            Console.WriteLine(
                "SNMP MONITOR: iniciado.");

            Console.WriteLine(
                $"SNMP MONITOR: IP = {_ipAddress}");

            _timer =
                new System.Threading.Timer(
                    Consultar,
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2));
        }

        // =============================================================
        // CONSULTAR SNMP
        // =============================================================

        private void Consultar(
            object? state)
        {
            try
            {
                if (!IsRunning)
                {
                    return;
                }

                SnmpPrinterStatus status =
                    _snmpService.GetStatus(
                        _ipAddress);

                // =====================================================
                // SNMP NÃO RESPONDEU
                // =====================================================

                if (!status.Respondeu)
                {
                    Console.WriteLine(
                        "SNMP MONITOR: SNMP não respondeu.");

                    // -1 = problema de comunicação
                    PrinterStatusChanged?.Invoke(-1);

                    return;
                }

                // =====================================================
                // SNMP RESPONDEU
                // =====================================================

                Console.WriteLine(
                    "SNMP MONITOR: consulta realizada.");

                Console.WriteLine(
                    $"SNMP MONITOR: PrinterStatus = {status.PrinterStatus}");

                Console.WriteLine(
                    $"SNMP MONITOR: DeviceStatus = {status.DeviceStatus}");

                Console.WriteLine(
                    $"SNMP MONITOR: DeviceErrors = {status.DeviceErrors}");

                Console.WriteLine(
                    $"SNMP MONITOR: Description = {status.Description}");

                int atualPrinterStatus =
                    status.PrinterStatus;

                int atualDeviceStatus =
                    status.DeviceStatus;

                // =====================================================
                // PRIMEIRA LEITURA
                // =====================================================

                if (_ultimoPrinterStatus == -1)
                {
                    _ultimoPrinterStatus =
                        atualPrinterStatus;

                    _ultimoDeviceStatus =
                        atualDeviceStatus;

                    int estadoInicial =
                        DeterminarStatus(
                            status);

                    Console.WriteLine(
                        $"SNMP MONITOR: estado inicial = {estadoInicial}");

                    PrinterStatusChanged?.Invoke(
                        estadoInicial);

                    return;
                }

                // =====================================================
                // DETECTAR MUDANÇA
                // =====================================================

                bool mudou =
                    atualPrinterStatus !=
                        _ultimoPrinterStatus ||

                    atualDeviceStatus !=
                        _ultimoDeviceStatus;

                if (mudou)
                {
                    int anteriorPrinterStatus =
                        _ultimoPrinterStatus;

                    int anteriorDeviceStatus =
                        _ultimoDeviceStatus;

                    Console.WriteLine(
                        "SNMP MONITOR: mudança de estado detectada.");

                    Console.WriteLine(
                        $"SNMP MONITOR: PrinterStatus anterior = {anteriorPrinterStatus}");

                    Console.WriteLine(
                        $"SNMP MONITOR: PrinterStatus atual = {atualPrinterStatus}");

                    Console.WriteLine(
                        $"SNMP MONITOR: DeviceStatus anterior = {anteriorDeviceStatus}");

                    Console.WriteLine(
                        $"SNMP MONITOR: DeviceStatus atual = {atualDeviceStatus}");

                    _ultimoPrinterStatus =
                        atualPrinterStatus;

                    _ultimoDeviceStatus =
                        atualDeviceStatus;

                    int novoEstado =
                        DeterminarStatus(
                            status);

                    PrinterStatusChanged?.Invoke(
                        novoEstado);

                    // =================================================
                    // IMPRESSORA ENTROU EM ATIVIDADE
                    // 3 -> 4
                    // =================================================

                    if (anteriorPrinterStatus == 3 &&
                        atualPrinterStatus == 4)
                    {
                        Console.WriteLine(
                            "SNMP MONITOR: impressora iniciou atividade.");

                        PrinterStarted?.Invoke();
                    }

                    // =================================================
                    // IMPRESSORA VOLTOU AO NORMAL
                    // 4 -> 3
                    // =================================================

                    if (anteriorPrinterStatus == 4 &&
                        atualPrinterStatus == 3)
                    {
                        Console.WriteLine(
                            "SNMP MONITOR: impressora voltou ao normal.");

                        PrinterStopped?.Invoke();
                    }

                    return;
                }

                // =====================================================
                // SEM MUDANÇA
                // =====================================================

                int estadoAtual =
                    DeterminarStatus(
                        status);

                PrinterStatusChanged?.Invoke(
                    estadoAtual);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SNMP MONITOR: erro na consulta: {ex.Message}");

                // Falha de comunicação.
                PrinterStatusChanged?.Invoke(-1);
            }
        }

        // =============================================================
        // DETERMINAR ESTADO
        // =============================================================

        private int DeterminarStatus(
            SnmpPrinterStatus status)
        {
            // =========================================================
            // SEM RESPOSTA
            // =========================================================

            if (!status.Respondeu)
            {
                return -1;
            }

            // =========================================================
            // IMPRESSORA TRABALHANDO
            //
            // Na Kyocera observada:
            // 4 = em atividade
            // =========================================================

            if (status.PrinterStatus == 4)
            {
                return 4;
            }

            // =========================================================
            // IMPRESSORA NORMAL
            //
            // Na Kyocera observada:
            // 3 = normal
            // =========================================================

            if (status.PrinterStatus == 3)
            {
                return 3;
            }

            // =========================================================
            // OUTROS ESTADOS DA IMPRESSORA
            // =========================================================

            return -2;
        }

        // =============================================================
        // PARAR
        // =============================================================

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;

            if (_timer != null)
            {
                _timer.Dispose();

                _timer = null;
            }

            Console.WriteLine(
                "SNMP MONITOR: parado.");
        }

        // =============================================================
        // DESCARTAR
        // =============================================================

        public void Dispose()
        {
            Stop();
        }
    }
}

