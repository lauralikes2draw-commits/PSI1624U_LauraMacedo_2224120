using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class MinhasMarcacoes : Form
    {
        private int idCliente;

        public MinhasMarcacoes()
        {
            InitializeComponent();
            this.Load += MinhasMarcacoes_Load;
        }

        private void MinhasMarcacoes_Load(object sender, EventArgs e)
        {
            try
            {
                ClienteSharedUi.PrepararPagina(this, "marcacoes");
                idCliente = ClienteRepository.ResolverIdCliente();
                ConfigurarControles();
                CarregarResumo();
                CarregarTabela();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar marcações: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarControles()
        {
            label6.Text = "Marcações";
            label7.Text = "Gerencie todas as suas marcações no centro de estética";
            guna2Button1.Text = "+ Nova Marcação";
            guna2Button1.Click -= NovaMarcacao_Click;
            guna2Button1.Click += NovaMarcacao_Click;

            guna2TextBox1.PlaceholderText = "Pesquisar marcações...";
            guna2TextBox1.TextChanged -= Filtros_Changed;
            guna2TextBox1.TextChanged += Filtros_Changed;

            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Pendente", "Confirmado", "Concluído", "Cancelado" });
            cboEstado.SelectedIndex = 0;
            cboEstado.SelectedIndexChanged -= Filtros_Changed;
            cboEstado.SelectedIndexChanged += Filtros_Changed;

            dgvClientes.CellContentClick -= DgvClientes_CellContentClick;
            dgvClientes.CellContentClick += DgvClientes_CellContentClick;
        }

        private void CarregarResumo()
        {
            ClienteDashboardResumo res = ClienteRepository.GetDashboardResumo(idCliente);
            lblClientes.Text = res.ProximaMarcacao == null ? "Sem marcações" : res.ProximaMarcacao.DataMarcacao.ToString("dd MMM, yyyy", ClienteRepository.Pt);
            label10.Text = res.ProximaMarcacao == null ? "" : res.ProximaMarcacao.DataMarcacao.ToString("dddd", ClienteRepository.Pt) + ", " + res.ProximaMarcacao.Hora.ToString(@"hh\:mm");
            lblServicos.Text = res.ServicoFavorito;
            label17.Text = res.ServicoFavoritoQtd + " vezes realizado";
            label12.Text = res.Pontos + " pontos";
            label14.Text = res.PromocoesDisponiveis + " ativas";
            label9.Text = "Próxima Marcação";
            label15.Text = "Serviço favorito";
            label13.Text = "Seus pontos";
            label16.Text = "Promoções disponíveis";
        }

        private void CarregarTabela()
        {
            string pesquisa = guna2TextBox1.Text.Trim();
            string estado = cboEstado.SelectedItem == null ? "Todos" : cboEstado.SelectedItem.ToString();
            List<ClienteMarcacaoInfo> lista = ClienteRepository.GetMarcacoes(idCliente, pesquisa, estado);

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
            dgvClientes.Columns["foto"].HeaderText = "Foto";
            dgvClientes.Columns["fotoProf"].HeaderText = "Foto";
            dgvClientes.Columns["ver"].DefaultCellStyle.ForeColor = ClienteSharedUi.Rosa;
            dgvClientes.Columns["cancelar"].DefaultCellStyle.ForeColor = Color.FromArgb(220, 78, 92);
            dgvClientes.Columns["ver"].DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvClientes.Columns["cancelar"].DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            foreach (ClienteMarcacaoInfo m in lista)
            {
                ServicoInfo fake = new ServicoInfo { Nome = m.Servico, Foto = m.ServicoFoto, Categoria = m.Servico };
                string cancelar = (m.Estado.ToLowerInvariant().Contains("cancel") || m.Estado.ToLowerInvariant().Contains("concl")) ? "" : "Cancelar";
                int row = dgvClientes.Rows.Add(ProfissionalSharedUi.CarregarImagemServico(fake), m.Servico, ProfissionalSharedUi.CarregarImagemPerfil(m.ProfissionalFoto), m.Profissional, m.DataMarcacao.ToString("dd/MM/yyyy"), m.Hora.ToString(@"hh\:mm"), m.Estado, "Ver", cancelar, m.IdMarcacao);
                dgvClientes.Rows[row].Tag = m.IdMarcacao;
            }
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            CarregarTabela();
        }

        private void NovaMarcacao_Click(object sender, EventArgs e)
        {
            using (NovaMarcacaoCliente f = new NovaMarcacaoCliente(idCliente))
                f.ShowDialog(this);
            CarregarResumo();
            CarregarTabela();
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Cells["idMarcacao"].Value);
            if (col == "ver")
            {
                using (DetalheMarcacaoClienteForm f = new DetalheMarcacaoClienteForm(id))
                    f.ShowDialog(this);
                CarregarResumo();
                CarregarTabela();
            }
            else if (col == "cancelar" && Convert.ToString(dgvClientes.Rows[e.RowIndex].Cells["cancelar"].Value) != "")
            {
                if (MessageBox.Show("Deseja cancelar esta marcação?", "BeauteCare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ClienteRepository.CancelarMarcacao(id, idCliente);
                    MessageBox.Show("Marcação cancelada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarResumo();
                    CarregarTabela();
                }
            }
        }

        private void lblClientes_Click(object sender, EventArgs e)
        {
        }

        private void label56_Click(object sender, EventArgs e)
        {
        }
    }
}
