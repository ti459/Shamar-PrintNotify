using System;
using System.Drawing;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public partial class ImpressoraDetalhesForm : Form
    {
        private readonly PrinterInfo _printer;

        private Label _lblConexao = null!;
        private Label _lblSnmp = null!;
        private Label _lblStatus = null!;

        public ImpressoraDetalhesForm(PrinterInfo printer)
        {
            InitializeComponent();

            _printer = printer;

            Text = "PrintNotify - Detalhes da Impressora";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width = 600;
            Height = 480;

            MinimumSize =
                new Size(600, 480);

            CriarInterface();

            Load +=
                ImpressoraDetalhesForm_Load;
        }

        // =============================================================
        // INTERFACE
        // =============================================================

        private void CriarInterface()
        {
            var lblTitulo = new Label
            {
                Text = "Detalhes da Impressora",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font(
                    "Segoe UI",
                    15,
                    FontStyle.Bold),
                Padding =
                    new Padding(20, 15, 0, 0)
            };

            Controls.Add(lblTitulo);

            var painel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding =
                    new Padding(25, 10, 25, 20)
            };

            Controls.Add(painel);

            // =========================================================
            // NOME
            // =========================================================

            CriarLabel(
                painel,
                "Nome:",
                25,
                20);

            CriarValor(
                painel,
                _printer.Name,
                170,
                20);

            // =========================================================
            // IP
            // =========================================================

            CriarLabel(
                painel,
                "Endereço IP:",
                25,
                60);

            CriarValor(
                painel,
                string.IsNullOrWhiteSpace(
                    _printer.IPAddress)
                    ? "Não informado"
                    : _printer.IPAddress,
                170,
                60);

            // =========================================================
            // PORTA
            // =========================================================

            CriarLabel(
                painel,
                "Porta:",
                25,
                100);

            CriarValor(
                painel,
                _printer.PortName,
                170,
                100);

            // =========================================================
            // TIPO
            // =========================================================

            CriarLabel(
                painel,
                "Tipo:",
                25,
                140);

            CriarValor(
                painel,
                _printer.IsTcpIp
                    ? "TCP/IP"
                    : "Local/Virtual",
                170,
                140);

            // =========================================================
            // CONEXÃO
            // =========================================================

            CriarLabel(
                painel,
                "Conexão:",
                25,
                185);

            _lblConexao =
                CriarStatusLabel(
                    painel,
                    170,
                    185);

            // =========================================================
            // SNMP
            // =========================================================

            CriarLabel(
                painel,
                "SNMP:",
                25,
                225);

            _lblSnmp =
                CriarStatusLabel(
                    painel,
                    170,
                    225);

            // =========================================================
            // STATUS
            // =========================================================

            CriarLabel(
                painel,
                "Status:",
                25,
                265);

            _lblStatus =
                CriarStatusLabel(
                    painel,
                    170,
                    265);

            // =========================================================
            // BOTÃO ATUALIZAR
            // =========================================================

            var btnAtualizar = new Button
            {
                Text = "Atualizar",
                Width = 110,
                Height = 35,
                Location =
                    new Point(320, 340),
                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Right
            };

            btnAtualizar.Click +=
                BtnAtualizar_Click;

            painel.Controls.Add(
                btnAtualizar);

            // =========================================================
            // BOTÃO FECHAR
            // =========================================================

            var btnFechar = new Button
            {
                Text = "Fechar",
                Width = 110,
                Height = 35,
                Location =
                    new Point(440, 340),
                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Right
            };

            btnFechar.Click +=
                (sender, e) => Close();

            painel.Controls.Add(
                btnFechar);
        }

        // =============================================================
        // LABEL DE STATUS
        // =============================================================

        private Label CriarStatusLabel(
            Panel painel,
            int x,
            int y)
        {
            var label = new Label
            {
                Text = "Verificando...",
                AutoSize = true,
                Location =
                    new Point(x, y),
                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
            };

            painel.Controls.Add(label);

            return label;
        }

        // =============================================================
        // LABEL
        // =============================================================

        private void CriarLabel(
            Panel painel,
            string texto,
            int x,
            int y)
        {
            var label = new Label
            {
                Text = texto,
                AutoSize = true,
                Location =
                    new Point(x, y),
                Font =
                    new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold)
            };

            painel.Controls.Add(label);
        }

        // =============================================================
        // VALOR
        // =============================================================

        private void CriarValor(
            Panel painel,
            string texto,
            int x,
            int y)
        {
            var label = new Label
            {
                Text = texto,
                AutoSize = true,
                Location =
                    new Point(x, y)
            };

            painel.Controls.Add(label);
        }

        // =============================================================
        // LOAD
        // =============================================================

        private void ImpressoraDetalhesForm_Load(
            object? sender,
            EventArgs e)
        {
            AtualizarInformacoes();
        }

        // =============================================================
        // ATUALIZAR
        // =============================================================

        private void BtnAtualizar_Click(
            object? sender,
            EventArgs e)
        {
            AtualizarInformacoes();
        }

        // =============================================================
        // CONSULTAR INFORMAÇÕES
        // =============================================================

        private void AtualizarInformacoes()
        {
            // =========================================================
            // CONEXÃO TCP/IP
            // =========================================================

            if (_printer.IsTcpIp &&
                !string.IsNullOrWhiteSpace(
                    _printer.IPAddress))
            {
                try
                {
                    using var ping =
                        new System.Net.NetworkInformation.Ping();

                    var resposta =
                        ping.Send(
                            _printer.IPAddress,
                            2000);

                    if (resposta.Status ==
                        System.Net.NetworkInformation.IPStatus.Success)
                    {
                        _lblConexao.Text =
                            "🟢 Online";

                        _lblConexao.ForeColor =
                            Color.Green;
                    }
                    else
                    {
                        _lblConexao.Text =
                            "🔴 Offline";

                        _lblConexao.ForeColor =
                            Color.Red;
                    }
                }
                catch
                {
                    _lblConexao.Text =
                        "🔴 Sem resposta";

                    _lblConexao.ForeColor =
                        Color.Red;
                }
            }
            else
            {
                _lblConexao.Text =
                    "Não aplicável";

                _lblConexao.ForeColor =
                    SystemColors.ControlText;
            }

            // =========================================================
            // SNMP
            // =========================================================

            if (_printer.IsTcpIp &&
                !string.IsNullOrWhiteSpace(
                    _printer.IPAddress))
            {
                try
                {
                    var snmp =
                        new SnmpService();

                    // TestConnection retorna BOOL.
                    // TRUE  = respondeu ao SNMP.
                    // FALSE = não respondeu.
                    bool snmpFuncionando =
                        snmp.TestConnection(
                            _printer.IPAddress);

                    if (snmpFuncionando)
                    {
                        _lblSnmp.Text =
                            "🟢 Ativo";

                        _lblSnmp.ForeColor =
                            Color.Green;

                        _lblStatus.Text =
                            "🟢 Normal";

                        _lblStatus.ForeColor =
                            Color.Green;
                    }
                    else
                    {
                        _lblSnmp.Text =
                            "🔴 Sem resposta";

                        _lblSnmp.ForeColor =
                            Color.Red;

                        _lblStatus.Text =
                            "🔴 Indisponível";

                        _lblStatus.ForeColor =
                            Color.Red;
                    }
                }
                catch
                {
                    _lblSnmp.Text =
                        "🔴 Erro";

                    _lblSnmp.ForeColor =
                        Color.Red;

                    _lblStatus.Text =
                        "🔴 Indisponível";

                    _lblStatus.ForeColor =
                        Color.Red;
                }
            }
            else
            {
                _lblSnmp.Text =
                    "Não aplicável";

                _lblSnmp.ForeColor =
                    SystemColors.ControlText;

                _lblStatus.Text =
                    "Não disponível";

                _lblStatus.ForeColor =
                    SystemColors.ControlText;
            }
        }
    }
}