using System;
using System.Drawing;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public partial class ConfiguracoesNotificacaoForm : Form
    {
        private CheckBox _chkErro = null!;
        private CheckBox _chkConcluida = null!;
        private CheckBox _chkAndamento = null!;
        private CheckBox _chkDesativar = null!;

        private ComboBox _cmbTempo = null!;

        private Button _btnSalvar = null!;
        private Button _btnCancelar = null!;

        public ConfiguracoesNotificacaoForm()
        {
            InitializeComponent();

            ConfigurarFormulario();
            CriarInterface();

            CarregarConfiguracoes();
        }

        // =========================================================
        // CONFIGURAÇÃO DA JANELA
        // =========================================================

        private void ConfigurarFormulario()
        {
            Text =
                "PrintNotify - Configurações de Notificação";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width =
                620;

            Height =
                520;

            MinimumSize =
                new Size(620, 520);

            MaximizeBox =
                false;

            MinimizeBox =
                false;

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            BackColor =
                Color.White;
        }

        // =========================================================
        // INTERFACE
        // =========================================================

        private void CriarInterface()
        {
            var lblTitulo =
                new Label
                {
                    Text =
                        "🔔  Configurações de Notificação",

                    Dock =
                        DockStyle.Top,

                    Height =
                        65,

                    Font =
                        new Font(
                            "Segoe UI",
                            17,
                            FontStyle.Bold),

                    Padding =
                        new Padding(
                            25,
                            15,
                            0,
                            0)
                };

            Controls.Add(
                lblTitulo);

            var lblSubtitulo =
                new Label
                {
                    Text =
                        "Escolha quando o PrintNotify deve avisar você.",

                    Dock =
                        DockStyle.Top,

                    Height =
                        40,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f),

                    ForeColor =
                        Color.DimGray,

                    Padding =
                        new Padding(
                            28,
                            0,
                            0,
                            0)
                };

            Controls.Add(
                lblSubtitulo);

            // =====================================================
            // EVENTOS
            // =====================================================

            var grupoEventos =
                new GroupBox
                {
                    Text =
                        "  Eventos de impressão  ",

                    Left =
                        25,

                    Top =
                        120,

                    Width =
                        550,

                    Height =
                        160,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold)
                };

            Controls.Add(
                grupoEventos);

            _chkErro =
                new CheckBox
                {
                    Text =
                        "🔴  Avisar quando ocorrer um erro",

                    Left =
                        20,

                    Top =
                        30,

                    Width =
                        480,

                    Height =
                        30,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            grupoEventos.Controls.Add(
                _chkErro);

            _chkConcluida =
                new CheckBox
                {
                    Text =
                        "🟢  Avisar quando a impressão for concluída",

                    Left =
                        20,

                    Top =
                        65,

                    Width =
                        480,

                    Height =
                        30,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            grupoEventos.Controls.Add(
                _chkConcluida);

            _chkAndamento =
                new CheckBox
                {
                    Text =
                        "🟡  Avisar quando uma impressão começar",

                    Left =
                        20,

                    Top =
                        100,

                    Width =
                        480,

                    Height =
                        30,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            grupoEventos.Controls.Add(
                _chkAndamento);

            // =====================================================
            // EXIBIÇÃO
            // =====================================================

            var grupoNotificacao =
                new GroupBox
                {
                    Text =
                        "  Exibição  ",

                    Left =
                        25,

                    Top =
                        295,

                    Width =
                        550,

                    Height =
                        105,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold)
                };

            Controls.Add(
                grupoNotificacao);

            _chkDesativar =
                new CheckBox
                {
                    Text =
                        "🔕  Não mostrar notificações do Windows",

                    Left =
                        20,

                    Top =
                        28,

                    Width =
                        480,

                    Height =
                        30,

                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            _chkDesativar.CheckedChanged +=
                ChkDesativar_CheckedChanged;

            grupoNotificacao.Controls.Add(
                _chkDesativar);

            var lblTempo =
                new Label
                {
                    Text =
                        "Tempo da notificação:",

                    Left =
                        20,

                    Top =
                        62,

                    Width =
                        150,

                    Height =
                        25,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f)
                };

            grupoNotificacao.Controls.Add(
                lblTempo);

            _cmbTempo =
                new ComboBox
                {
                    Left =
                        175,

                    Top =
                        58,

                    Width =
                        100,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f)
                };

            _cmbTempo.Items.Add(
                "3 segundos");

            _cmbTempo.Items.Add(
                "5 segundos");

            _cmbTempo.Items.Add(
                "7 segundos");

            _cmbTempo.Items.Add(
                "10 segundos");

            grupoNotificacao.Controls.Add(
                _cmbTempo);

            // =====================================================
            // BOTÕES
            // =====================================================

            _btnSalvar =
                new Button
                {
                    Text =
                        "💾  Salvar",

                    Width =
                        120,

                    Height =
                        38,

                    Left =
                        300,

                    Top =
                        420,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f,
                            FontStyle.Bold)
                };

            _btnSalvar.Click +=
                BtnSalvar_Click;

            Controls.Add(
                _btnSalvar);

            _btnCancelar =
                new Button
                {
                    Text =
                        "✖  Cancelar",

                    Width =
                        120,

                    Height =
                        38,

                    Left =
                        435,

                    Top =
                        420,

                    Font =
                        new Font(
                            "Segoe UI",
                            9.5f)
                };

            _btnCancelar.Click +=
                (sender, e) =>
                {
                    Close();
                };

            Controls.Add(
                _btnCancelar);
        }

        // =========================================================
        // CARREGAR
        // =========================================================

        private void CarregarConfiguracoes()
        {
            var settings =
                NotificationSettingsService.Load();

            _chkErro.Checked =
                settings.NotifyOnError;

            _chkConcluida.Checked =
                settings.NotifyOnFinished;

            _chkAndamento.Checked =
                settings.NotifyOnStarted;

            _chkDesativar.Checked =
                settings.DisableWindowsNotifications;

            switch (
                settings.NotificationDurationSeconds)
            {
                case 3:
                    _cmbTempo.SelectedIndex = 0;
                    break;

                case 5:
                    _cmbTempo.SelectedIndex = 1;
                    break;

                case 10:
                    _cmbTempo.SelectedIndex = 3;
                    break;

                default:
                    _cmbTempo.SelectedIndex = 2;
                    break;
            }

            AtualizarEstadoControles();
        }

        // =========================================================
        // DESATIVAR NOTIFICAÇÕES
        // =========================================================

        private void ChkDesativar_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            AtualizarEstadoControles();
        }

        private void AtualizarEstadoControles()
        {
            bool ativo =
                !_chkDesativar.Checked;

            _chkErro.Enabled =
                ativo;

            _chkConcluida.Enabled =
                ativo;

            _chkAndamento.Enabled =
                ativo;

            _cmbTempo.Enabled =
                ativo;
        }

        // =========================================================
        // SALVAR
        // =========================================================

        private void BtnSalvar_Click(
            object? sender,
            EventArgs e)
        {
            int segundos = 7;

            if (_cmbTempo.SelectedIndex == 0)
                segundos = 3;

            if (_cmbTempo.SelectedIndex == 1)
                segundos = 5;

            if (_cmbTempo.SelectedIndex == 3)
                segundos = 10;

            var settings =
                new NotificationSettings
                {
                    NotifyOnError =
                        _chkErro.Checked,

                    NotifyOnFinished =
                        _chkConcluida.Checked,

                    NotifyOnStarted =
                        _chkAndamento.Checked,

                    DisableWindowsNotifications =
                        _chkDesativar.Checked,

                    NotificationDurationSeconds =
                        segundos
                };

            NotificationSettingsService.Save(
                settings);

            MessageBox.Show(
                "Configurações de notificação salvas.",
                "PrintNotify",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Close();
        }
    }
}