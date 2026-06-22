using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class DashboardProfissional : Form
    {
        private Guna2ShadowPanel painelPesquisa;

        public DashboardProfissional()
        {
            InitializeComponent();
            this.Load += DashboardProfissional_Load;
        }

        private void DashboardProfissional_Load(object sender, EventArgs e)
        {
            ProfissionalSharedUi.PrepararPagina(this, "dashboard");
            CorrigirTextos();
            ConfigurarAcoes();
            CarregarDashboard();
        }

        private void CorrigirTextos()
        {
            AdminSharedUi.ColorirPrimeiroNome(label1, ProfissionalRepository.PrimeiroNome(UsuarioLogado.Nome));
            label6.Text = "Ver agenda >";
            label9.Text = "Marcações hoje";
            label7.Text = "Comissão (Este Mês)";
            label14.Text = "Ver ganhos >";
            label15.Text = "Avaliação Média";
            label10.Text = "Próximas Marcações";
            label12.Text = "Seu desempenho";
            label16.Text = "Seus serviços realizados nos últimos 6 meses";
            label17.Text = "Resumo do Mês";
            label18.Text = "Sua comissão (%)";
            label19.Text = "Já pago";
            label21.Text = "Pendente a receber";
            label25.Text = "Total gerado";
            label28.Text = "Serviços realizados";
            label33.Text = "Data prevista";
            label35.Text = "Próximo pagamento";
            label36.Text = "Valor estimado a receber";
            label34.Text = "Os pagamentos são realizados pela\r\nadministração todo dia 15\r\nde cada mês.";
            guna2Button5.Text = "Ver Relatório";
            btnFaturas.Text = "Faturação";
            btnMarcacoes.Text = "Marcações";
            txtPesquisar.PlaceholderText = "Pesquisar clientes, marcações, faturas...";

            horario.HeaderText = "Horário";
            cliente.HeaderText = "Cliente";
            servico.HeaderText = "Serviço";
            estado.HeaderText = "Estado";
        }

        private void ConfigurarAcoes()
        {
            label6.Cursor = Cursors.Hand;
            label11.Cursor = Cursors.Hand;
            label14.Cursor = Cursors.Hand;
            guna2Button8.Cursor = Cursors.Hand;
            guna2Button5.Cursor = Cursors.Hand;

            label6.Click += AbrirMarcacoes_Click;
            label11.Click += AbrirMarcacoes_Click;
            guna2Button8.Click += AbrirMarcacoes_Click;
            label14.Click += AbrirFaturacao_Click;
            guna2Button5.Click += AbrirFaturacao_Click;

            txtPesquisar.TextChanged += TxtPesquisar_TextChanged;
            txtPesquisar.KeyDown += TxtPesquisar_KeyDown;
            ProfissionalSharedUi.ConfigurarGridRosa(dgvClientes);
            dgvClientes.RowTemplate.Height = 44;
            dgvClientes.RowTemplate.MinimumHeight = 44;
            foto.ImageLayout = DataGridViewImageCellLayout.Zoom;
            foto.Width = 48;
            foto.HeaderText = "";
        }

        private void CarregarDashboard()
        {
            try
            {
                int id = ProfissionalRepository.ResolverIdProfissional();
                DashboardResumo r = ProfissionalRepository.GetDashboardResumo(id);

                lblMarcacoesHoje.Text = r.MarcacoesHoje.ToString();
                lblFaturacao.Text = r.ServicosSemana + " Serviços";
                label8.Text = ProfissionalRepository.FormatarMoeda(r.ComissaoMes);
                lblServicos.Text = r.AvaliacaoMedia.ToString("0.0") + "/5";
                guna2RatingStar1.Value = (float)Math.Max(0, Math.Min(5, r.AvaliacaoMedia));

                label29.Text = ProfissionalRepository.FormatarMoeda(r.TotalGeradoMes);
                label31.Text = r.ServicosMes.ToString();
                label30.Text = r.ComissaoPercentual.ToString("0") + "%";
                label20.Text = ProfissionalRepository.FormatarMoeda(r.JaPago);
                label22.Text = ProfissionalRepository.FormatarMoeda(r.PendenteReceber);
                label23.Text = ProfissionalRepository.FormatarMoeda(r.PendenteReceber);
                label26.Text = r.ProximoPagamento.ToString("dd/MM/yyyy");

                CarregarProximasMarcacoes(id);
                CarregarGrafico(id);
                ProfissionalSharedUi.AtualizarBadgeNotificacoes(this, id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dashboard: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CarregarProximasMarcacoes(int idProfissional)
        {
            dgvClientes.Rows.Clear();
            List<MarcacaoInfo> proximas = ProfissionalRepository.GetProximasMarcacoes(idProfissional, 8);
            Image imgPadrao = ProfissionalSharedUi.CarregarImagemPerfil("");

            foreach (MarcacaoInfo m in proximas)
            {
                Image fotoCliente = ProfissionalSharedUi.CarregarImagemPerfil(m.ClienteFoto);
                string horarioTexto = m.DataMarcacao.ToString("dd/MM") + " " + m.Hora.ToString(@"hh\:mm");
                int row = dgvClientes.Rows.Add(horarioTexto, fotoCliente, m.Cliente, m.Servico, NormalizarEstado(m.Estado));
                dgvClientes.Rows[row].Tag = m.IdMarcacao;
            }

            if (proximas.Count == 0)
            {
                dgvClientes.Rows.Add("--", imgPadrao, "Sem próximas marcações", "", "");
            }
        }

        private string NormalizarEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "Pendente";
            if (estado.Equals("Cacelado", StringComparison.OrdinalIgnoreCase)) return "Cancelado";
            return estado;
        }

        private void CarregarGrafico(int idProfissional)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            ChartArea area = new ChartArea("Desempenho");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(245, 220, 230);
            area.AxisX.LabelStyle.ForeColor = ProfissionalSharedUi.Cinza;
            area.AxisY.LabelStyle.ForeColor = ProfissionalSharedUi.Cinza;
            chart1.ChartAreas.Add(area);

            Series serie = new Series("Serviços");
            serie.ChartType = SeriesChartType.SplineArea;
            serie.Color = Color.FromArgb(120, 255, 79, 135);
            serie.BorderColor = ProfissionalSharedUi.Rosa;
            serie.BorderWidth = 3;
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 8;
            serie.MarkerColor = ProfissionalSharedUi.Rosa;
            chart1.Series.Add(serie);

            Dictionary<string, int> dados = ProfissionalRepository.GetDesempenhoMensal(idProfissional);
            foreach (KeyValuePair<string, int> item in dados)
                serie.Points.AddXY(item.Key, item.Value);
        }

        private void TxtPesquisar_TextChanged(object sender, EventArgs e)
        {
            MostrarResultadosPesquisa(txtPesquisar.Text.Trim());
        }

        private void TxtPesquisar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) FecharPesquisa();
        }

        private void MostrarResultadosPesquisa(string termo)
        {
            FecharPesquisa();
            if (termo.Length < 2) return;

            List<SearchItem> resultados;
            try { resultados = ProfissionalRepository.PesquisarTudo(UsuarioLogado.Id, termo); }
            catch { return; }

            painelPesquisa = new Guna2ShadowPanel();
            painelPesquisa.Name = "painelPesquisaDashboard";
            painelPesquisa.Size = new Size(txtPesquisar.Width + 210, 340);
            painelPesquisa.Radius = 18;
            painelPesquisa.FillColor = Color.White;
            painelPesquisa.ShadowColor = Color.Gray;
            painelPesquisa.ShadowDepth = 15;

            Point screen = txtPesquisar.Parent.PointToScreen(new Point(txtPesquisar.Left, txtPesquisar.Bottom + 6));
            painelPesquisa.Location = this.PointToClient(screen);

            Label titulo = new Label();
            titulo.Text = "Resultados encontrados";
            titulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            titulo.ForeColor = ProfissionalSharedUi.Texto;
            titulo.Location = new Point(16, 12);
            titulo.Size = new Size(250, 26);
            painelPesquisa.Controls.Add(titulo);

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Location = new Point(14, 48);
            flow.Size = new Size(painelPesquisa.Width - 28, 275);
            flow.AutoScroll = true;
            flow.FlowDirection = FlowDirection.TopDown;
            flow.WrapContents = false;
            painelPesquisa.Controls.Add(flow);

            if (resultados.Count == 0)
            {
                Label vazio = new Label();
                vazio.Text = "Nenhum resultado para “" + termo + "”.";
                vazio.ForeColor = ProfissionalSharedUi.Cinza;
                vazio.Font = new Font("Segoe UI", 10F);
                vazio.Size = new Size(flow.Width - 20, 40);
                flow.Controls.Add(vazio);
            }
            else
            {
                foreach (SearchItem item in resultados)
                    flow.Controls.Add(CriarLinhaResultado(item));
            }

            this.Controls.Add(painelPesquisa);
            painelPesquisa.BringToFront();
        }

        private Control CriarLinhaResultado(SearchItem item)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(painelPesquisa.Width - 52, 70);
            card.FillColor = Color.FromArgb(255, 248, 251);
            card.BorderRadius = 14;
            card.Margin = new Padding(0, 0, 0, 8);
            card.Cursor = Cursors.Hand;

            Label tipo = new Label();
            tipo.Text = item.Tipo;
            tipo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            tipo.ForeColor = ProfissionalSharedUi.Rosa;
            tipo.Location = new Point(12, 8);
            tipo.Size = new Size(120, 18);
            card.Controls.Add(tipo);

            Label titulo = new Label();
            titulo.Text = item.Titulo;
            titulo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            titulo.ForeColor = ProfissionalSharedUi.Texto;
            titulo.Location = new Point(12, 26);
            titulo.Size = new Size(card.Width - 24, 22);
            card.Controls.Add(titulo);

            Label sub = new Label();
            sub.Text = item.Subtitulo;
            sub.Font = new Font("Segoe UI", 8.3F);
            sub.ForeColor = ProfissionalSharedUi.Cinza;
            sub.Location = new Point(12, 48);
            sub.Size = new Size(card.Width - 24, 20);
            card.Controls.Add(sub);

            card.Click += delegate
            {
                if (item.Tipo == "Fatura") AbrirFaturacao_Click(card, EventArgs.Empty);
                else if (item.Tipo == "Marcação") AbrirMarcacoes_Click(card, EventArgs.Empty);
            };
            return card;
        }

        private void FecharPesquisa()
        {
            if (painelPesquisa != null)
            {
                this.Controls.Remove(painelPesquisa);
                painelPesquisa.Dispose();
                painelPesquisa = null;
            }
        }

        private void AbrirMarcacoes_Click(object sender, EventArgs e)
        {
            MarcacoesProfissionais f = new MarcacoesProfissionais();
            f.Show();
            this.Hide();
        }

        private void AbrirFaturacao_Click(object sender, EventArgs e)
        {
            FaturacaoProfissional f = new FaturacaoProfissional();
            f.Show();
            this.Hide();
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }
    }
}
