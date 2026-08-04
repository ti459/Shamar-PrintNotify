using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public partial class ImpressorasForm : Form
    {
        private ListView _lista = null!;
        private Button _btnDetalhes = null!;
        private Button _btnAtualizar = null!;
        private Button _btnFechar = null!;

        private List<PrinterInfo> _impressoras = new();

        // =========================================================
        // CORES DO PRINTNOTIFY
        // =========================================================

        private readonly Color AzulEscuro =
            Color.FromArgb(20, 47, 78);

        private readonly Color AzulPrincipal =
            Color.FromArgb(31, 78, 121);

        private readonly Color AzulClaro =
            Color.FromArgb(235, 242, 249);

        private readonly Color Fundo =
            Color.FromArgb(245, 247, 250);

        private readonly Color Texto =
            Color.FromArgb(35, 42, 50);

        // =========================================================
        // CONSTRUTOR
        // =========================================================

        public ImpressorasForm()
        {
            InitializeComponent();

            ConfigurarFormulario();
            CriarInterface();

            Load += ImpressorasForm_Load;
        }

        // =========================================================
        // CONFIGURAÇÃO DO FORMULÁRIO
        // =========================================================

        private void ConfigurarFormulario()
        {
            Text = "PrintNotify - Impressoras";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width = 1050;
            Height = 650;

            MinimumSize =
                new Size(900, 580);

            BackColor = Fundo;

            Font =
                new Font(
                    "Segoe UI",
                    9.5f);

            DoubleBuffered = true;
        }

        // =========================================================
        // CRIAÇÃO DA INTERFACE
        // =========================================================

        private void CriarInterface()
        {
            // =====================================================
            // CABEÇALHO
            // =====================================================

            var painelCabecalho =
                new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 105,
                    BackColor = AzulEscuro
                };

            Controls.Add(painelCabecalho);

            var lblIcone =
                new Label
                {
                    Text = "🖨",
                    Left = 25,
                    Top = 20,
                    Width = 55,
                    Height = 55,
                    Font =
                        new Font(
                            "Segoe UI Emoji",
                            28),
                    ForeColor = Color.White,
                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            painelCabecalho.Controls.Add(
                lblIcone);

            var lblTitulo =
                new Label
                {
                    Text = "Impressoras",
                    Left = 90,
                    Top = 18,
                    Width = 600,
                    Height = 35,
                    Font =
                        new Font(
                            "Segoe UI",
                            19,
                            FontStyle.Bold),
                    ForeColor = Color.White
                };

            painelCabecalho.Controls.Add(
                lblTitulo);

            var lblSubtitulo =
                new Label
                {
                    Text =
                        "Impressoras instaladas e disponíveis neste computador",
                    Left = 92,
                    Top = 57,
                    Width = 750,
                    Height = 25,
                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f),
                    ForeColor =
                        Color.FromArgb(
                            215,
                            225,
                            235)
                };

            painelCabecalho.Controls.Add(
                lblSubtitulo);

            // =====================================================
            // ÁREA PRINCIPAL
            // =====================================================

            var painelPrincipal =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding =
                        new Padding(
                            25,
                            20,
                            25,
                            10),
                    BackColor = Fundo
                };

            Controls.Add(painelPrincipal);

            var lblInformacao =
                new Label
                {
                    Text = "Impressoras encontradas",
                    Dock = DockStyle.Top,
                    Height = 35,
                    Font =
                        new Font(
                            "Segoe UI",
                            11,
                            FontStyle.Bold),
                    ForeColor = Texto
                };

            painelPrincipal.Controls.Add(
                lblInformacao);

            // =====================================================
            // LISTA
            // =====================================================

            _lista =
                new ListView
                {
                    Dock = DockStyle.Fill,

                    View = View.Details,

                    FullRowSelect = true,

                    GridLines = true,

                    MultiSelect = false,

                    HideSelection = false,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    BackColor = Color.White,

                    ForeColor = Texto,

                    Font =
                        new Font(
                            "Segoe UI",
                            10f),

                    UseCompatibleStateImageBehavior =
                        false
                };

            _lista.Columns.Add(
                "STATUS",
                120);

            _lista.Columns.Add(
                "IMPRESSORA",
                300);

            _lista.Columns.Add(
                "ENDEREÇO IP",
                180);

            _lista.Columns.Add(
                "PORTA",
                300);

            _lista.SelectedIndexChanged +=
                Lista_SelectedIndexChanged;

            _lista.DoubleClick +=
                Lista_DoubleClick;

            painelPrincipal.Controls.Add(
                _lista);

            // =====================================================
            // PAINEL DOS BOTÕES
            // =====================================================

            var painelBotoes =
                new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 75,
                    BackColor = Fundo
                };

            Controls.Add(
                painelBotoes);

            // =====================================================
            // BOTÃO DETALHES
            // =====================================================

            _btnDetalhes =
                CriarBotao(
                    "🔎  Detalhes",
                    125);

            _btnDetalhes.Left = 25;
            _btnDetalhes.Top = 17;
            _btnDetalhes.Enabled = false;

            _btnDetalhes.Click +=
                BtnDetalhes_Click;

            painelBotoes.Controls.Add(
                _btnDetalhes);

            // =====================================================
            // BOTÃO ATUALIZAR
            // =====================================================

            _btnAtualizar =
                CriarBotao(
                    "🔄  Atualizar",
                    125);

            _btnAtualizar.Left = 165;
            _btnAtualizar.Top = 17;

            _btnAtualizar.Click +=
                BtnAtualizar_Click;

            painelBotoes.Controls.Add(
                _btnAtualizar);

            // =====================================================
            // BOTÃO FECHAR
            // =====================================================

            _btnFechar =
                CriarBotao(
                    "✖  Fechar",
                    125);

            _btnFechar.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            _btnFechar.Top = 17;

            _btnFechar.Click +=
                BtnFechar_Click;

            painelBotoes.Resize +=
                PainelBotoes_Resize;

            painelBotoes.Controls.Add(
                _btnFechar);

            PosicionarBotaoFechar(
                painelBotoes);
        }

        // =========================================================
        // BOTÃO PADRÃO
        // =========================================================

        private Button CriarBotao(
            string texto,
            int largura)
        {
            var botao =
                new Button
                {
                    Text = texto,

                    Width = largura,

                    Height = 40,

                    FlatStyle =
                        FlatStyle.Flat,

                    BackColor =
                        Color.White,

                    ForeColor =
                        AzulPrincipal,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f,
                            FontStyle.Bold),

                    Cursor =
                        Cursors.Hand,

                    UseVisualStyleBackColor =
                        false
                };

            botao.FlatAppearance.BorderColor =
                Color.FromArgb(
                    205,
                    215,
                    225);

            botao.FlatAppearance.BorderSize =
                1;

            botao.MouseEnter +=
                (sender, e) =>
                {
                    botao.BackColor =
                        AzulClaro;
                };

            botao.MouseLeave +=
                (sender, e) =>
                {
                    botao.BackColor =
                        Color.White;
                };

            return botao;
        }

        // =========================================================
        // POSICIONAMENTO DO BOTÃO FECHAR
        // =========================================================

        private void PainelBotoes_Resize(
            object? sender,
            EventArgs e)
        {
            if (sender is Panel painel)
            {
                PosicionarBotaoFechar(
                    painel);
            }
        }

        private void PosicionarBotaoFechar(
            Panel painel)
        {
            if (_btnFechar == null)
            {
                return;
            }

            _btnFechar.Left =
                painel.ClientSize.Width -
                _btnFechar.Width -
                25;
        }

        // =========================================================
        // LOAD
        // =========================================================

        private void ImpressorasForm_Load(
            object? sender,
            EventArgs e)
        {
            CarregarImpressoras();
        }

        // =========================================================
        // CARREGAR IMPRESSORAS
        // =========================================================

        private void CarregarImpressoras()
        {
            try
            {
                _lista.BeginUpdate();

                _lista.Items.Clear();

                var printerService =
                    new PrinterService();

                _impressoras =
                    printerService.GetPrinters();

                foreach (PrinterInfo printer
                    in _impressoras)
                {
                    AdicionarImpressora(
                        printer);
                }

                AtualizarEstadoBotoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar as impressoras:\r\n\r\n" +
                    ex.Message,

                    "PrintNotify",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
            finally
            {
                _lista.EndUpdate();
            }
        }

        // =========================================================
        // ADICIONAR IMPRESSORA
        // =========================================================

        private void AdicionarImpressora(
            PrinterInfo printer)
        {
            string status;

            if (printer.IsOffline)
            {
                status = "🔴 Offline";
            }
            else
            {
                status = "🟢 Online";
            }

            var item =
                new ListViewItem(
                    status);

            item.SubItems.Add(
                string.IsNullOrWhiteSpace(
                    printer.Name)
                    ? "(Sem nome)"
                    : printer.Name);

            item.SubItems.Add(
                string.IsNullOrWhiteSpace(
                    printer.IPAddress)
                    ? "-"
                    : printer.IPAddress);

            item.SubItems.Add(
                string.IsNullOrWhiteSpace(
                    printer.PortName)
                    ? "-"
                    : printer.PortName);

            item.Tag =
                printer;

            _lista.Items.Add(
                item);
        }

        // =========================================================
        // SELEÇÃO
        // =========================================================

        private void Lista_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            AtualizarEstadoBotoes();
        }

        private void AtualizarEstadoBotoes()
        {
            if (_btnDetalhes == null ||
                _lista == null)
            {
                return;
            }

            _btnDetalhes.Enabled =
                _lista.SelectedItems.Count > 0;
        }

        // =========================================================
        // DUPLO CLIQUE
        // =========================================================

        private void Lista_DoubleClick(
            object? sender,
            EventArgs e)
        {
            AbrirDetalhes();
        }

        // =========================================================
        // DETALHES
        // =========================================================

        private void BtnDetalhes_Click(
            object? sender,
            EventArgs e)
        {
            AbrirDetalhes();
        }

        private void AbrirDetalhes()
        {
            if (_lista.SelectedItems.Count == 0)
            {
                return;
            }

            var item =
                _lista.SelectedItems[0];

            if (item.Tag is not PrinterInfo printer)
            {
                return;
            }

            using var detalhes =
                new ImpressoraDetalhesForm(
                    printer);

            detalhes.ShowDialog();

            CarregarImpressoras();
        }

        // =========================================================
        // ATUALIZAR
        // =========================================================

        private void BtnAtualizar_Click(
            object? sender,
            EventArgs e)
        {
            CarregarImpressoras();
        }

        // =========================================================
        // FECHAR
        // =========================================================

        private void BtnFechar_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}