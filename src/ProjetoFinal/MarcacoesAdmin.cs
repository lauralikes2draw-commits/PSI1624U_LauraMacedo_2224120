using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjetoFinal
{
    public partial class MarcacoesAdmin : Form
    {
        private List<AdminMarcacao> marcacoes = new List<AdminMarcacao>();

        public MarcacoesAdmin()
        {
            InitializeComponent();
        }

        private void MarcacoesAdmin_Load(object sender, EventArgs e)
        {
            AdminSharedUi.PrepararPagina(this, "marcacoes");
            InicializarFiltros();
            LigarEventos();
            CarregarPagina();
        }

        private void InicializarFiltros()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Confirmado", "Cancelado", "Pendente", "Concluído" });
            cboEstado.SelectedIndex = 0;
            guna2TextBox1.PlaceholderText = "Pesquisar por cliente, profissional ou serviço";
        }

        private void LigarEventos()
        {
            guna2TextBox1.TextChanged -= Filtro_Changed; guna2TextBox1.TextChanged += Filtro_Changed;
            cboEstado.SelectedIndexChanged -= Filtro_Changed; cboEstado.SelectedIndexChanged += Filtro_Changed;
            dgvClientes.CellContentClick -= DgvClientes_CellContentClick; dgvClientes.CellContentClick += DgvClientes_CellContentClick;
            guna2Button1.Click -= Nova_Click; guna2Button1.Click += Nova_Click;
            guna2Button10.Click -= Nova_Click; guna2Button10.Click += Nova_Click;
            guna2Button5.Click -= Agenda_Click; guna2Button5.Click += Agenda_Click;
            guna2Button8.Click -= Lembrete_Click; guna2Button8.Click += Lembrete_Click;
            guna2Button9.Click -= ConfirmarPendentes_Click; guna2Button9.Click += ConfirmarPendentes_Click;
            guna2Button15.Click -= Exportar_Click; guna2Button15.Click += Exportar_Click;
            guna2Button7.Text = "<"; guna2Button6.Text = ">";
            AdminSharedUi.AplicarPaginacaoSetas(this, "guna2Button7", "guna2Button6");
        }

        private void Filtro_Changed(object sender, EventArgs e) { CarregarTabela(); }

        private void CarregarPagina()
        {
            CarregarPaineis();
            CarregarHoje();
            CarregarResumo();
            CarregarTabela();
        }

        private void CarregarPaineis()
        {
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            lblClientes.Text = AdminRepository.CountMarcacoes(null, null, "Todos").ToString();
            label8.Text = "+" + AdminRepository.CountMarcacoes(inicioMes, hoje, "Todos") + " este mês";
            label11.Text = AdminRepository.CountMarcacoes(null, null, "Confirmado").ToString();
            label14.Text = AdminRepository.CountMarcacoes(null, null, "Pendente").ToString();
            label17.Text = AdminRepository.CountMarcacoes(null, null, "Cancelado").ToString();
            label15.Text = "+" + AdminRepository.CountMarcacoes(inicioMes, hoje, "Cancelado") + " este mês";
        }

        private void CarregarHoje()
        {
            List<AdminMarcacao> hoje = AdminRepository.GetMarcacoes("", "Todos", DateTime.Today, DateTime.Today, 3);
            Label[] nomes = new Label[] { label21, label22, label23 };
            Label[] servs = new Label[] { label24, label25, label26 };
            Guna.UI2.WinForms.Guna2Button[] horas = new Guna.UI2.WinForms.Guna2Button[] { guna2Button2, guna2Button3, guna2Button4 };
            for (int i = 0; i < 3; i++)
            {
                if (i < hoje.Count)
                {
                    nomes[i].Text = hoje[i].Cliente;
                    servs[i].Text = hoje[i].Servico;
                    horas[i].Text = hoje[i].Hora.ToString(@"hh\:mm");
                    horas[i].Tag = hoje[i].IdMarcacao;
                    horas[i].Click -= Hora_Click; horas[i].Click += Hora_Click;
                }
                else
                {
                    nomes[i].Text = "Sem marcação"; servs[i].Text = ""; horas[i].Text = "--:--"; horas[i].Tag = null;
                }
            }
        }

        private void Hora_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button b = sender as Guna.UI2.WinForms.Guna2Button;
            if (b == null || b.Tag == null) return;
            int id = Convert.ToInt32(b.Tag);
            foreach (AdminMarcacao m in AdminRepository.GetMarcacoes("", "Todos", null, null, 2000)) if (m.IdMarcacao == id) AdminDialogos.MostrarDetalheMarcacao(this, m);
            CarregarPagina();
        }

        private void CarregarResumo()
        {
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            label31.Text = AdminRepository.CountMarcacoes(hoje, hoje, "Todos").ToString();
            label28.Text = AdminRepository.Money(AdminRepository.SumMarcacoes(hoje, hoje, "Confirmado"));
            label20.Text = "1h";
            List<AdminServico> topS = AdminRepository.GetTopServicos(1); if (topS.Count > 0) { label40.Text = topS[0].Nome; label39.Text = topS[0].TotalMarcacoes + " marcações"; }
            List<AdminProfissional> topP = AdminRepository.GetTopProfissionais(1); if (topP.Count > 0) { label35.Text = topP[0].Nome; label34.Text = topP[0].AtendimentosMes + " marcações"; }
            int confirmadas = AdminRepository.CountMarcacoes(inicioMes, hoje, "Confirmado") + AdminRepository.CountMarcacoes(inicioMes, hoje, "Concluído");
            int total = Math.Max(1, AdminRepository.CountMarcacoes(inicioMes, hoje, "Todos"));
            label43.Text = ((confirmadas * 100) / total) + "%";
            label53.Text = AdminRepository.Money(AdminRepository.SumMarcacoes(inicioMes, hoje.AddMonths(1), "Confirmado"));
        }

        private void CarregarTabela()
        {
            string termo = guna2TextBox1.Text.Trim();
            string estado = cboEstado.SelectedItem == null ? "Todos" : cboEstado.SelectedItem.ToString();
            marcacoes = AdminRepository.GetMarcacoes(termo, estado, null, null, 1000);
            AdminSharedUi.ConfigurarGrid(dgvClientes);
            dgvClientes.Rows.Clear();
            foreach (AdminMarcacao m in marcacoes)
            {
                int idx = dgvClientes.Rows.Add(AdminSharedUi.CarregarImagemPerfil(m.ClienteFoto), m.Cliente, m.Profissional, m.Servico, m.DataMarcacao.ToString("dd/MM/yyyy"), m.Hora.ToString(@"hh\:mm"), m.Estado, "Ver", "Editar", "Cancelar");
                dgvClientes.Rows[idx].Tag = m.IdMarcacao;
            }
            AdminSharedUi.PintarEstado(dgvClientes, "estado");
            label56.Text = "Mostrando " + marcacoes.Count + " marcações";
        }

        private void DgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            int id = Convert.ToInt32(dgvClientes.Rows[e.RowIndex].Tag);
            AdminMarcacao m = marcacoes.Find(x => x.IdMarcacao == id);
            string col = dgvClientes.Columns[e.ColumnIndex].Name;
            if (col == "ver") AdminDialogos.MostrarDetalheMarcacao(this, m);
            else if (col == "editar") AdminDialogos.CriarMarcacao(this, CarregarPagina);
            else if (col == "cancelar") { AdminRepository.SetEstadoMarcacao(id, "Cancelada"); MessageBox.Show("Marcação cancelada."); }
            CarregarPagina();
        }

        private void Nova_Click(object sender, EventArgs e) { AdminDialogos.CriarMarcacao(this, CarregarPagina); }
        private void Agenda_Click(object sender, EventArgs e) { AdminDialogos.MostrarAgendaDia(this); }
        private void Lembrete_Click(object sender, EventArgs e) { AdminDialogos.MostrarEnviarMensagem(this, null); }
        private void ConfirmarPendentes_Click(object sender, EventArgs e)
        {
            foreach (AdminMarcacao m in AdminRepository.GetMarcacoes("", "Pendente", null, null, 2000)) AdminRepository.SetEstadoMarcacao(m.IdMarcacao, "Confirmada");
            MessageBox.Show("Marcações pendentes confirmadas.");
            CarregarPagina();
        }
        private void Exportar_Click(object sender, EventArgs e) { AdminDialogos.ExportarGrid(dgvClientes, "marcacoes"); }

        private void guna2CirclePictureBox11_Click(object sender, EventArgs e) { }
        private void guna2Button6_Click(object sender, EventArgs e) { }
        private void guna2ShadowPanel5_Paint(object sender, PaintEventArgs e) { }
    }
}
