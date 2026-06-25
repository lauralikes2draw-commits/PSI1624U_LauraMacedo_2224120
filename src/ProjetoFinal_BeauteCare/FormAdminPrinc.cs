using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class FormAdminPrinc : Form
    {
        private List<AdminCliente> clientesInativos = new List<AdminCliente>();

        public FormAdminPrinc()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void FormAdminPrinc_Load(object sender, EventArgs e)
        {
            try
            {
                AdminSharedUi.PrepararPagina(this, "dashboard");
                CarregarDashboard();
                LigarEventosDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dashboard da administração: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LigarEventosDashboard()
        {
            if (guna2Button1 != null) { guna2Button1.Click -= AbrirMarcacoes_Click; guna2Button1.Click += AbrirMarcacoes_Click; }
            if (label20 != null) { label20.Cursor = Cursors.Hand; label20.Click -= AbrirClientes_Click; label20.Click += AbrirClientes_Click; }
            if (guna2Button2 != null) { guna2Button2.Click -= Contactar1_Click; guna2Button2.Click += Contactar1_Click; }
            if (guna2Button3 != null) { guna2Button3.Click -= Contactar2_Click; guna2Button3.Click += Contactar2_Click; }
            if (guna2Button4 != null) { guna2Button4.Click -= Contactar3_Click; guna2Button4.Click += Contactar3_Click; }
        }

        private void CarregarDashboard()
        {
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime inicioMesAnterior = inicioMes.AddMonths(-1);
            DateTime fimMesAnterior = inicioMes.AddDays(-1);

            lblClientes.Text = AdminRepository.CountClientes(true).ToString();
            label8.Text = "+" + AdminRepository.CountNewClientesMes() + " este mês";

            lblMarcacoesHoje.Text = AdminRepository.CountMarcacoes(hoje, hoje, "Todos").ToString();
            label11.Text = "+" + AdminRepository.CountMarcacoes(inicioMes, hoje, "Todos") + " este mês";

            decimal fatMes = AdminRepository.SumFaturas(inicioMes, hoje, "Todos");
            if (fatMes <= 0) fatMes = AdminRepository.SumMarcacoes(inicioMes, hoje, "Concluído");
            decimal fatAnterior = AdminRepository.SumFaturas(inicioMesAnterior, fimMesAnterior, "Todos");
            lblFaturacao.Text = AdminRepository.Money(fatMes);
            decimal variacao = fatAnterior > 0 ? ((fatMes - fatAnterior) / fatAnterior) * 100m : 0m;
            label14.Text = (variacao >= 0 ? "+" : "") + Math.Round(variacao) + "% este mês";

            lblServicos.Text = AdminRepository.CountServicos(true).ToString();
            label17.Text = "+" + AdminRepository.CountServicos(true) + " ativos";

            CarregarAgendaHoje();
            CarregarClientesInativos();
            CarregarGraficoFaturacao();
            CarregarPaineisInferiores();
            AdminSharedUi.PreencherNotificacoesResumo(this);
        }

        private void CarregarAgendaHoje()
        {
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            dgvClientes.Rows.Clear();
            foreach (AdminMarcacao m in AdminRepository.GetMarcacoes("", "Todos", DateTime.Today, DateTime.Today, 4))
            {
                dgvClientes.Rows.Add(AdminSharedUi.CarregarImagemPerfil(m.ClienteFoto), m.Cliente, m.Hora.ToString(@"hh\:mm"), m.Profissional, m.Servico, m.Estado);
            }
            AdminSharedUi.PintarEstado(dgvClientes, "dataGridViewTextBoxColumn5");
        }

        private void CarregarClientesInativos()
        {
            clientesInativos = AdminRepository.GetClientesInativos(3);
            Label[] nomes = new Label[] { label21, label22, label23 };
            Label[] visitas = new Label[] { label24, label25, label26 };
            Guna2CirclePictureBox[] fotos = new Guna2CirclePictureBox[] { guna2CirclePictureBox8, guna2CirclePictureBox9, guna2CirclePictureBox10 };
            for (int i = 0; i < 3; i++)
            {
                if (i < clientesInativos.Count)
                {
                    AdminCliente c = clientesInativos[i];
                    nomes[i].Text = c.Nome;
                    nomes[i].Tag = c.IdUsuario;
                    visitas[i].Text = "Última visita: " + (c.UltimaVisita.HasValue ? c.UltimaVisita.Value.ToString("dd/MM/yyyy") : "Nunca");
                    fotos[i].Image = AdminSharedUi.CarregarImagemPerfil(c.Foto);
                    fotos[i].SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    nomes[i].Text = "Sem cliente";
                    visitas[i].Text = "";
                    fotos[i].Image = AdminSharedUi.CarregarImagemPerfil("");
                }
            }
        }

        private void CarregarGraficoFaturacao()
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(245, 220, 230);
            Series s = new Series("Faturação");
            s.ChartType = SeriesChartType.SplineArea;
            s.Color = Color.FromArgb(140, 255, 79, 135);
            s.BorderColor = AdminSharedUi.Rosa;
            s.BorderWidth = 3;
            chart1.Series.Add(s);
            DateTime inicio = DateTime.Today.AddDays(-30);
            List<AdminFatura> faturas = AdminRepository.GetFaturas("", "Todos", inicio, DateTime.Today, 500);
            Dictionary<string, decimal> porDia = new Dictionary<string, decimal>();
            foreach (AdminFatura f in faturas)
            {
                string key = f.DataFatura.ToString("dd/MM");
                if (!porDia.ContainsKey(key)) porDia[key] = 0;
                porDia[key] += f.Total;
            }
            for (int i = 30; i >= 0; i--)
            {
                DateTime d = DateTime.Today.AddDays(-i);
                string key = d.ToString("dd/MM");
                s.Points.AddXY(key, porDia.ContainsKey(key) ? (double)porDia[key] : 0d);
            }
        }

        private void CarregarPaineisInferiores()
        {
            List<AdminServico> topServ = AdminRepository.GetTopServicos(1);
            if (topServ.Count > 0)
            {
                lblServicoPopular.Text = topServ[0].Nome;
                lblQtdServico.Text = topServ[0].TotalMarcacoes + " vezes";
            }
            else { lblServicoPopular.Text = "Nenhum serviço"; lblQtdServico.Text = "0 vezes"; }

            List<AdminProfissional> topProf = AdminRepository.GetTopProfissionais(1);
            if (topProf.Count > 0)
            {
                lblProfissional.Text = topProf[0].Nome;
                lblQtdAtendimentos.Text = topProf[0].AtendimentosMes + " serviços";
            }
            else { lblProfissional.Text = "Nenhuma profissional"; lblQtdAtendimentos.Text = "0 serviços"; }

            int marcHoje = AdminRepository.CountMarcacoes(DateTime.Today, DateTime.Today, "Todos");
            int capacidade = Math.Max(1, AdminRepository.CountProfissionais(true) * 8);
            int taxa = Math.Min(100, (marcHoje * 100) / capacidade);
            lblTaxa.Text = taxa + "%";
            guna2ProgressBar1.Value = taxa;
        }

        private void AbrirMarcacoes_Click(object sender, EventArgs e) { MarcacoesAdmin f = new MarcacoesAdmin(); f.Show(); Hide(); }
        private void AbrirClientes_Click(object sender, EventArgs e) { ClientesAdmin f = new ClientesAdmin(); f.Show(); Hide(); }
        private void Contactar1_Click(object sender, EventArgs e) { Contactar(0); }
        private void Contactar2_Click(object sender, EventArgs e) { Contactar(1); }
        private void Contactar3_Click(object sender, EventArgs e) { Contactar(2); }
        private void Contactar(int index)
        {
            if (index < clientesInativos.Count) AdminSharedUi.MostrarContacto(clientesInativos[index]);
        }

        private void label5_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label29_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e) { }
        private void btnFaturas_Click(object sender, EventArgs e) { }
        private void guna2Button4_Click(object sender, EventArgs e) { }
        private void btnClientes_Click(object sender, EventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
    }
}
