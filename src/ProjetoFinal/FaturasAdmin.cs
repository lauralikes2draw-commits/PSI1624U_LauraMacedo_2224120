using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class FaturasAdmin : Form
    {
        private List<AdminFatura> faturas = new List<AdminFatura>();

        public FaturasAdmin()
        {
            InitializeComponent();
            Load += FaturasAdmin_Load;
        }

        private void FaturasAdmin_Load(object sender, EventArgs e)
        {
            AdminSharedUi.PrepararPagina(this, "faturas");
            InicializarFiltros();
            LigarEventos();
            CarregarPagina();
        }

        private void InicializarFiltros()
        {
            cboEstado.Items.Clear();
            for (int i = 1; i <= 12; i++)
                cboEstado.Items.Add(new DateTime(DateTime.Today.Year, i, 1).ToString("MMMM"));
            cboEstado.SelectedIndex = DateTime.Today.Month - 1;
            cboEstado.Width = Math.Max(cboEstado.Width, 128);

            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.AddRange(new object[] { "Todos", "Paga", "Pendente", "Não paga" });
            guna2ComboBox1.SelectedIndex = 0;

            guna2DateTimePicker1.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            guna2DateTimePicker2.Value = DateTime.Today;
            guna2TextBox1.PlaceholderText = "Pesquisar por cliente, nº da fatura ou serviço";
            label33.Text = "Faturamento mensal";
            label26.Text = "Método de pagamento";
            label26.AutoSize = false;
            label26.Location = new Point(60, 10);
            label26.Size = new Size(170, 18);
            label25.Text = "Cartão";
            label25.AutoSize = false; label25.Size = new Size(90, 19);
            label24.AutoSize = false; label24.Size = new Size(90, 19);
            label28.AutoSize = false; label28.Size = new Size(90, 19);
            label29.AutoSize = false; label29.Size = new Size(55, 19);
            label30.AutoSize = false; label30.Size = new Size(55, 19);
            label31.AutoSize = false; label31.Size = new Size(55, 19);
        }

        private void LigarEventos()
        {
            DataGridViewComboBoxColumn metodo = dgvClientes.Columns["metodoPagamento"] as DataGridViewComboBoxColumn;
            if (metodo != null)
            {
                metodo.Items.Clear();
                metodo.Items.AddRange(new object[] { "Cartão", "Cartao", "MBWay", "MB WAY", "Dinheiro", "Multibanco", "Transferência", "Transferencia", "App", "Importado", "" });
                metodo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
            guna2TextBox1.TextChanged -= Filtro_Changed; guna2TextBox1.TextChanged += Filtro_Changed;
            cboEstado.SelectedIndexChanged -= MesGrafico_Changed; cboEstado.SelectedIndexChanged += MesGrafico_Changed;
            guna2ComboBox1.SelectedIndexChanged -= Filtro_Changed; guna2ComboBox1.SelectedIndexChanged += Filtro_Changed;
            guna2DateTimePicker1.ValueChanged -= Filtro_Changed; guna2DateTimePicker1.ValueChanged += Filtro_Changed;
            guna2DateTimePicker2.ValueChanged -= Filtro_Changed; guna2DateTimePicker2.ValueChanged += Filtro_Changed;
            dgvClientes.CellContentClick -= dgvClientes_CellContentClick; dgvClientes.CellContentClick += dgvClientes_CellContentClick;
            dgvClientes.DataError -= DgvClientes_DataError; dgvClientes.DataError += DgvClientes_DataError;
            guna2Button8.Click -= Exportar_Click; guna2Button8.Click += Exportar_Click;
            guna2Button9.Text = "<"; guna2Button10.Text = ">";
            AdminSharedUi.AplicarPaginacaoSetas(this, "guna2Button9", "guna2Button10");
        }

        private void Filtro_Changed(object sender, EventArgs e) { CarregarTabela(); }
        private void MesGrafico_Changed(object sender, EventArgs e) { CarregarGraficos(); }

        private void CarregarPagina()
        {
            CarregarPaineis();
            CarregarGraficos();
            CarregarTabela();
        }

        private void CarregarPaineis()
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            label9.Text = AdminRepository.CountMarcacoes(inicioMes, DateTime.Today, "Pendente").ToString();
            label11.Text = AdminRepository.GetFaturas("", "Paga", inicioMes, DateTime.Today, 5000).Count.ToString();
            lblFaturacao.Text = AdminRepository.Money(AdminRepository.SumFaturas(inicioMes, DateTime.Today, "Todos"));
            label14.Text = "+" + AdminRepository.GetFaturas("", "Todos", inicioMes, DateTime.Today, 5000).Count + " este mês";

            List<AdminServico> topServ = AdminRepository.GetTopServicos(1);
            if (topServ.Count > 0) { label40.Text = topServ[0].Nome; label39.Text = AdminRepository.Money(topServ[0].TotalFaturado); }
            List<AdminCliente> clientes = AdminRepository.GetClientes("", "Ativo", 5000);
            AdminCliente melhor = null;
            foreach (AdminCliente c in clientes) if (melhor == null || c.TotalGasto > melhor.TotalGasto) melhor = c;
            if (melhor != null) { label44.Text = melhor.Nome; label43.Text = AdminRepository.Money(melhor.TotalGasto) + " este mês"; }
            label17.Text = AdminRepository.Money(AdminRepository.SumMarcacoes(DateTime.Today, DateTime.Today.AddDays(30), "Confirmado"));
            label21.Text = AdminRepository.Money(AdminRepository.SumFaturas(inicioMes, DateTime.Today, "Todos"));
            label20.Text = "+" + AdminRepository.Money(AdminRepository.SumFaturas(inicioMes, DateTime.Today, "Paga"));
        }

        private void CarregarGraficos()
        {
            ConfigurarChart(chart1, "Faturação mensal");
            int mes = cboEstado.SelectedIndex >= 0 ? cboEstado.SelectedIndex + 1 : DateTime.Today.Month;
            DateTime inicio = new DateTime(DateTime.Today.Year, mes, 1);
            DateTime fim = inicio.AddMonths(1).AddDays(-1);
            if (fim > DateTime.Today) fim = DateTime.Today;
            List<AdminFatura> fs = AdminRepository.GetFaturas("", "Todos", inicio, fim, 5000);
            Dictionary<int, decimal> dias = new Dictionary<int, decimal>();
            foreach (AdminFatura f in fs)
            {
                int dia = f.DataFatura.Day;
                if (!dias.ContainsKey(dia)) dias[dia] = 0;
                dias[dia] += f.Total;
            }
            for (int dia = 1; dia <= DateTime.DaysInMonth(inicio.Year, inicio.Month); dia++)
                chart1.Series[0].Points.AddXY(dia.ToString(), dias.ContainsKey(dia) ? (double)dias[dia] : 0d);

            ConfigurarChart(chart2, "Estados");
            chart2.Series[0].ChartType = SeriesChartType.Doughnut;
            chart2.Series[0].Points.AddXY("Paga", AdminRepository.GetFaturas("", "Paga", inicio, fim, 5000).Count);
            chart2.Series[0].Points.AddXY("Pendente", AdminRepository.GetFaturas("", "Pendente", inicio, fim, 5000).Count);
            chart2.Series[0].Points.AddXY("Não paga", AdminRepository.GetFaturas("", "Não paga", inicio, fim, 5000).Count);
        }

        private void ConfigurarChart(Chart chart, string nome)
        {
            chart.Series.Clear();
            Series s = new Series(nome);
            s.ChartType = SeriesChartType.SplineArea;
            s.Color = Color.FromArgb(140, 255, 79, 135);
            s.BorderColor = AdminSharedUi.Rosa;
            s.BorderWidth = 3;
            chart.Series.Add(s);
        }

        private void CarregarTabela()
        {
            string termo = guna2TextBox1.Text.Trim();
            string estado = guna2ComboBox1.SelectedItem == null ? "Todos" : guna2ComboBox1.SelectedItem.ToString();
            DateTime inicio = guna2DateTimePicker1.Value.Date;
            DateTime fim = guna2DateTimePicker2.Value.Date;
            faturas = AdminRepository.GetFaturas(termo, estado, inicio, fim, 1000);
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            if (dgvClientes.Columns.Contains("metodoPagamento"))
            {
                dgvClientes.Columns["metodoPagamento"].HeaderText = "Método";
                dgvClientes.Columns["metodoPagamento"].MinimumWidth = 92;
            }
            dgvClientes.Rows.Clear();
            foreach (AdminFatura f in faturas)
            {
                string metodo = string.IsNullOrWhiteSpace(f.MetodoPagamento) ? "Multibanco" : f.MetodoPagamento;
                if (metodo != "Cartão" && metodo != "MBWay" && metodo != "Dinheiro" && metodo != "Multibanco" && metodo != "Transferência" && metodo != "App") metodo = "Multibanco";
                int idx = dgvClientes.Rows.Add(f.NumeroFatura, f.Cliente, f.DataFatura.ToString("dd/MM/yyyy"), f.Servicos, AdminRepository.Money(f.Subtotal), AdminRepository.Money(f.Desconto), AdminRepository.Money(f.Total), metodo, f.Estado, "Ver", "Importar", "Imprimir");
                dgvClientes.Rows[idx].Tag = f.IdFatura;
            }
            AdminSharedUi.PintarEstado(dgvClientes, "estado");
            label51.Text = "Mostrando " + faturas.Count + " faturas";
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            AdminFatura fat = AdminRepository.GetFaturaById(id);
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            if (col == "ver") { AdminDialogos.MostrarDetalheFatura(this, fat); CarregarPagina(); }
            else if (col == "imprimir") AdminDialogos.ImprimirFatura(fat);
            else if (col == "importar") ImportarFaturas();
        }

        private void ImportarFaturas()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV|*.csv";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                int n = AdminRepository.ImportarFaturasCsv(ofd.FileName);
                MessageBox.Show(n + " faturas importadas.");
                CarregarPagina();
            }
        }

        private void Exportar_Click(object sender, EventArgs e) { AdminDialogos.ExportarGrid(dgvClientes, "faturas"); }
        private void label15_Click(object sender, EventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }

        private void DgvClientes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

    }
}