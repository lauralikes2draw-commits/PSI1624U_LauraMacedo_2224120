using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class ServicosAdmin : Form
    {
        private List<AdminServico> servicos = new List<AdminServico>(); //guada temporariamento os servicos carregados da base de dados

        public ServicosAdmin()
        {
            InitializeComponent();
            Load += ServicosAdmin_Load;
        }

        private void ServicosAdmin_Load(object sender, EventArgs e)
        {
            AdminSharedUi.PrepararPagina(this, "servicos");
            InicializarFiltros();
            LigarEventos();
            CarregarPagina();
        }

        private void InicializarFiltros()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Ativo", "Inativo" });
            cboEstado.SelectedIndex = 0;
            guna2TextBox1.PlaceholderText = "Pesquisar por nome, categoria ou descrição";
        }

        private void LigarEventos()
        {
            guna2TextBox1.TextChanged -= Filtro_Changed; guna2TextBox1.TextChanged += Filtro_Changed;//evita que a acao execute 2 vezes
            cboEstado.SelectedIndexChanged -= Filtro_Changed; cboEstado.SelectedIndexChanged += Filtro_Changed;
            dgvClientes.CellContentClick -= DgvClientes_CellContentClick; dgvClientes.CellContentClick += DgvClientes_CellContentClick;
            guna2Button1.Click -= Novo_Click; guna2Button1.Click += Novo_Click;
            guna2Button15.Click -= Novo_Click; guna2Button15.Click += Novo_Click;
            guna2Button8.Click -= Exportar_Click; guna2Button8.Click += Exportar_Click;
            guna2Button6.Click -= GerirPrecos_Click; guna2Button6.Click += GerirPrecos_Click;
            guna2Button7.Click -= Categorias_Click; guna2Button7.Click += Categorias_Click;
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
            int total = AdminRepository.CountServicos(null);
            int ativos = AdminRepository.CountServicos(true);
            int inativos = AdminRepository.CountServicos(false);
            lblClientes.Text = total.ToString();
            label8.Text = "+" + ativos + " ativos";
            label11.Text = ativos.ToString();
            label10.Text = total > 0 ? ((ativos * 100) / total) + "% do total" : "0% do total";
            label14.Text = inativos.ToString();
            label17.Text = inativos.ToString();
        }

        private void CarregarTop5()
        {
            label19.Text = "Serviços mais populares";
            List<AdminServico> top = AdminRepository.GetTopServicos(5);
            Label[] nomes = new Label[] { label21, label22, label23, label25, label26 };
            Label[] subs = new Label[] { label24, label18, label34, label35, label36 };
            Guna2CirclePictureBox[] fotos = new Guna2CirclePictureBox[] { guna2CirclePictureBox8, guna2CirclePictureBox13, guna2CirclePictureBox14, guna2CirclePictureBox15, guna2CirclePictureBox16 };
            Guna2CirclePictureBox[] ranks = new Guna2CirclePictureBox[] { guna2CirclePictureBox9, guna2CirclePictureBox10, guna2CirclePictureBox17, guna2CirclePictureBox18, guna2CirclePictureBox19 };

            for (int i = 0; i < nomes.Length; i++)
            {
                nomes[i].AutoEllipsis = true;
                nomes[i].AutoSize = false;
                nomes[i].Width = 165;
                subs[i].AutoEllipsis = true;
                subs[i].AutoSize = false;
                subs[i].Width = 165;

                if (i < top.Count)
                {
                    AdminServico s = top[i];
                    nomes[i].Text = s.Nome;
                    subs[i].Text = s.TotalMarcacoes + " marcações · ★ " + s.Avaliacao.ToString("0.0");
                    fotos[i].Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria);
                    fotos[i].SizeMode = PictureBoxSizeMode.Zoom;
                    fotos[i].Visible = true;
                    ranks[i].Visible = true;
                }
                else
                {
                    nomes[i].Text = "Sem serviço";
                    subs[i].Text = "";
                    fotos[i].Image = null;
                    fotos[i].Visible = false;
                    ranks[i].Visible = false;
                }
            }
        }

        private void CarregarResumo()
        {
            List<AdminServico> all = AdminRepository.GetServicos("", "Ativo", 5000);
            int ativos = all.Count;
            decimal totalPreco = 0m; int totalDur = 0; string categoriaTop = "-";
            Dictionary<string, int> cats = new Dictionary<string, int>();
            foreach (AdminServico s in all)
            {
                totalPreco += s.Preco; totalDur += s.DuracaoMinutos;
                string cat = string.IsNullOrWhiteSpace(s.Categoria) ? "Sem categoria" : s.Categoria;
                if (!cats.ContainsKey(cat)) cats[cat] = 0; cats[cat] += s.TotalMarcacoes;
            }
            int best = -1; foreach (string c in cats.Keys) if (cats[c] > best) { best = cats[c]; categoriaTop = c; }
            label31.Text = ativos.ToString();
            label28.Text = ativos > 0 ? AdminRepository.Money(totalPreco / ativos) : AdminRepository.Money(0);
            label20.Text = ativos > 0 ? (totalDur / ativos) + " min" : "0 min";
            List<AdminServico> top = AdminRepository.GetTopServicos(1);
            if (top.Count > 0)
            {
                label40.Text = top[0].Nome; label39.Text = "Margem " + (top[0].Preco > 0 ? "80%" : "0%");
                lblServicoPopular.Text = top[0].Avaliacao.ToString("0.0") + " / 5";
                lblQtdServico.Text = "Baseado nas avaliações";
                label48.Text = categoriaTop;
                label47.Text = best + " marcações";
            }
            label44.Text = ativos > 0 ? (totalDur / ativos) + " min" : "0 min";
            label53.Text = AdminRepository.Money(AdminRepository.SumMarcacoes(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today.AddMonths(1), "Confirmado"));
        }

        private void CarregarTabela()
        {
            string termo = guna2TextBox1.Text.Trim();
            string estado = cboEstado.SelectedItem == null ? "Todos" : cboEstado.SelectedItem.ToString();
            servicos = AdminRepository.GetServicos(termo, estado, 1000);
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            dgvClientes.Rows.Clear();
            foreach (AdminServico s in servicos)
            {
                int idx = dgvClientes.Rows.Add(AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria), s.Nome, s.Categoria, s.DuracaoMinutos + " min", AdminRepository.Money(s.Preco), s.Ativo ? "Ativo" : "Inativo", s.TotalMarcacoes, "Ver", "Editar", "Eliminar");
                dgvClientes.Rows[idx].Tag = s.IdServico;
            }
            AdminSharedUi.PintarEstado(dgvClientes, "estado");
            label51.Text = "Mostrando " + servicos.Count + " serviços";
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            AdminServico s = servicos.Find(x => x.IdServico == id);
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            if (col == "ver") AdminDialogos.MostrarDetalheServico(this, s);
            else if (col == "editar") AdminDialogos.EditarServico(this, s, CarregarPagina);
            else if (col == "eliminar") { if (MessageBox.Show("Deseja eliminar este serviço do catálogo? Ele será removido das categorias e das marcações futuras.", "BeauteCare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { AdminRepository.EliminarServico(id); MessageBox.Show("Serviço eliminado do catálogo."); } }
            CarregarPagina();
        }

        private void Novo_Click(object sender, EventArgs e) { AdminDialogos.CriarServico(this, CarregarPagina); }
        private void Exportar_Click(object sender, EventArgs e) { AdminDialogos.ExportarGrid(dgvClientes, "servicos"); }
        private void GerirPrecos_Click(object sender, EventArgs e) { MessageBox.Show("Use a tabela para ver os serviços e clique em Editar para ajustar dados/preços."); }
        private void Categorias_Click(object sender, EventArgs e)
        {
            AdminDialogos.MostrarCategoriasServicos(this);
        }
        private void guna2CirclePictureBox18_Click(object sender, EventArgs e) { }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
