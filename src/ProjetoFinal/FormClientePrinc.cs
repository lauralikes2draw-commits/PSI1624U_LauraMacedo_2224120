using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class FormClientePrinc : Form
    {
        private int idCliente;
        private List<ServicoInfo> recomendados = new List<ServicoInfo>();

        public FormClientePrinc()
        {
            InitializeComponent();
        }

        private void btnDefinicoes_Click(object sender, EventArgs e)
        {
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void FormClientePrinc_Load(object sender, EventArgs e)
        {
            try
            {
                ClienteSharedUi.PrepararPagina(this, "dashboard");
                idCliente = ClienteRepository.ResolverIdCliente();
                CarregarDashboard();
                ConfigurarEventos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dashboard da cliente: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarEventos()
        {
            btnMarcacoes.Click -= btnMarcacoes_Click;
            btnSair.Click -= btnSair_Click;
            guna2Button1.Click -= NovaMarcacao_Click;
            guna2Button1.Click += NovaMarcacao_Click;
            guna2Button8.Click -= VerTodasMarcacoes_Click;
            guna2Button8.Click += VerTodasMarcacoes_Click;
            guna2Button2.Click -= VerTodasMarcacoes_Click;
            guna2Button2.Click += VerTodasMarcacoes_Click;
            label11.Cursor = Cursors.Hand;
            label11.Click -= VerTodasMarcacoes_Click;
            label11.Click += VerTodasMarcacoes_Click;
            label6.Cursor = Cursors.Default;
            label6.Click -= Resgatar_Click;
            label10.Cursor = Cursors.Hand;
            label10.Click -= PromocaoMicroPigmentacao_Click;
            label10.Click += PromocaoMicroPigmentacao_Click;
            label12.Cursor = Cursors.Hand;
            label12.Click -= PromocaoMicroPigmentacao_Click;
            label12.Click += PromocaoMicroPigmentacao_Click;
            guna2PictureBox6.Cursor = Cursors.Hand;
            guna2PictureBox6.Click -= PromocaoMicroPigmentacao_Click;
            guna2PictureBox6.Click += PromocaoMicroPigmentacao_Click;
            guna2Button9.Click -= PromocaoMicroPigmentacao_Click;
            guna2Button9.Click += PromocaoMicroPigmentacao_Click;
            guna2Button6.Click -= Promocao1_Click;
            guna2Button6.Click += Promocao1_Click;
            guna2Button7.Click -= Promocao2_Click;
            guna2Button7.Click += Promocao2_Click;
            dgvClientes.CellContentClick -= DgvClientes_CellContentClick;
            dgvClientes.CellContentClick += DgvClientes_CellContentClick;
        }

        private void CarregarDashboard()
        {
            ClienteDashboardResumo res = ClienteRepository.GetDashboardResumo(idCliente);
            lblMarcacoesHoje.Text = res.ProximaMarcacao == null ? "Sem marcações" : res.ProximaMarcacao.DataMarcacao.ToString("dd/MM/yyyy") + "\r\n" + res.ProximaMarcacao.Hora.ToString(@"hh\:mm");
            lblServicos.Text = res.ServicoFavorito;
            label17.Text = res.ServicoFavoritoQtd > 0 ? "O que você mais solicita >" : "Faça a sua primeira marcação >";
            label7.Text = res.Pontos + " pontos";
            label48.Text = res.MarcacoesMes.ToString();
            label44.Text = res.HorasCuidadoMes;
            label38.Text = ClienteRepository.FormatarMoeda(res.TotalGastoMes);
            label37.Text = (res.VariacaoMes >= 0 ? "+" : "") + ClienteRepository.FormatarMoeda(res.VariacaoMes) + " este mês";
            label49.Text = "Marcações este mês";
            label45.Text = "Horas de cuidado";
            label39.Text = "Total gasto";

            CarregarTabelaProximas();
            CarregarNotificacoesPrincipais();
            CarregarPromocoes();
            CarregarServicosRecomendados();
        }

        private void CarregarTabelaProximas()
        {
            ClienteSharedUi.ConfigurarGridRosa(dgvClientes);
            dgvClientes.Rows.Clear();
            if (!dgvClientes.Columns.Contains("idMarcacao"))
            {
                DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
                idCol.Name = "idMarcacao";
                idCol.Visible = false;
                dgvClientes.Columns.Add(idCol);
            }
            dgvClientes.Columns["servico"].HeaderText = "Serviço";
            dgvClientes.Columns["ftProfissional"].HeaderText = "Foto";
            dgvClientes.Columns["ver"].HeaderText = "Ver";
            dgvClientes.Columns["ver"].DefaultCellStyle.ForeColor = ClienteSharedUi.Rosa;
            dgvClientes.Columns["ver"].DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            List<ClienteMarcacaoInfo> lista = ClienteRepository.GetProximasMarcacoes(idCliente, 6);
            foreach (ClienteMarcacaoInfo m in lista)
            {
                int row = dgvClientes.Rows.Add(m.DataMarcacao.ToString("dd/MM/yyyy"), m.Hora.ToString(@"hh\:mm"), m.Servico, ProfissionalSharedUi.CarregarImagemPerfil(m.ProfissionalFoto), m.Profissional, m.Estado, "Ver", m.IdMarcacao);
                dgvClientes.Rows[row].Tag = m.IdMarcacao;
            }
        }

        private void CarregarNotificacoesPrincipais()
        {
            CupaoInfo micro = ClienteRepository.GetPromocaoMicroPigmentacao(idCliente);
            guna2Button9.Tag = micro;

            label10.Text = micro == null ? "35% OFF" : micro.PercentualDesconto.ToString("0") + "% OFF";
            label12.Text = "em " + (micro == null ? "Micro Pigmentação" : micro.NomeServico);
            guna2Button9.Text = "Aproveitar agora";
        }

        private void CarregarPromocoes()
        {
            CupaoInfo p1 = ClienteRepository.GetPromocaoDoMes(idCliente, 0);
            CupaoInfo p2 = ClienteRepository.GetPromocaoDoMes(idCliente, 1);
            CupaoInfo micro = ClienteRepository.GetPromocaoMicroPigmentacao(idCliente);
            guna2Button9.Tag = micro;
            if (micro != null)
            {
                label10.Text = micro.PercentualDesconto.ToString("0") + "% OFF";
                label12.Text = "em " + micro.NomeServico;
            }
            else
            {
                label10.Text = "35% OFF";
                label12.Text = "em Micro Pigmentação";
            }

            if (p1 != null)
            {
                label41.Text = "Especial de " + DateTime.Today.ToString("MMMM", ClienteRepository.Pt);
                label42.Text = p1.PercentualDesconto.ToString("0") + "% OFF";
                label47.Text = "em " + p1.NomeServico;
                guna2Button6.Tag = p1;
            }
            if (p2 != null)
            {
                label51.Text = p2.PercentualDesconto.ToString("0") + "% OFF";
                label52.Text = "em " + p2.NomeServico;
                guna2Button7.Tag = p2;
            }
        }

        private void CarregarServicosRecomendados()
        {
            recomendados = ClienteRepository.GetServicosRecomendados(idCliente, 3);
            PreencherCardServico(0, guna2PictureBox1, label19, label20, label21, label22, guna2Button3);
            PreencherCardServico(1, guna2PictureBox2, label26, label25, label24, label23, guna2Button4);
            PreencherCardServico(2, guna2PictureBox3, label34, label33, label29, label27, guna2Button5);
        }

        private void PreencherCardServico(int index, Guna2PictureBox img, Label nome, Label duracao, Label desc, Label preco, Guna2Button botao)
        {
            if (index >= recomendados.Count)
            {
                botao.Enabled = false;
                nome.Text = "Serviço indisponível";
                return;
            }
            ServicoInfo s = recomendados[index];
            img.Image = ProfissionalSharedUi.CarregarImagemServico(s);
            img.SizeMode = PictureBoxSizeMode.Zoom;
            nome.Text = s.Nome;
            duracao.Text = s.DuracaoMinutos + " min";
            desc.Text = string.IsNullOrWhiteSpace(s.Categoria) ? "Procedimento especial\r\nBeauteCare" : s.Categoria + "\r\n★ " + s.Avaliacao.ToString("0.0");
            preco.Text = ClienteRepository.FormatarMoeda(s.Preco);
            botao.Text = "Marcar";
            botao.Tag = s.IdServico;
            botao.Click -= MarcarServico_Click;
            botao.Click += MarcarServico_Click;
        }

        private void NovaMarcacao_Click(object sender, EventArgs e)
        {
            using (NovaMarcacaoCliente modal = new NovaMarcacaoCliente(idCliente))
                modal.ShowDialog(this);
            CarregarDashboard();
        }

        private void MarcarServico_Click(object sender, EventArgs e)
        {
            Guna2Button b = sender as Guna2Button;
            if (b == null || b.Tag == null) return;
            int idServico = Convert.ToInt32(b.Tag);
            using (NovaMarcacaoCliente modal = new NovaMarcacaoCliente(idCliente, idServico))
                modal.ShowDialog(this);
            CarregarDashboard();
        }

        private void VerTodasMarcacoes_Click(object sender, EventArgs e)
        {
            MinhasMarcacoes f = new MinhasMarcacoes();
            f.Show();
            Hide();
        }

        private void VerNotificacoes_Click(object sender, EventArgs e)
        {
            ClienteSharedUi.MostrarPainelNotificacoes(this, idCliente, guna2CircleButton2);
        }

        private void Resgatar_Click(object sender, EventArgs e)
        {
            try
            {
                CupaoInfo c = ClienteRepository.GerarCupaoPontos(idCliente);
                MessageBox.Show("Cupão gerado: " + c.Codigo + "\nUse este código antes de pagar a próxima marcação.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void PromocaoMicroPigmentacao_Click(object sender, EventArgs e)
        {
            GerarCupaoPromocao(guna2Button9.Tag as CupaoInfo);
        }

        private void Promocao1_Click(object sender, EventArgs e)
        {
            GerarCupaoPromocao(guna2Button6.Tag as CupaoInfo);
        }

        private void Promocao2_Click(object sender, EventArgs e)
        {
            GerarCupaoPromocao(guna2Button7.Tag as CupaoInfo);
        }

        private void GerarCupaoPromocao(CupaoInfo promo)
        {
            try
            {
                if (promo == null || !promo.IdServico.HasValue)
                {
                    MessageBox.Show("Esta promoção ainda não está disponível.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (idCliente <= 0) idCliente = ClienteRepository.ResolverIdCliente();
                CupaoInfo c = ClienteRepository.GerarCupaoPromocao(idCliente, promo.IdServico.Value, promo.PercentualDesconto);
                if (c == null) throw new InvalidOperationException("Não foi possível gerar o cupão desta promoção.");
                MessageBox.Show("Cupão gerado: " + c.Codigo + "\nVálido apenas para " + c.NomeServico + ".", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível resgatar esta promoção: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvClientes.Columns[e.ColumnIndex].Name != "ver") return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Cells["idMarcacao"].Value);
            using (DetalheMarcacaoClienteForm f = new DetalheMarcacaoClienteForm(id))
                f.ShowDialog(this);
            CarregarDashboard();
        }

        private void label42_Click(object sender, EventArgs e)
        {
        }

        private void label41_Click(object sender, EventArgs e)
        {
        }

        private void btnMarcacoes_Click(object sender, EventArgs e)
        {
            VerTodasMarcacoes_Click(sender, e);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            FormLogin f = new FormLogin();
            f.Show();
            Hide();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }
    }
}
