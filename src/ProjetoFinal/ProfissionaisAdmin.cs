using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class ProfissionaisAdmin : Form
    {
        private List<AdminProfissional> profissionais = new List<AdminProfissional>();

        public ProfissionaisAdmin()
        {
            InitializeComponent();
            Load += ProfissionaisAdmin_Load;
        }

        private void ProfissionaisAdmin_Load(object sender, EventArgs e)
        {
            AdminSharedUi.PrepararPagina(this, "profissionais");
            InicializarFiltros();
            LigarEventos();
            CarregarPagina();
        }

        private void InicializarFiltros()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Ativo", "Inativo" });
            cboEstado.SelectedIndex = 0;
            guna2TextBox1.PlaceholderText = "Pesquisar por nome, telefone, email ou especialidade";
        }

        private void LigarEventos()
        {
            guna2TextBox1.TextChanged -= Filtro_Changed; guna2TextBox1.TextChanged += Filtro_Changed;
            cboEstado.SelectedIndexChanged -= Filtro_Changed; cboEstado.SelectedIndexChanged += Filtro_Changed;
            dgvClientes.CellContentClick -= DgvClientes_CellContentClick; dgvClientes.CellContentClick += DgvClientes_CellContentClick;
            guna2Button1.Click -= Novo_Click; guna2Button1.Click += Novo_Click;
            guna2Button17.Click -= Novo_Click; guna2Button17.Click += Novo_Click;
            guna2Button16.Click -= Importar_Click; guna2Button16.Click += Importar_Click;
            guna2Button15.Click -= Mensagem_Click; guna2Button15.Click += Mensagem_Click;
            guna2Button5.Click -= Top10_Click; guna2Button5.Click += Top10_Click;
            guna2Button8.Click -= Exportar_Click; guna2Button8.Click += Exportar_Click;
            guna2Button9.Text = "<"; guna2Button10.Text = ">";
            AdminSharedUi.AplicarPaginacaoSetas(this, "guna2Button9", "guna2Button10");
        }

        private void Filtro_Changed(object sender, EventArgs e) { CarregarTabela(); }

        private void CarregarPagina()
        {
            CarregarPaineis();
            CarregarTop5();
            CarregarResumo();
            CarregarTabela();
        }

        private void CarregarPaineis()
        {
            int total = AdminRepository.CountProfissionais(null);
            int ativos = AdminRepository.CountProfissionais(true);
            int inativos = AdminRepository.CountProfissionais(false);
            lblClientes.Text = total.ToString();
            label8.Text = "+" + ativos + " ativas";
            label11.Text = ativos.ToString();
            label10.Text = total > 0 ? ((ativos * 100) / total) + "% do total" : "0% do total";
            label14.Text = inativos.ToString();
            List<AdminProfissional> top = AdminRepository.GetTopProfissionais(1);
            if (top.Count > 0) { label17.Text = top[0].Nome; lblServicos.Text = top[0].Avaliacao.ToString("0.0") + "/5"; }
        }

        private void CarregarTop5()
        {
            label28.Text = "Top 5 funcionárias";
            List<AdminProfissional> top = AdminRepository.GetTopProfissionais(5);
            Label[] nomes = new Label[] { label27, label22, label23, label20, label25 };
            Label[] subs = new Label[] { label24, label18, label19, label21, label26 };
            Guna2CirclePictureBox[] fotos = new Guna2CirclePictureBox[] { guna2CirclePictureBox20, guna2CirclePictureBox13, guna2CirclePictureBox10, guna2CirclePictureBox8, guna2CirclePictureBox9 };

            for (int i = 0; i < nomes.Length; i++)
            {
                if (i < top.Count)
                {
                    nomes[i].Text = top[i].Nome;
                    nomes[i].AutoEllipsis = true;
                    nomes[i].Width = 160;
                    subs[i].Text = "★ " + top[i].Avaliacao.ToString("0.0") + "  •  " + top[i].Servicos + " atendimentos";
                    if (i < fotos.Length && fotos[i] != null)
                    {
                        fotos[i].Image = AdminSharedUi.CarregarImagemPerfil(top[i].Foto);
                        fotos[i].SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                else
                {
                    nomes[i].Text = "Sem profissional";
                    subs[i].Text = "";
                    if (i < fotos.Length && fotos[i] != null) fotos[i].Image = AdminSharedUi.CarregarImagemPerfil("");
                }
            }
        }

        private void CarregarResumo()
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            List<AdminProfissional> top = AdminRepository.GetTopProfissionais(1);
            if (top.Count > 0)
            {
                label48.Text = top[0].Nome; label47.Text = top[0].AtendimentosMes + " atendimentos";
                label35.Text = "84%";
            }
            List<AdminServico> topS = AdminRepository.GetTopServicos(1);
            if (topS.Count > 0) { label40.Text = topS[0].Nome; label39.Text = topS[0].TotalMarcacoes + " serviços"; }
            label44.Text = AdminRepository.CountMarcacoes(inicioMes, DateTime.Today, "Confirmado") + "h";
        }

        private void CarregarTabela()
        {
            string termo = guna2TextBox1.Text.Trim();
            string estado = cboEstado.SelectedItem == null ? "Todos" : cboEstado.SelectedItem.ToString();
            profissionais = AdminRepository.GetProfissionais(termo, estado, 1000);
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            dgvClientes.Rows.Clear();
            foreach (AdminProfissional p in profissionais)
            {
                int idx = dgvClientes.Rows.Add(AdminSharedUi.CarregarImagemPerfil(p.Foto), p.Nome, p.Telefone, p.Especialidade, p.Email, p.Ativo ? "Ativo" : "Inativo", p.Avaliacao.ToString("0.0"), p.Servicos, "Ver", "Editar", false);
                dgvClientes.Rows[idx].Tag = p.IdUsuario;
            }
            AdminSharedUi.PintarEstado(dgvClientes, "estado");
            label51.Text = "Mostrando " + profissionais.Count + " profissionais";
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            AdminProfissional p = profissionais.Find(x => x.IdUsuario == id);
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            if (col == "ver") AdminDialogos.MostrarDetalheProfissional(this, p);
            else if (col == "editar") AdminDialogos.EditarProfissional(this, p, CarregarPagina);
            else if (col == "desativar") { AdminRepository.DesativarUsuario(id, p == null || !p.Ativo); MessageBox.Show("Estado atualizado."); }
            CarregarPagina();
        }

        private void Novo_Click(object sender, EventArgs e) { AdminDialogos.CriarProfissional(this, CarregarPagina); }
        private void Importar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV|*.csv";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                int n = AdminRepository.ImportarProfissionaisCsv(ofd.FileName);
                MessageBox.Show(n + " profissionais importadas.");
                CarregarPagina();
            }
        }
        private void Mensagem_Click(object sender, EventArgs e) { AdminDialogos.MostrarEnviarMensagem(this, null); }
        private void Top10_Click(object sender, EventArgs e) { AdminDialogos.MostrarTopProfissionais(this); }
        private void Exportar_Click(object sender, EventArgs e) { AdminDialogos.ExportarGrid(dgvClientes, "profissionais"); }
        private void label16_Click(object sender, EventArgs e) { }
    }
}
