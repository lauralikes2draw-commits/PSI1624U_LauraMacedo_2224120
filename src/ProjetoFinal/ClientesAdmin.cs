using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class ClientesAdmin : Form
    {
        private List<AdminCliente> clientes = new List<AdminCliente>();
        private List<AdminCliente> inativos = new List<AdminCliente>();

        public ClientesAdmin()
        {
            InitializeComponent();
            Load += ClientesAdmin_Load;
        }

        private void ClientesAdmin_Load(object sender, EventArgs e)
        {
            AdminSharedUi.PrepararPagina(this, "clientes");
            InicializarFiltros();
            LigarEventos();
            CarregarPagina();
        }

        private void InicializarFiltros()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Ativo", "Inativo" });
            cboEstado.SelectedIndex = 0;
            guna2TextBox1.PlaceholderText = "Pesquisar por nome, telefone ou email";
        }

        private void LigarEventos()
        {
            guna2TextBox1.TextChanged -= Filtro_Changed; guna2TextBox1.TextChanged += Filtro_Changed;
            cboEstado.SelectedIndexChanged -= Filtro_Changed; cboEstado.SelectedIndexChanged += Filtro_Changed;
            dgvClientes.CellContentClick -= dgvClientes_CellContentClick; dgvClientes.CellContentClick += dgvClientes_CellContentClick;
            guna2Button1.Click -= NovoCliente_Click; guna2Button1.Click += NovoCliente_Click;
            guna2Button17.Click -= NovoCliente_Click; guna2Button17.Click += NovoCliente_Click;
            guna2Button16.Click -= Importar_Click; guna2Button16.Click += Importar_Click;
            guna2Button15.Click -= EnviarMensagem_Click; guna2Button15.Click += EnviarMensagem_Click;
            guna2Button8.Click -= Exportar_Click; guna2Button8.Click += Exportar_Click;
            guna2Button9.Text = "<";
            guna2Button10.Text = ">";
            AdminSharedUi.AplicarPaginacaoSetas(this, "guna2Button9", "guna2Button10");
        }

        private void Filtro_Changed(object sender, EventArgs e) { CarregarTabela(); }

        private void CarregarPagina()
        {
            CarregarPaineis();
            CarregarInativos();
            CarregarResumoRapido();
            CarregarTabela();
        }

        private void CarregarPaineis()
        {
            int total = AdminRepository.CountClientes(null);
            int ativos = AdminRepository.CountClientes(true);
            int inat = AdminRepository.CountClientes(false);
            lblClientes.Text = total.ToString();
            label8.Text = "+" + AdminRepository.CountNewClientesMes() + " este mês";
            label11.Text = ativos.ToString();
            label10.Text = total > 0 ? ((ativos * 100) / total) + "% do total" : "0% do total";
            label14.Text = inat.ToString();
            label13.Text = "Necessitam contacto";
            lblServicos.Text = AdminRepository.CountServicos(true).ToString();
            label17.Text = "+" + AdminRepository.CountServicos(true) + " ativos";
        }

        private void CarregarInativos()
        {
            inativos = AdminRepository.GetClientesInativos(3);
            Label[] nomes = new Label[] { label21, label22, label23 };
            Label[] visitas = new Label[] { label24, label25, label26 };
            Guna2CirclePictureBox[] fotos = new Guna2CirclePictureBox[] { guna2CirclePictureBox8, guna2CirclePictureBox9, guna2CirclePictureBox10 };
            Guna2Button[] botoes = new Guna2Button[] { guna2Button2, guna2Button3, guna2Button4 };
            for (int i = 0; i < 3; i++)
            {
                if (i < inativos.Count)
                {
                    AdminCliente c = inativos[i];
                    nomes[i].Text = c.Nome;
                    visitas[i].Text = "Última visita: " + (c.UltimaVisita.HasValue ? c.UltimaVisita.Value.ToString("dd/MM/yyyy") : "Nunca");
                    fotos[i].Image = AdminSharedUi.CarregarImagemPerfil(c.Foto);
                    fotos[i].SizeMode = PictureBoxSizeMode.Zoom;
                    botoes[i].Text = "Contactar";
                    botoes[i].Tag = c.IdUsuario;
                    botoes[i].Click -= ContactarInativo_Click;
                    botoes[i].Click += ContactarInativo_Click;
                }
                else
                {
                    nomes[i].Text = "Sem cliente"; visitas[i].Text = ""; fotos[i].Image = AdminSharedUi.CarregarImagemPerfil(""); botoes[i].Tag = null;
                }
            }
        }

        private void ContactarInativo_Click(object sender, EventArgs e)
        {
            Guna2Button b = sender as Guna2Button;
            if (b == null || b.Tag == null) return;
            int id = Convert.ToInt32(b.Tag);
            foreach (AdminCliente c in inativos) if (c.IdUsuario == id) AdminSharedUi.MostrarContacto(c);
        }

        private void CarregarResumoRapido()
        {
            DateTime inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            label20.Text = AdminRepository.CountMarcacoes(inicioMes, DateTime.Today, "Todos").ToString();
            label28.Text = AdminRepository.Money(AdminRepository.SumFaturas(inicioMes, DateTime.Today, "Todos"));
            label30.Text = AdminRepository.CountNewClientesMes().ToString();
            List<AdminServico> topS = AdminRepository.GetTopServicos(1);
            if (topS.Count > 0) label40.Text = topS[0].Nome;
            List<AdminCliente> cs = AdminRepository.GetClientes("", "Ativo", 5000);
            AdminCliente frequente = null, gasto = null;
            foreach (AdminCliente c in cs)
            {
                if (frequente == null || c.TotalServicos > frequente.TotalServicos) frequente = c;
                if (gasto == null || c.TotalGasto > gasto.TotalGasto) gasto = c;
            }
            if (gasto != null) { label44.Text = gasto.Nome; label43.Text = AdminRepository.Money(gasto.TotalGasto); }
            if (frequente != null) { label48.Text = frequente.Nome; label47.Text = frequente.TotalServicos + " marcações"; }
            lblServicoPopular.Text = "4.8 / 5";
            lblQtdServico.Text = "Baseado nas avaliações";
        }

        private void CarregarTabela()
        {
            string termo = guna2TextBox1.Text.Trim();
            string estado = cboEstado.SelectedItem == null ? "Todos" : cboEstado.SelectedItem.ToString();
            clientes = AdminRepository.GetClientes(termo, estado, 1000);
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            dgvClientes.Rows.Clear();
            foreach (AdminCliente c in clientes)
            {
                int idx = dgvClientes.Rows.Add(AdminSharedUi.CarregarImagemPerfil(c.Foto), c.Nome, c.Telefone, c.Email, c.UltimaVisita.HasValue ? c.UltimaVisita.Value.ToString("dd/MM/yyyy") : "Nunca", c.Ativo ? "Ativo" : "Inativo", "Ver", "Editar", "Eliminar", "Agenda");
                dgvClientes.Rows[idx].Tag = c.IdUsuario;
            }
            AdminSharedUi.PintarEstado(dgvClientes, "estado");
            label51.Text = "Mostrando " + clientes.Count + " clientes";
        }

        private void NovoCliente_Click(object sender, EventArgs e)
        {
            AdminDialogos.CriarCliente(this, CarregarPagina);
        }

        private void Importar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV|*.csv";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                int n = AdminRepository.ImportarClientesCsv(ofd.FileName);
                MessageBox.Show(n + " clientes importadas.");
                CarregarPagina();
            }
        }

        private void EnviarMensagem_Click(object sender, EventArgs e)
        {
            AdminDialogos.MostrarEnviarMensagem(this, null);
        }

        private void Exportar_Click(object sender, EventArgs e) { AdminDialogos.ExportarGrid(dgvClientes, "clientes"); }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            AdminCliente c = AdminRepository.GetCliente(id);
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            if (col == "ver") AdminDialogos.MostrarDetalheCliente(this, c);
            else if (col == "editar") AdminDialogos.EditarCliente(c);
            else if (col == "eliminar") { if (MessageBox.Show("Deseja desativar esta cliente?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) AdminRepository.DesativarUsuario(id, false); }
            else if (col == "agenda") AdminDialogos.MostrarAgendaCliente(this, id, c == null ? "" : c.Nome);
            CarregarPagina();
        }

        private void label9_Click(object sender, EventArgs e) { }
        private void guna2CirclePictureBox19_Click(object sender, EventArgs e) { }
    }
}
