using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public class TrayIconService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly PrintMonitorService _printMonitor;

        private readonly Icon _greenIcon;
        private readonly Icon _yellowIcon;
        private readonly Icon _redIcon;

        private ContextMenuStrip? _menuUsuario;
        private ContextMenuStrip? _menuTecnico;

        private SnmpAutoTestService? _snmpAutoTest;
        private SnmpPrinterMonitor? _snmpPrinterMonitor;

        private const string KyoceraIp =
            "192.168.200.250";

        private bool _disposed;

        // Estado atual do SNMP
        private int _snmpStatus = 3;

        public TrayIconService()
        {
            _greenIcon =
                LoadIcon("green.ico");

            _yellowIcon =
                LoadIcon("yellow.ico");

            _redIcon =
                LoadIcon("red.ico");

            _notifyIcon =
                new NotifyIcon
                {
                    Icon = _greenIcon,
                    Text = "PrintNotify - Normal",
                    Visible = true
                };

            _notifyIcon.MouseUp +=
                NotifyIcon_MouseUp;

            _printMonitor =
                new PrintMonitorService();

            _printMonitor.PrintJobDetected +=
                PrintMonitor_PrintJobDetected;

            _printMonitor.PrintJobFinished +=
                PrintMonitor_PrintJobFinished;

            _printMonitor.PrintJobError +=
                PrintMonitor_PrintJobError;

            _printMonitor.PrintJobCancelled +=
                PrintMonitor_PrintJobCancelled;

            IniciarMonitorSnmp();

            CriarMenuUsuario();

            CriarMenuTecnico();
        }

        // =============================================================
        // MONITOR SNMP
        // =============================================================

        private void IniciarMonitorSnmp()
        {
            try
            {
                _snmpPrinterMonitor =
                    new SnmpPrinterMonitor(
                        KyoceraIp);

                _snmpPrinterMonitor.PrinterStarted +=
                    SnmpPrinterMonitor_PrinterStarted;

                _snmpPrinterMonitor.PrinterStopped +=
                    SnmpPrinterMonitor_PrinterStopped;

                _snmpPrinterMonitor.PrinterStatusChanged +=
                    SnmpPrinterMonitor_PrinterStatusChanged;

                _snmpPrinterMonitor.Start();
            }
            catch
            {
                _snmpPrinterMonitor =
                    null;

                _snmpStatus = -1;

                SetRedStatus();
            }
        }

        private void SnmpPrinterMonitor_PrinterStarted()
        {
            if (_disposed)
            {
                return;
            }

            _snmpStatus = 4;

            SetYellowStatus();
        }

        private void SnmpPrinterMonitor_PrinterStopped()
        {
            if (_disposed)
            {
                return;
            }

            _snmpStatus = 3;

            SetGreenStatus();
        }

        private void SnmpPrinterMonitor_PrinterStatusChanged(
            int status)
        {
            if (_disposed)
            {
                return;
            }

            _snmpStatus =
                status;

            // =========================================================
            // SNMP NÃO RESPONDEU
            // =========================================================

            if (status == -1)
            {
                SetRedStatus();

                return;
            }

            // =========================================================
            // ESTADO NORMAL
            // =========================================================

            if (status == 3)
            {
                SetGreenStatus();

                return;
            }

            // =========================================================
            // IMPRESSORA TRABALHANDO
            // =========================================================

            if (status == 4)
            {
                SetYellowStatus();

                return;
            }

            // =========================================================
            // ESTADO ANORMAL
            // =========================================================

            if (status == -2)
            {
                SetRedStatus();

                return;
            }

            SetRedStatus();
        }

        // =============================================================
        // CARREGAR ÍCONE
        // =============================================================

        private Icon LoadIcon(
            string fileName)
        {
            string path =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Icons",
                    fileName);

            if (File.Exists(path))
            {
                return new Icon(path);
            }

            return SystemIcons.Application;
        }

        // =============================================================
        // CLIQUE NO ÍCONE
        // =============================================================

        private void NotifyIcon_MouseUp(
            object? sender,
            MouseEventArgs e)
        {
            if (_disposed)
            {
                return;
            }

            if (e.Button ==
                MouseButtons.Left)
            {
                if (_menuUsuario != null &&
                    _menuUsuario.Visible)
                {
                    _menuUsuario.Close();

                    return;
                }

                if (_menuTecnico != null &&
                    _menuTecnico.Visible)
                {
                    _menuTecnico.Close();
                }

                MostrarMenuUsuario();

                return;
            }

            if (e.Button ==
                MouseButtons.Right)
            {
                if (_menuTecnico != null &&
                    _menuTecnico.Visible)
                {
                    _menuTecnico.Close();

                    return;
                }

                if (_menuUsuario != null &&
                    _menuUsuario.Visible)
                {
                    _menuUsuario.Close();
                }

                MostrarMenuTecnico();
            }
        }

        // =============================================================
        // MENU USUÁRIO
        // =============================================================

        private void CriarMenuUsuario()
        {
            _menuUsuario =
                CriarMenuBase();

            AdicionarItem(
                _menuUsuario,
                "Status",
                SystemIcons.Information,
                Status_Click);

            AdicionarItem(
                _menuUsuario,
                "Trabalhos de Impressão",
                SystemIcons.Application,
                TrabalhosImpressao_Click);

            AdicionarItem(
                _menuUsuario,
                "Histórico recente",
                SystemIcons.Application,
                Historico_Click);

            AdicionarItem(
                _menuUsuario,
                "Histórico completo",
                SystemIcons.Application,
                HistoricoCompleto_Click);

            AdicionarItem(
                _menuUsuario,
                "Configurações de Notificação",
                SystemIcons.Information,
                ConfiguracoesNotificacao_Click);

            _menuUsuario.Items.Add(
                new ToolStripSeparator());

            AdicionarItem(
                _menuUsuario,
                "Sair",
                SystemIcons.Error,
                Sair_Click);
        }

        // =============================================================
        // MENU TÉCNICO
        // =============================================================

        private void CriarMenuTecnico()
        {
            _menuTecnico =
                CriarMenuBase();

            var titulo =
                new ToolStripMenuItem(
                    "PRINTNOTIFY - ÁREA TÉCNICA");

            titulo.Enabled =
                false;

            titulo.Font =
                new Font(
                    titulo.Font,
                    FontStyle.Bold);

            _menuTecnico.Items.Add(
                titulo);

            _menuTecnico.Items.Add(
                new ToolStripSeparator());

            AdicionarItem(
                _menuTecnico,
                "TESTE SNMP TEMPORÁRIO",
                SystemIcons.Application,
                TesteSnmpTemporario_Click);

            AdicionarItem(
                _menuTecnico,
                "TESTE SNMP AUTOMÁTICO",
                SystemIcons.Information,
                TesteSnmpAutomatico_Click);

            AdicionarItem(
                _menuTecnico,
                "TESTE PRINTER-MIB / ALERTAS",
                SystemIcons.Warning,
                TestePrinterMib_Click);

            _menuTecnico.Items.Add(
                new ToolStripSeparator());

            AdicionarItem(
                _menuTecnico,
                "Sair",
                SystemIcons.Error,
                Sair_Click);
        }

        // =============================================================
        // MENU BASE
        // =============================================================

        private ContextMenuStrip CriarMenuBase()
        {
            var menu =
                new ContextMenuStrip();

            menu.AutoSize =
                true;

            menu.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular);

            menu.Renderer =
                new PrintNotifyMenuRenderer();

            return menu;
        }

        // =============================================================
        // ADICIONAR ITEM
        // =============================================================

        private void AdicionarItem(
            ContextMenuStrip menu,
            string texto,
            Icon icone,
            EventHandler evento)
        {
            var item =
                new ToolStripMenuItem(
                    texto);

            item.Image =
                IconToBitmap(
                    icone,
                    18,
                    18);

            item.ImageScaling =
                ToolStripItemImageScaling.SizeToFit;

            item.Click +=
                evento;

            menu.Items.Add(
                item);
        }

        // =============================================================
        // CONVERTER ÍCONE
        // =============================================================

        private Bitmap IconToBitmap(
            Icon icon,
            int width,
            int height)
        {
            using var bitmap =
                new Bitmap(
                    width,
                    height);

            using Graphics graphics =
                Graphics.FromImage(
                    bitmap);

            graphics.Clear(
                Color.Transparent);

            graphics.DrawIcon(
                icon,
                new Rectangle(
                    0,
                    0,
                    width,
                    height));

            return new Bitmap(
                bitmap);
        }

        // =============================================================
        // MOSTRAR MENU USUÁRIO
        // =============================================================

        private void MostrarMenuUsuario()
        {
            if (_disposed)
            {
                return;
            }

            if (_menuUsuario == null ||
                _menuUsuario.IsDisposed)
            {
                CriarMenuUsuario();
            }

            if (_menuUsuario == null)
            {
                return;
            }

            _menuUsuario.Show(
                Cursor.Position);
        }

        // =============================================================
        // MOSTRAR MENU TÉCNICO
        // =============================================================

        private void MostrarMenuTecnico()
        {
            if (_disposed)
            {
                return;
            }

            if (_menuTecnico == null ||
                _menuTecnico.IsDisposed)
            {
                CriarMenuTecnico();
            }

            if (_menuTecnico == null)
            {
                return;
            }

            _menuTecnico.Show(
                Cursor.Position);
        }

        // =============================================================
        // STATUS
        // =============================================================

        private void Status_Click(
            object? sender,
            EventArgs e)
        {
            string statusSnmp;

            if (_snmpStatus == 3)
            {
                statusSnmp =
                    "SNMP: Normal";
            }
            else if (_snmpStatus == 4)
            {
                statusSnmp =
                    "SNMP: Impressora em atividade";
            }
            else if (_snmpStatus == -1)
            {
                statusSnmp =
                    "SNMP: Sem resposta";
            }
            else
            {
                statusSnmp =
                    "SNMP: Estado anormal";
            }

            MessageBox.Show(
                "PrintNotify está operacional.\r\n\r\n" +
                "Monitoramento automático ativo.\r\n" +
                "Monitor SNMP da Kyocera ativo.\r\n\r\n" +
                statusSnmp,

                "PrintNotify - Status",

                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =============================================================
        // CONFIGURAÇÕES
        // =============================================================

        private void ConfiguracoesNotificacao_Click(
            object? sender,
            EventArgs e)
        {
            using var form =
                new ConfiguracoesNotificacaoForm();

            form.ShowDialog();
        }

        // =============================================================
        // TRABALHOS DE IMPRESSÃO
        // =============================================================

        private void TrabalhosImpressao_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                var service =
                    new PrintJobService();

                var jobs =
                    service.GetPrintJobs();

                if (jobs.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhum trabalho de impressão está atualmente no spooler.",

                        "PrintNotify - Trabalhos de Impressão",

                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                string mensagem =
                    string.Empty;

                foreach (var job in jobs)
                {
                    mensagem +=
                        "ID: " +
                        job.JobId +
                        "\r\n" +

                        "Documento: " +
                        job.Document +
                        "\r\n" +

                        "Impressora: " +
                        job.PrinterName +
                        "\r\n" +

                        "Usuário: " +
                        job.UserName +
                        "\r\n" +

                        "Status: " +
                        job.Status +
                        "\r\n\r\n";
                }

                MessageBox.Show(
                    mensagem,

                    "PrintNotify - Trabalhos de Impressão",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao consultar os trabalhos de impressão:\r\n\r\n" +
                    ex.Message,

                    "PrintNotify - Trabalhos",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // HISTÓRICO RECENTE
        // =============================================================

        private void Historico_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                var historyService =
                    new PrintHistoryService();

                var history =
                    historyService.GetAll();

                if (history.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressão registrada ainda.",

                        "PrintNotify - Histórico",

                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                var ultimos =
                    history
                        .OrderByDescending(
                            x => x.DateTime)
                        .Take(3)
                        .ToList();

                string mensagem =
                    string.Empty;

                foreach (var item in ultimos)
                {
                    mensagem +=
                        item.DateTime.ToString(
                            "dd/MM/yyyy HH:mm:ss") +
                        " - " +
                        item.Document +
                        " - " +
                        item.PrinterName +
                        " - " +
                        item.Status +
                        "\r\n";
                }

                MessageBox.Show(
                    mensagem,

                    "PrintNotify - Histórico recente",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar o histórico:\r\n\r\n" +
                    ex.Message,

                    "PrintNotify - Histórico",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // HISTÓRICO COMPLETO
        // =============================================================

        private void HistoricoCompleto_Click(
            object? sender,
            EventArgs e)
        {
            using var form =
                new HistoricoForm();

            form.ShowDialog();
        }

        // =============================================================
        // NOVO TRABALHO
        // =============================================================

        private void PrintMonitor_PrintJobDetected(
            PrintJobInfo job)
        {
            if (_disposed)
            {
                return;
            }

            SetYellowStatus();

            var settings =
                NotificationSettingsService.Load();

            if (settings.DisableWindowsNotifications)
            {
                return;
            }

            if (!settings.NotifyOnStarted)
            {
                return;
            }

            _notifyIcon.ShowBalloonTip(
                settings.NotificationDurationSeconds * 1000,

                "PrintNotify - Impressão iniciada",

                "Documento: " +
                job.Document +
                "\r\nImpressora: " +
                job.PrinterName,

                ToolTipIcon.Info);
        }

        // =============================================================
        // TRABALHO FINALIZADO
        // =============================================================

        private void PrintMonitor_PrintJobFinished(
            PrintJobInfo job)
        {
            if (_disposed)
            {
                return;
            }

            // Só volta para verde se o SNMP também
            // estiver indicando estado normal.
            if (_snmpStatus == 3)
            {
                SetGreenStatus();
            }
            else if (_snmpStatus == 4)
            {
                SetYellowStatus();
            }
            else if (_snmpStatus == -1 ||
                     _snmpStatus == -2)
            {
                SetRedStatus();
            }

            var settings =
                NotificationSettingsService.Load();

            if (settings.DisableWindowsNotifications)
            {
                return;
            }

            if (!settings.NotifyOnFinished)
            {
                return;
            }

            _notifyIcon.ShowBalloonTip(
                settings.NotificationDurationSeconds * 1000,

                "PrintNotify - Impressão concluída",

                "Documento: " +
                job.Document +
                "\r\nImpressora: " +
                job.PrinterName,

                ToolTipIcon.Info);
        }

        // =============================================================
        // ERRO
        // =============================================================

        private void PrintMonitor_PrintJobError(
            PrintJobInfo job)
        {
            if (_disposed)
            {
                return;
            }

            SetRedStatus();

            var settings =
                NotificationSettingsService.Load();

            if (settings.DisableWindowsNotifications)
            {
                return;
            }

            if (!settings.NotifyOnError)
            {
                return;
            }

            _notifyIcon.ShowBalloonTip(
                settings.NotificationDurationSeconds * 1000,

                "PrintNotify - Erro de impressão",

                "Documento: " +
                job.Document +
                "\r\nImpressora: " +
                job.PrinterName +
                "\r\nStatus: " +
                job.Status,

                ToolTipIcon.Error);
        }

        // =============================================================
        // TRABALHO CANCELADO
        // =============================================================

        private void PrintMonitor_PrintJobCancelled(
            PrintJobInfo job)
        {
            if (_disposed)
            {
                return;
            }

            if (_snmpStatus == 3)
            {
                SetGreenStatus();
            }
            else if (_snmpStatus == 4)
            {
                SetYellowStatus();
            }
            else
            {
                SetRedStatus();
            }
        }

        // =============================================================
        // ÍCONE VERDE
        // =============================================================

        private void SetGreenStatus()
        {
            if (_disposed)
            {
                return;
            }

            _notifyIcon.Icon =
                _greenIcon;

            _notifyIcon.Text =
                "PrintNotify - Normal";
        }

        // =============================================================
        // ÍCONE AMARELO
        // =============================================================

        private void SetYellowStatus()
        {
            if (_disposed)
            {
                return;
            }

            _notifyIcon.Icon =
                _yellowIcon;

            _notifyIcon.Text =
                "PrintNotify - Impressão em andamento";
        }

        // =============================================================
        // ÍCONE VERMELHO
        // =============================================================

        private void SetRedStatus()
        {
            if (_disposed)
            {
                return;
            }

            _notifyIcon.Icon =
                _redIcon;

            _notifyIcon.Text =
                "PrintNotify - Problema na impressão";
        }

        // =============================================================
        // TESTE SNMP TEMPORÁRIO
        // =============================================================

        private void TesteSnmpTemporario_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                string ip =
                    KyoceraIp;

                var snmp =
                    new SnmpService();

                bool respondeu =
                    snmp.TestConnection(ip);

                string resultado =
                    respondeu
                        ? "SNMP respondeu com sucesso."
                        : "SNMP não respondeu.";

                MessageBox.Show(
                    "TESTE SNMP\r\n\r\n" +
                    "Impressora:\r\n" +
                    "Kyocera ECOSYS P3145dn\r\n\r\n" +
                    "IP:\r\n" +
                    ip +
                    "\r\n\r\n" +
                    "Resultado:\r\n" +
                    resultado,

                    "PrintNotify - Teste SNMP",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERRO NO TESTE SNMP\r\n\r\n" +
                    ex.Message,

                    "PrintNotify - Teste SNMP",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // TESTE SNMP AUTOMÁTICO
        // =============================================================

        private void TesteSnmpAutomatico_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                if (_snmpAutoTest != null &&
                    _snmpAutoTest.IsRunning)
                {
                    DialogResult parar =
                        MessageBox.Show(
                            "O teste SNMP automático está em execução.\r\n\r\n" +
                            "Deseja parar o teste e salvar o relatório?",

                            "PrintNotify - Teste SNMP Automático",

                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                    if (parar !=
                        DialogResult.Yes)
                    {
                        return;
                    }

                    _snmpAutoTest.Stop();

                    string arquivo =
                        _snmpAutoTest.FilePath;

                    _snmpAutoTest.Dispose();

                    _snmpAutoTest =
                        null;

                    MessageBox.Show(
                        "Teste SNMP finalizado.\r\n\r\n" +
                        "Relatório salvo em:\r\n" +
                        arquivo,

                        "PrintNotify - Teste SNMP Automático",

                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                string pasta =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments),
                        "PrintNotify",
                        "SNMP");

                Directory.CreateDirectory(
                    pasta);

                string nomeArquivo =
                    "SNMP_Kyocera_" +
                    DateTime.Now.ToString(
                        "yyyy-MM-dd_HHmmss") +
                    ".txt";

                string caminho =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                _snmpAutoTest =
                    new SnmpAutoTestService(
                        KyoceraIp,
                        caminho);

                _snmpAutoTest.Start();

                MessageBox.Show(
                    "Teste SNMP automático iniciado.\r\n\r\n" +
                    "Impressora:\r\n" +
                    "Kyocera ECOSYS P3145dn\r\n\r\n" +
                    "IP:\r\n" +
                    KyoceraIp +
                    "\r\n\r\n" +
                    "A consulta será realizada a cada 2 segundos.\r\n\r\n" +
                    "Agora você pode realizar uma impressão normalmente.\r\n\r\n" +
                    "Para encerrar, abra novamente:\r\n" +
                    "Área Técnica → TESTE SNMP AUTOMÁTICO",

                    "PrintNotify - Teste SNMP Automático",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERRO AO INICIAR TESTE SNMP AUTOMÁTICO\r\n\r\n" +
                    ex.Message,

                    "PrintNotify - Teste SNMP Automático",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // TESTE PRINTER-MIB
        // =============================================================

        private void TestePrinterMib_Click(
            object? sender,
            EventArgs e)
        {
            try
            {
                string pasta =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments),
                        "PrintNotify",
                        "SNMP");

                Directory.CreateDirectory(
                    pasta);

                string nomeArquivo =
                    "PrinterMIB_Kyocera_" +
                    DateTime.Now.ToString(
                        "yyyy-MM-dd_HHmmss") +
                    ".txt";

                string caminho =
                    Path.Combine(
                        pasta,
                        nomeArquivo);

                var teste =
                    new SnmpPrinterAlertTestService(
                        KyoceraIp,
                        caminho);

                teste.Start();

                teste.Dispose();

                MessageBox.Show(
                    "TESTE PRINTER-MIB FINALIZADO.\r\n\r\n" +
                    "Arquivo salvo em:\r\n" +
                    caminho +
                    "\r\n\r\n" +
                    "Agora abra o arquivo e me envie o conteúdo.",

                    "PrintNotify - Printer-MIB",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERRO NO TESTE PRINTER-MIB\r\n\r\n" +
                    ex.Message,

                    "PrintNotify - Printer-MIB",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // SAIR
        // =============================================================

        private void Sair_Click(
            object? sender,
            EventArgs e)
        {
            Application.Exit();
        }

        // =============================================================
        // DESCARTAR
        // =============================================================

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_snmpPrinterMonitor != null)
            {
                _snmpPrinterMonitor.PrinterStarted -=
                    SnmpPrinterMonitor_PrinterStarted;

                _snmpPrinterMonitor.PrinterStopped -=
                    SnmpPrinterMonitor_PrinterStopped;

                _snmpPrinterMonitor.PrinterStatusChanged -=
                    SnmpPrinterMonitor_PrinterStatusChanged;

                _snmpPrinterMonitor.Stop();

                _snmpPrinterMonitor.Dispose();

                _snmpPrinterMonitor = null;
            }

            if (_snmpAutoTest != null)
            {
                if (_snmpAutoTest.IsRunning)
                {
                    _snmpAutoTest.Stop();
                }

                _snmpAutoTest.Dispose();

                _snmpAutoTest = null;
            }

            _printMonitor.PrintJobDetected -=
                PrintMonitor_PrintJobDetected;

            _printMonitor.PrintJobFinished -=
                PrintMonitor_PrintJobFinished;

            _printMonitor.PrintJobError -=
                PrintMonitor_PrintJobError;

            _printMonitor.PrintJobCancelled -=
                PrintMonitor_PrintJobCancelled;

            _printMonitor.Dispose();

            _menuUsuario?.Dispose();

            _menuUsuario = null;

            _menuTecnico?.Dispose();

            _menuTecnico = null;

            _notifyIcon.MouseUp -=
                NotifyIcon_MouseUp;

            _notifyIcon.Visible =
                false;

            _notifyIcon.Dispose();

            _greenIcon.Dispose();

            _yellowIcon.Dispose();

            _redIcon.Dispose();
        }
    }

    // =============================================================
    // RENDERIZADOR DO MENU
    // =============================================================

    public class PrintNotifyMenuRenderer :
        ToolStripProfessionalRenderer
    {
        public PrintNotifyMenuRenderer()
            : base(
                new PrintNotifyColorTable())
        {
        }
    }

    // =============================================================
    // CORES DO MENU
    // =============================================================

    public class PrintNotifyColorTable :
        ProfessionalColorTable
    {
        public override Color MenuBorder
        {
            get
            {
                return Color.FromArgb(
                    20,
                    45,
                    75);
            }
        }

        public override Color MenuItemSelected
        {
            get
            {
                return Color.FromArgb(
                    30,
                    75,
                    120);
            }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                return Color.FromArgb(
                    30,
                    75,
                    120);
            }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                return Color.FromArgb(
                    30,
                    75,
                    120);
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                return Color.FromArgb(
                    20,
                    55,
                    95);
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return Color.FromArgb(
                    20,
                    55,
                    95);
            }
        }

        public override Color ToolStripDropDownBackground
        {
            get
            {
                return Color.White;
            }
        }

        public override Color ImageMarginGradientBegin
        {
            get
            {
                return Color.FromArgb(
                    235,
                    240,
                    245);
            }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return Color.FromArgb(
                    235,
                    240,
                    245);
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                return Color.FromArgb(
                    235,
                    240,
                    245);
            }
        }
    }
}
