using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class FaturasClientes : Form
    {
        private int idCliente;
        private Guna2ShadowPanel painelBeneficios;

        public FaturasClientes()
        {
            InitializeComponent();
            this.Load += FaturasClientes_Load;
        }

        private void FaturasClientes_Load(object sender, EventArgs e)
        {
            try
            {
                ClienteSharedUi.PrepararPagina(this, "faturacao");
                idCliente = ClienteRepository.ResolverIdCliente();
                ConfigurarControles();
                CarregarResumo();
                CarregarTabela();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar faturas: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarControles()
        {
            label6.Text = "Faturas";
            label7.Text = "Gerencie todas as suas faturas e gastos do centro de estética";
            label36.Text = "Resumo e benefícios";
            label18.Text = "Total investido";
            label20.Text = "Média por mês";
            label22.Text = "Faturas pagas";
            label24.Text = "Cupão por pontos";
            guna2TextBox1.PlaceholderText = "Pesquisar faturas...";
            guna2TextBox1.TextChanged -= Filtros_Changed;
            guna2TextBox1.TextChanged += Filtros_Changed;

            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.AddRange(new object[] { "Todos", "Paga", "Pendente", "Não paga" });
            guna2ComboBox1.SelectedIndex = 0;
            guna2ComboBox1.SelectedIndexChanged -= Filtros_Changed;
            guna2ComboBox1.SelectedIndexChanged += Filtros_Changed;

            DataGridViewComboBoxColumn metodo = dgvClientes.Columns["metodoPagamento"] as DataGridViewComboBoxColumn;
            if (metodo != null)
            {
                metodo.Items.Clear();
                metodo.Items.AddRange("Cartão", "Cartao", "MBWay", "MB WAY", "Dinheiro", "Multibanco", "Transferência", "Transferencia", "App", "Importado", "");
                metodo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                metodo.FlatStyle = FlatStyle.Flat;
            }

            dgvClientes.CellContentClick -= DgvClientes_CellContentClick;
            dgvClientes.CellContentClick += DgvClientes_CellContentClick;
            dgvClientes.DataError -= DgvClientes_DataError; dgvClientes.DataError += DgvClientes_DataError;
        }

        private void CarregarResumo()
        {
            ClienteDashboardResumo res = ClienteRepository.GetDashboardResumo(idCliente);
            lblFaturacao.Text = ClienteRepository.FormatarMoeda(res.TotalGastoAno);
            label11.Text = res.FaturasPagasAno.ToString();
            label9.Text = ClienteRepository.FormatarMoeda(res.FaturasPendentes);
            label8.Text = res.FaturasPendentes > 0 ? "Pagamento pendente" : "Sem pagamentos pendentes";
            label16.Text = res.Pontos + " pontos";
            label19.Text = ClienteRepository.FormatarMoeda(res.TotalGastoAno);
            label21.Text = ClienteRepository.FormatarMoeda(res.MediaMensal);
            label23.Text = res.FaturasPagasAno + " pagas";
            label25.Text = res.Pontos >= 100 ? "Disponível" : (100 - res.Pontos) + " pts faltam";
            label19.AutoEllipsis = true; label21.AutoEllipsis = true; label23.AutoEllipsis = true; label25.AutoEllipsis = true;
            label19.Width = 105; label21.Width = 105; label23.Width = 105; label25.Width = 105;
            CarregarPainelBeneficios(res);
        }

        private void CarregarPainelBeneficios(ClienteDashboardResumo res)
        {
            if (painelBeneficios == null)
            {
                painelBeneficios = new Guna2ShadowPanel();
                painelBeneficios.Name = "painelBeneficiosFatura";
                painelBeneficios.BackColor = Color.Transparent;
                painelBeneficios.FillColor = Color.White;
                painelBeneficios.Radius = 20;
                painelBeneficios.ShadowColor = Color.Black;
                painelBeneficios.ShadowDepth = 20;
                painelBeneficios.Location = new Point(guna2ShadowPanel5.Left, guna2ShadowPanel5.Bottom + 18);
                painelBeneficios.Size = new Size(guna2ShadowPanel5.Width, 260);
                this.Controls.Add(painelBeneficios);
                painelBeneficios.BringToFront();
            }
            painelBeneficios.Controls.Clear();

            Label titulo = new Label();
            titulo.Text = "Próxima vantagem";
            titulo.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            titulo.ForeColor = ClienteSharedUi.Rosa;
            titulo.Location = new Point(24, 18);
            titulo.Size = new Size(235, 28);
            painelBeneficios.Controls.Add(titulo);

            int faltam = Math.Max(0, 100 - res.Pontos);
            Label destaque = new Label();
            destaque.Text = res.Pontos >= 100 ? "Cupão disponível" : faltam + " pontos para novo cupão";
            destaque.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            destaque.ForeColor = ClienteSharedUi.Texto;
            destaque.Location = new Point(24, 62);
            destaque.Size = new Size(245, 34);
            destaque.AutoEllipsis = true;
            painelBeneficios.Controls.Add(destaque);

            Guna2ProgressBar barra = new Guna2ProgressBar();
            barra.Location = new Point(24, 106);
            barra.Size = new Size(245, 14);
            barra.BorderRadius = 7;
            barra.FillColor = Color.FromArgb(255, 230, 240);
            barra.ProgressColor = ClienteSharedUi.Rosa;
            barra.ProgressColor2 = Color.FromArgb(255, 150, 185);
            barra.Maximum = 100;
            barra.Value = Math.Min(100, Math.Max(0, res.Pontos));
            painelBeneficios.Controls.Add(barra);

            Label texto = new Label();
            texto.Text = res.Pontos >= 100
                ? "Já pode resgatar um desconto por pontos na próxima marcação."
                : "Cada pagamento concluído aproxima-a de benefícios exclusivos.";
            texto.Font = new Font("Segoe UI", 9F);
            texto.ForeColor = ClienteSharedUi.Cinza;
            texto.Location = new Point(24, 136);
            texto.Size = new Size(245, 46);
            painelBeneficios.Controls.Add(texto);

            Guna2Button botao = new Guna2Button();
            botao.Text = res.Pontos >= 100 ? "Resgatar cupão" : "Agendar novo serviço";
            botao.BorderRadius = 18;
            botao.FillColor = ClienteSharedUi.Rosa;
            botao.ForeColor = Color.White;
            botao.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            botao.Location = new Point(24, 198);
            botao.Size = new Size(245, 38);
            botao.Click += delegate
            {
                if (res.Pontos >= 100) ResgatarCupaoPontosNaFatura();
                else AbrirNovaMarcacaoNaFatura();
            };
            painelBeneficios.Controls.Add(botao);
        }

        private void ResgatarCupaoPontosNaFatura()
        {
            try
            {
                CupaoInfo c = ClienteRepository.GerarCupaoPontos(idCliente);
                MessageBox.Show("Cupão gerado: " + c.Codigo + Environment.NewLine + "Use este código antes de pagar a próxima marcação.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarResumo();
                CarregarTabela();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AbrirNovaMarcacaoNaFatura()
        {
            using (NovaMarcacaoCliente modal = new NovaMarcacaoCliente(idCliente))
                modal.ShowDialog(this);
            CarregarResumo();
            CarregarTabela();
        }

        private void CarregarTabela()
        {
            string pesquisa = guna2TextBox1.Text.Trim();
            string estado = guna2ComboBox1.SelectedItem == null ? "Todos" : guna2ComboBox1.SelectedItem.ToString();
            List<FaturaInfo> faturas = ClienteRepository.GetFaturas(idCliente, pesquisa, estado);

            ClienteSharedUi.ConfigurarGridRosa(dgvClientes);
            dgvClientes.Rows.Clear();
            if (!dgvClientes.Columns.Contains("idFatura"))
            {
                DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
                idCol.Name = "idFatura";
                idCol.Visible = false;
                dgvClientes.Columns.Add(idCol);
            }
            dgvClientes.Columns["nFatura"].HeaderText = "N.º Fatura";
            dgvClientes.Columns["servicos"].HeaderText = "Serviços";
            dgvClientes.Columns["metodoPagamento"].HeaderText = "Método de pagamento";
            dgvClientes.Columns["ver"].DefaultCellStyle.ForeColor = ClienteSharedUi.Rosa;
            dgvClientes.Columns["ver"].DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            foreach (FaturaInfo f in faturas)
            {
                string metodo = string.IsNullOrWhiteSpace(f.MetodoPagamento) ? "Multibanco" : f.MetodoPagamento;
                int row = dgvClientes.Rows.Add(f.NumeroFatura, f.DataFatura.ToString("dd/MM/yyyy"), f.Servicos, ClienteRepository.FormatarMoeda(f.Desconto), ClienteRepository.FormatarMoeda(f.Total), metodo, f.Estado, "Ver", f.IdFatura);
                dgvClientes.Rows[row].Tag = f.IdFatura;
            }
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            CarregarTabela();
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvClientes.Columns[e.ColumnIndex].Name != "ver") return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Cells["idFatura"].Value);
            using (ReciboFaturaForm f = new ReciboFaturaForm(id, false))
                f.ShowDialog(this);
        }

        private void DgvClientes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

    }
}