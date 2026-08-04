using System.Drawing;
using System.Windows.Forms;

namespace Shamar_PrintNotify
{
    public partial class HistoricoForm : Form
    {
        private DataGridView _grid = null!;
        private Button _btnFechar = null!;

        public HistoricoForm()
        {
            InitializeComponent();

            Text = "PrintNotify - Histórico de Impressões";
            StartPosition = FormStartPosition.CenterScreen;

            Width = 950;
            Height = 550;

            MinimumSize = new Size(800, 450);

            CriarInterface();

            Load += HistoricoForm_Load;
        }

        private void CriarInterface()
        {
            // =====================================================
            // TÍTULO
            // =====================================================

            Label lblTitulo = new Label
            {
                Text = "Histórico de Impressões",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Bold),
                Padding = new Padding(10, 10, 0, 0)
            };

            Controls.Add(lblTitulo);

            // =====================================================
            // PAINEL DO RODAPÉ
            // =====================================================

            Panel painelRodape = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            _btnFechar = new Button
            {
                Text = "Fechar",
                Width = 100,
                Height = 30
            };

            _btnFechar.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            _btnFechar.Location =
                new Point(
                    820,
                    10);

            _btnFechar.Click += BtnFechar_Click;

            painelRodape.Controls.Add(_btnFechar);

            Controls.Add(painelRodape);

            // =====================================================
            // GRID
            // =====================================================

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.FixedSingle
            };

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Data/Hora",
                    Name = "DataHora",
                    Width = 145
                });

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Documento",
                    Name = "Documento",
                    Width = 220
                });

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Impressora",
                    Name = "Impressora",
                    Width = 180
                });

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Usuário",
                    Name = "Usuario",
                    Width = 130
                });

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Status",
                    Name = "Status",
                    Width = 100
                });

            _grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = "Job ID",
                    Name = "JobId",
                    Width = 70
                });

            Controls.Add(_grid);
        }

        private void HistoricoForm_Load(
            object? sender,
            EventArgs e)
        {
            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            try
            {
                var historyService =
                    new PrintHistoryService();

                var history =
                    historyService.GetAll();

                _grid.Rows.Clear();

                var registros =
                    history
                        .OrderByDescending(
                            x => x.DateTime)
                        .ToList();

                foreach (var item in registros)
                {
                    int rowIndex =
                        _grid.Rows.Add();

                    DataGridViewRow row =
                        _grid.Rows[rowIndex];

                    row.Cells["DataHora"].Value =
                        item.DateTime.ToString(
                            "dd/MM/yyyy HH:mm:ss");

                    row.Cells["Documento"].Value =
                        item.Document;

                    row.Cells["Impressora"].Value =
                        item.PrinterName;

                    row.Cells["Usuario"].Value =
                        item.UserName;

                    row.Cells["Status"].Value =
                        item.Status;

                    row.Cells["JobId"].Value =
                        item.JobId;

                    if (item.Status.Equals(
                        "Erro",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.Font =
                            new Font(
                                _grid.Font,
                                FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar o histórico:\n\n" +
                    ex.Message,
                    "PrintNotify",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnFechar_Click(
            object? sender,
            EventArgs e)
        {
            Close();
        }
    }
}