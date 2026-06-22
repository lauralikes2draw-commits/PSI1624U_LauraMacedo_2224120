using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class FaturacaoProfissional : Form
    {
        public FaturacaoProfissional()
        {
            InitializeComponent();
            this.Load += FaturacaoProfissional_Load;
        }

        private void FaturacaoProfissional_Load(object sender, EventArgs e)
        {
            ProfissionalSharedUi.PrepararPagina(this, "faturacao");
            CorrigirTextos();
            ConfigurarAcoes();
            CarregarTudo();
        }

        private void CorrigirTextos()
        {
            AdminSharedUi.ColorirPrimeiroNome(label1, ProfissionalRepository.PrimeiroNome(UsuarioLogado.Nome));
            label6.Text = "Faturas";
            label7.Text = "Gerencie todos os seus pagamentos";
            label13.Text = "Comissão acumulada";
            label14.Text = "este mês";
            label9.Text = "Serviços realizados";
            label8.Text = "A receber no próximo pagamento";
            label15.Text = "Valor pendente";
            label12.Text = "Pagamento pela administração";
            label17.Text = "Próximo pagamento";
            label25.Text = "Total gerado";
            label26.Text = "Resumo do Mês";
            label28.Text = "Serviços realizados";
            label18.Text = "Sua comissão (%)";
            label19.Text = "Já pago";
            label21.Text = "Pendente a receber";
            label35.Text = "Histórico de Pagamentos";
            label33.Text = "Data do pagamento";
            label36.Text = "Valor";
            btnFaturas.Text = "Faturação";
            btnMarcacoes.Text = "Marcações";
            txtPesquisar.PlaceholderText = "Pesquisar clientes, marcações...";
            guna2TextBox1.PlaceholderText = "Pesquisar fatura, cliente, serviço...";

            data.HeaderText = "Data";
            cliente.HeaderText = "Cliente";
            servicos.HeaderText = "Serviços";
            valorDoServico.HeaderText = "Valor do Serviço";
            comissao.HeaderText = "Comissão (%)";
            suaComissao.HeaderText = "Sua Comissão";
            estado.HeaderText = "Estado";
            ver.HeaderText = "Ver";
            ver.Text = "Ver";
            ver.UseColumnTextForButtonValue = true;
        }

        private void ConfigurarAcoes()
        {
            ProfissionalSharedUi.ConfigurarGridRosa(dgvClientes);
            guna2TextBox1.TextChanged += FiltroAlterado;
            txtPesquisar.TextChanged += FiltroCabecalhoAlterado;
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("Todos");
            guna2ComboBox1.Items.Add("Paga");
            guna2ComboBox1.Items.Add("Pendente");
            guna2ComboBox1.Items.Add("Não paga");
            guna2ComboBox1.Items.Add("Cancelada");
            guna2ComboBox1.SelectedIndex = 0;
            guna2ComboBox1.SelectedIndexChanged += FiltroAlterado;
            dgvClientes.CellContentClick += DgvClientes_CellContentClick;
        }

        private void CarregarTudo()
        {
            try
            {
                DashboardResumo r = ProfissionalRepository.GetDashboardResumo(UsuarioLogado.Id);
                lblFaturacao.Text = ProfissionalRepository.FormatarMoeda(r.ComissaoMes);
                lblMarcacoesHoje.Text = r.ServicosMes.ToString();
                label10.Text = ProfissionalRepository.FormatarMoeda(r.PendenteReceber);
                label16.Text = r.ProximoPagamento.ToString("dd/MM/yy");

                label29.Text = ProfissionalRepository.FormatarMoeda(r.TotalGeradoMes);
                label31.Text = r.ServicosMes.ToString();
                label30.Text = r.ComissaoPercentual.ToString("0") + "%";
                label20.Text = ProfissionalRepository.FormatarMoeda(r.JaPago);
                label22.Text = ProfissionalRepository.FormatarMoeda(r.PendenteReceber);

                CarregarTabela();
                CarregarHistorico();
                ProfissionalSharedUi.AtualizarBadgeNotificacoes(this, UsuarioLogado.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar faturação: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CarregarTabela()
        {
            dgvClientes.Rows.Clear();
            string pesquisa = string.IsNullOrWhiteSpace(guna2TextBox1.Text) ? txtPesquisar.Text : guna2TextBox1.Text;
            string filtroEstado = guna2ComboBox1.SelectedItem == null ? "Todos" : guna2ComboBox1.SelectedItem.ToString();
            List<FaturaInfo> faturas = ProfissionalRepository.GetFaturas(UsuarioLogado.Id, pesquisa, filtroEstado);

            foreach (FaturaInfo f in faturas)
            {
                decimal comissaoValor = f.Total * f.ComissaoPercentual / 100m;
                int row = dgvClientes.Rows.Add(
                    f.DataFatura.ToString("dd/MM/yyyy"),
                    f.Cliente,
                    f.Servicos,
                    ProfissionalRepository.FormatarMoeda(f.Total),
                    f.ComissaoPercentual.ToString("0") + "%",
                    ProfissionalRepository.FormatarMoeda(comissaoValor),
                    NormalizarEstado(f.Estado),
                    "Ver");
                dgvClientes.Rows[row].Tag = f.IdFatura;
                AplicarCorEstadoFatura(row, NormalizarEstado(f.Estado));
            }

            if (faturas.Count == 0)
            {
                dgvClientes.Rows.Add("--", "Sem faturas", "", "", "", "", "", "");
            }
        }

        private void CarregarHistorico()
        {
            List<FaturaInfo> historico = ProfissionalRepository.GetHistoricoPagamentos(UsuarioLogado.Id);
            guna2ShadowPanel5.Controls.Clear();

            Label titulo = new Label();
            titulo.Text = "Histórico de pagamentos";
            titulo.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            titulo.ForeColor = ProfissionalSharedUi.Rosa;
            titulo.Location = new Point(22, 18);
            titulo.Size = new Size(235, 28);
            guna2ShadowPanel5.Controls.Add(titulo);

            Label sub = new Label();
            sub.Text = "Clique para ver a fatura recebida";
            sub.Font = new Font("Segoe UI", 8F);
            sub.ForeColor = ProfissionalSharedUi.Cinza;
            sub.Location = new Point(22, 44);
            sub.Size = new Size(235, 22);
            guna2ShadowPanel5.Controls.Add(sub);

            if (historico.Count == 0)
            {
                Label vazio = new Label();
                vazio.Text = "Ainda não existem pagamentos registados.";
                vazio.Font = new Font("Segoe UI", 9F);
                vazio.ForeColor = ProfissionalSharedUi.Cinza;
                vazio.Location = new Point(22, 92);
                vazio.Size = new Size(220, 50);
                guna2ShadowPanel5.Controls.Add(vazio);
                return;
            }

            int y = 78;
            int max = Math.Min(4, historico.Count);
            for (int i = 0; i < max; i++)
            {
                FaturaInfo f = historico[i];
                Guna2Panel card = new Guna2Panel();
                card.BorderRadius = 15;
                card.FillColor = Color.FromArgb(255, 248, 251);
                card.Size = new Size(230, 55);
                card.Location = new Point(22, y);
                card.Cursor = Cursors.Hand;
                card.Tag = f.IdFatura;
                card.Click += HistoricoPagamento_Click;

                Label data = new Label();
                data.Text = f.DataFatura.ToString("dd/MM/yyyy");
                data.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                data.ForeColor = ProfissionalSharedUi.Texto;
                data.Location = new Point(12, 8);
                data.Size = new Size(90, 20);
                data.Click += HistoricoPagamento_Click;
                card.Controls.Add(data);

                Label valor = new Label();
                valor.Text = ProfissionalRepository.FormatarMoeda(f.Total * f.ComissaoPercentual / 100m);
                valor.Font = new Font("Segoe UI", 9.3F, FontStyle.Bold);
                valor.ForeColor = ProfissionalSharedUi.Rosa;
                valor.TextAlign = ContentAlignment.MiddleRight;
                valor.Location = new Point(112, 8);
                valor.Size = new Size(105, 20);
                valor.Click += HistoricoPagamento_Click;
                card.Controls.Add(valor);

                Label detalhe = new Label();
                detalhe.Text = string.IsNullOrWhiteSpace(f.Cliente) ? "Pagamento da administração" : f.Cliente;
                detalhe.Font = new Font("Segoe UI", 7.8F);
                detalhe.ForeColor = ProfissionalSharedUi.Cinza;
                detalhe.Location = new Point(12, 30);
                detalhe.Size = new Size(205, 18);
                detalhe.AutoEllipsis = true;
                detalhe.Click += HistoricoPagamento_Click;
                card.Controls.Add(detalhe);

                guna2ShadowPanel5.Controls.Add(card);
                y += 62;
            }
        }

        private void HistoricoPagamento_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            while (c != null && !(c.Tag is int)) c = c.Parent;
            if (c == null || !(c.Tag is int)) return;
            int idFatura = Convert.ToInt32(c.Tag);
            using (ReciboFaturaForm recibo = new ReciboFaturaForm(idFatura))
                recibo.ShowDialog(this);
        }

        private void AplicarCorEstadoFatura(int row, string estadoFatura)
        {
            if (row < 0 || row >= dgvClientes.Rows.Count) return;
            DataGridViewCell cell = dgvClientes.Rows[row].Cells[estado.Name];
            string e = (estadoFatura ?? "").ToLowerInvariant();
            if (e.Contains("paga") && !e.Contains("não") && !e.Contains("nao"))
            {
                cell.Style.ForeColor = Color.FromArgb(35, 135, 75);
                cell.Style.BackColor = Color.FromArgb(224, 248, 232);
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (e.Contains("pend"))
            {
                cell.Style.ForeColor = Color.FromArgb(170, 110, 0);
                cell.Style.BackColor = Color.FromArgb(255, 246, 215);
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
            else if (e.Contains("não") || e.Contains("nao") || e.Contains("cancel"))
            {
                cell.Style.ForeColor = Color.FromArgb(190, 45, 65);
                cell.Style.BackColor = Color.FromArgb(255, 230, 236);
                cell.Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }

        private string NormalizarEstado(string estadoFatura)
        {
            if (string.IsNullOrWhiteSpace(estadoFatura)) return "Pendente";
            if (estadoFatura.Equals("Cancelado", StringComparison.OrdinalIgnoreCase)) return "Cancelada";
            return estadoFatura;
        }

        private void FiltroAlterado(object sender, EventArgs e)
        {
            CarregarTabela();
        }

        private void FiltroCabecalhoAlterado(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text)) CarregarTabela();
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvClientes.Columns[e.ColumnIndex] != ver) return;
            if (dgvClientes.Rows[e.RowIndex].Tag == null) return;
            int idFatura = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            using (ReciboFaturaForm recibo = new ReciboFaturaForm(idFatura))
            {
                recibo.ShowDialog(this);
            }
        }

        private void guna2Separator5_Click(object sender, EventArgs e)
        {
        }

        private void label35_Click(object sender, EventArgs e)
        {
        }
    }
}
