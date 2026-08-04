using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;
using System.Text;

namespace Shamar_PrintNotify
{
    public class SnmpAutoTestService : IDisposable
    {
        private readonly System.Threading.Timer _timer;
        private readonly object _lock = new object();

        private readonly string _ipAddress;
        private readonly string _filePath;

        private bool _running;
        private bool _disposed;

        private readonly string[] _oids =
        {
            "1.3.6.1.2.1.1.1.0",
            "1.3.6.1.2.1.25.3.5.1.1.1",
            "1.3.6.1.2.1.25.3.2.1.5.1",
            "1.3.6.1.2.1.25.3.2.1.6.1"
        };

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _running;
            }
        }

        public SnmpAutoTestService(
            string ipAddress,
            string filePath)
        {
            _ipAddress = ipAddress;
            _filePath = filePath;

            CriarArquivo();

            _timer = new System.Threading.Timer(
                Consultar,
                null,
                Timeout.Infinite,
                Timeout.Infinite);
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                if (_running)
                {
                    return;
                }

                _running = true;

                Registrar(
                    "============================================================");

                Registrar(
                    "INÍCIO DO TESTE SNMP AUTOMÁTICO");

                Registrar(
                    "Impressora: Kyocera ECOSYS P3145dn");

                Registrar(
                    "IP: " + _ipAddress);

                Registrar(
                    "Intervalo: 2 segundos");

                Registrar(
                    "============================================================");

                _timer.Change(
                    0,
                    2000);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                if (!_running)
                {
                    return;
                }

                _running = false;

                _timer.Change(
                    Timeout.Infinite,
                    Timeout.Infinite);

                Registrar(
                    "============================================================");

                Registrar(
                    "FIM DO TESTE SNMP AUTOMÁTICO");

                Registrar(
                    "Data/Hora: " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss"));

                Registrar(
                    "============================================================");
            }
        }

        private void Consultar(object? state)
        {
            if (!_running)
            {
                return;
            }

            try
            {
                var endpoint =
                    new IPEndPoint(
                        IPAddress.Parse(_ipAddress),
                        161);

                var variables =
                    new List<Variable>();

                foreach (string oid in _oids)
                {
                    variables.Add(
                        new Variable(
                            new ObjectIdentifier(
                                oid)));
                }

                var resultado =
                    Messenger.Get(
                        VersionCode.V1,
                        endpoint,
                        new OctetString("public"),
                        variables,
                        5000);

                Registrar(
                    "");

                Registrar(
                    "CONSULTA SNMP - " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff"));

                Registrar(
                    "IP: " +
                    _ipAddress);

                if (resultado.Count == 0)
                {
                    Registrar(
                        "A impressora não retornou dados.");

                    return;
                }

                foreach (var item in resultado)
                {
                    Registrar(
                        "OID: " +
                        item.Id);

                    Registrar(
                        "VALOR: " +
                        item.Data);

                    Registrar(
                        "TIPO: " +
                        item.Data.TypeCode);

                    Registrar(
                        "");
                }
            }
            catch (Exception ex)
            {
                Registrar(
                    "");

                Registrar(
                    "ERRO SNMP - " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm:ss.fff"));

                Registrar(
                    ex.Message);

                Registrar(
                    "");
            }
        }

        private void CriarArquivo()
        {
            string? diretorio =
                Path.GetDirectoryName(
                    _filePath);

            if (!string.IsNullOrWhiteSpace(
                diretorio))
            {
                Directory.CreateDirectory(
                    diretorio);
            }

            File.WriteAllText(
                _filePath,
                "PRINTNOTIFY - TESTE SNMP AUTOMÁTICO\r\n" +
                "Arquivo criado em: " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm:ss") +
                "\r\n\r\n",
                Encoding.UTF8);
        }

        private void Registrar(
            string texto)
        {
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(
                        _filePath,
                        texto +
                        Environment.NewLine,
                        Encoding.UTF8);
                }
            }
            catch
            {
                // Não interromper o teste por erro de gravação.
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                _running = false;

                _timer.Change(
                    Timeout.Infinite,
                    Timeout.Infinite);

                _timer.Dispose();
            }
        }
    }
}