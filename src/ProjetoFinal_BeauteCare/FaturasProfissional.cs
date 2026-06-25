using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class MarcacoesProfissionais : Form
    {
        private Panel agendaHost;
        private DateTime dataAtual;

        public MarcacoesProfissionais()
        {
            InitializeComponent();
            this.Load += MarcacoesProfissionais_Load;
        }

        private void MarcacoesProfissionais_Load(object sender, EventArgs e)
        {
            ProfissionalSharedUi.PrepararPagina(this, "marcacoes");
            CorrigirTextos();
            PrepararAgendaDinamica();
            ConfigurarAcoes();
            dataAtual = DateTime.Today;
            guna2DateTimePicker1.Value = dataAtual;
            CarregarAgenda();
        }

        private void CorrigirTextos()
        {
            AdminSharedUi.ColorirPrimeiroNome(label1, ProfissionalRepository.PrimeiroNome(UsuarioLogado.Nome));
            label6.Text = "Marcações";
            label7.Text = "Gerencie todas as suas marcações do centro de estética";
            btnFaturas.Text = "Faturação";
            btnMarcacoes.Text = "Marcações";
            txtPesquisar.PlaceholderText = "Pesquisar clientes, marcações...";
            guna2Button12.Text = "+ Nova Marcação";
            label59.Text = "Confirmado";
            label60.Text = "Pendente";
            label61.Text = "Cancelado";
        }

        private void ConfigurarAcoes()
        {
            guna2Button1.Click += ProximoDia_Click;
            guna2Button8.Click += DiaAnterior_Click;
            guna2DateTimePicker1.ValueChanged += DatePicker_ValueChanged;
            guna2Button12.Click += NovaMarcacao_Click;
            txtPesquisar.TextChanged += TxtPesquisar_TextChanged;
        }

        private void PrepararAgendaDinamica()
        {
            foreach (Control c in guna2ShadowPanel18.Controls)
            {
                if (c is Guna2Panel) c.Visible = false;
            }

            agendaHost = new Panel();
            agendaHost.Name = "agendaHostDinamica";
            agendaHost.Location = new Point(120, 72);
            agendaHost.Size = new Size(1035, 645);
            agendaHost.AutoScroll = true;
            agendaHost.BackColor = Color.Transparent;
            guna2ShadowPanel18.Controls.Add(agendaHost);
            agendaHost.BringToFront();
        }

        private void CarregarAgenda()
        {
            if (agendaHost == null) return;
            agendaHost.Controls.Clear();
            try
            {
                List<MarcacaoInfo> marcacoes = ProfissionalRepository.GetMarcacoesPorDia(UsuarioLogado.Id, dataAtual);
                int y = 5;

                if (marcacoes.Count == 0)
                {
                    agendaHost.Controls.Add(CriarMensagemVazia());
                    return;
                }

                foreach (MarcacaoInfo m in marcacoes)
                {
                    Control card = CriarCardMarcacao(m);
                    card.Location = new Point(5, y);
                    agendaHost.Controls.Add(card);
                    y += card.Height + 14;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar marcações: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Control CriarMensagemVazia()
        {
            Guna2Panel vazio = new Guna2Panel();
            vazio.Size = new Size(1000, 150);
            vazio.BorderRadius = 18;
            vazio.FillColor = Color.FromArgb(255, 248, 251);

            Label icon = new Label();
            icon.Text = "♡";
            icon.Font = new Font("Segoe UI", 34F, FontStyle.Bold);
            icon.ForeColor = ProfissionalSharedUi.Rosa;
            icon.TextAlign = ContentAlignment.MiddleCenter;
            icon.Location = new Point(0, 12);
            icon.Size = new Size(1000, 52);
            vazio.Controls.Add(icon);

            Label msg = new Label();
            msg.Text = "Não tem marcações para este dia.";
            msg.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            msg.ForeColor = ProfissionalSharedUi.Texto;
            msg.TextAlign = ContentAlignment.MiddleCenter;
            msg.Location = new Point(0, 68);
            msg.Size = new Size(1000, 32);
            vazio.Controls.Add(msg);

            Label sub = new Label();
            sub.Text = "Use “+ Nova Marcação” para criar um agendamento.";
            sub.Font = new Font("Segoe UI", 9.5F);
            sub.ForeColor = ProfissionalSharedUi.Cinza;
            sub.TextAlign = ContentAlignment.MiddleCenter;
            sub.Location = new Point(0, 102);
            sub.Size = new Size(1000, 24);
            vazio.Controls.Add(sub);
            return vazio;
        }

        private Control CriarCardMarcacao(MarcacaoInfo m)
        {
            Guna2Panel outer = new Guna2Panel();
            outer.Size = new Size(1000, 82);
            outer.BorderRadius = 18;
            outer.FillColor = CorEstado(m.Estado, true);
            outer.Margin = new Padding(0, 0, 0, 12);

            Guna2Panel inner = new Guna2Panel();
            inner.Location = new Point(5, 0);
            inner.Size = new Size(995, 82);
            inner.BorderRadius = 18;
            inner.FillColor = Color.FromArgb(255, 248, 251);
            outer.Controls.Add(inner);

            Label hora = new Label();
            hora.Text = m.Hora.ToString(@"hh\:mm") + " - " + m.Hora.Add(TimeSpan.FromMinutes(m.DuracaoMinutos)).ToString(@"hh\:mm");
            hora.Font = new Font("Segoe UI", 8.5F);
            hora.ForeColor = ProfissionalSharedUi.Cinza;
            hora.Location = new Point(18, 10);
            hora.Size = new Size(150, 22);
            inner.Controls.Add(hora);

            Label servico = new Label();
            servico.Text = string.IsNullOrWhiteSpace(m.Servico) ? "Serviço" : m.Servico;
            servico.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            servico.ForeColor = ProfissionalSharedUi.Texto;
            servico.Location = new Point(18, 36);
            servico.Size = new Size(300, 26);
            inner.Controls.Add(servico);

            Label cliente = new Label();
            cliente.Text = m.Cliente;
            cliente.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            cliente.ForeColor = ProfissionalSharedUi.Texto;
            cliente.Location = new Point(330, 35);
            cliente.Size = new Size(210, 26);
            inner.Controls.Add(cliente);

            Label valor = new Label();
            valor.Text = ProfissionalRepository.FormatarMoeda(m.Valor);
            valor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            valor.ForeColor = ProfissionalSharedUi.Rosa;
            valor.Location = new Point(555, 35);
            valor.Size = new Size(100, 26);
            inner.Controls.Add(valor);

            Guna2Button estado = new Guna2Button();
            estado.Text = NormalizarEstado(m.Estado);
            estado.BorderRadius = 18;
            estado.FillColor = CorEstado(m.Estado, false);
            estado.ForeColor = CorTextoEstado(m.Estado);
            estado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            estado.Size = new Size(145, 38);
            estado.Location = new Point(690, 22);
            estado.Enabled = false;
            inner.Controls.Add(estado);

            Guna2Button mais = new Guna2Button();
            mais.Text = "...";
            mais.BorderRadius = 16;
            mais.FillColor = Color.White;
            mais.ForeColor = ProfissionalSharedUi.Rosa;
            mais.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            mais.Size = new Size(58, 38);
            mais.Location = new Point(880, 22);
            mais.Cursor = Cursors.Hand;
            mais.Click += delegate { AbrirMenuMarcacao(mais, m); };
            inner.Controls.Add(mais);

            TornarCardClicavel(outer, m);
            return outer;
        }

        private void TornarCardClicavel(Control control, MarcacaoInfo m)
        {
            if (control == null || m == null) return;
            if (!(control is Guna2Button))
            {
                control.Cursor = Cursors.Hand;
                control.Click += delegate { MostrarDetalheMarcacaoProfissional(m); };
            }
            foreach (Control child in control.Controls) TornarCardClicavel(child, m);
        }

        private void MostrarDetalheMarcacaoProfissional(MarcacaoInfo m)
        {
            if (m == null) return;
            using (Form f = new Form())
            {
                f.Text = "Detalhes da marcação";
                f.Size = new Size(650, 560);
                f.StartPosition = FormStartPosition.CenterParent;
                f.BackColor = Color.White;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;

                Label titulo = new Label();
                titulo.Text = "Detalhes da marcação";
                titulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                titulo.ForeColor = ProfissionalSharedUi.Texto;
                titulo.Location = new Point(34, 26);
                titulo.Size = new Size(520, 42);
                f.Controls.Add(titulo);

                Label subtitulo = new Label();
                subtitulo.Text = "Veja os procedimentos e atualize o estado da marcação.";
                subtitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                subtitulo.ForeColor = ProfissionalSharedUi.Rosa;
                subtitulo.Location = new Point(36, 68);
                subtitulo.Size = new Size(560, 28);
                f.Controls.Add(subtitulo);

                Guna2Panel card = new Guna2Panel();
                card.Location = new Point(34, 118);
                card.Size = new Size(565, 300);
                card.BorderRadius = 22;
                card.FillColor = Color.FromArgb(255, 248, 251);
                f.Controls.Add(card);

                Label cliente = new Label();
                cliente.Text = string.IsNullOrWhiteSpace(m.Cliente) ? "Cliente" : m.Cliente;
                cliente.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                cliente.ForeColor = ProfissionalSharedUi.Rosa;
                cliente.Location = new Point(24, 22);
                cliente.Size = new Size(510, 38);
                card.Controls.Add(cliente);

                Label procedimentos = new Label();
                procedimentos.Text = "Procedimentos: " + (string.IsNullOrWhiteSpace(m.Servico) ? "Serviço" : m.Servico);
                procedimentos.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular);
                procedimentos.ForeColor = ProfissionalSharedUi.Texto;
                procedimentos.Location = new Point(24, 78);
                procedimentos.Size = new Size(515, 68);
                card.Controls.Add(procedimentos);

                Label dataHora = new Label();
                dataHora.Text = "Data/Hora: " + m.DataMarcacao.ToString("dd/MM/yyyy") + " às " + m.Hora.ToString(@"hh\:mm") + " - " + m.Hora.Add(TimeSpan.FromMinutes(m.DuracaoMinutos)).ToString(@"hh\:mm");
                dataHora.Font = new Font("Segoe UI", 10F);
                dataHora.ForeColor = ProfissionalSharedUi.Texto;
                dataHora.Location = new Point(24, 155);
                dataHora.Size = new Size(515, 26);
                card.Controls.Add(dataHora);

                Label valor = new Label();
                valor.Text = "Valor: " + ProfissionalRepository.FormatarMoeda(m.Valor);
                valor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                valor.ForeColor = ProfissionalSharedUi.Texto;
                valor.Location = new Point(24, 188);
                valor.Size = new Size(515, 26);
                card.Controls.Add(valor);

                Label estado = new Label();
                estado.Text = "Estado: " + NormalizarEstado(m.Estado);
                estado.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                estado.ForeColor = CorTextoEstado(m.Estado);
                estado.Location = new Point(24, 220);
                estado.Size = new Size(515, 26);
                card.Controls.Add(estado);

                Label obs = new Label();
                obs.Text = "Observações: " + (string.IsNullOrWhiteSpace(m.Observacoes) ? "Sem observações." : m.Observacoes);
                obs.Font = new Font("Segoe UI", 9.2F);
                obs.ForeColor = ProfissionalSharedUi.Cinza;
                obs.Location = new Point(24, 252);
                obs.Size = new Size(515, 36);
                card.Controls.Add(obs);

                Guna2Button confirmar = new Guna2Button();
                confirmar.Text = "Confirmar";
                confirmar.BorderRadius = 18;
                confirmar.FillColor = Color.FromArgb(39, 174, 96);
                confirmar.ForeColor = Color.White;
                confirmar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                confirmar.Location = new Point(34, 450);
                confirmar.Size = new Size(150, 42);
                confirmar.Click += delegate
                {
                    try
                    {
                        ProfissionalRepository.ConfirmarMarcacao(m.IdMarcacao, UsuarioLogado.Id);
                        m.Estado = "Confirmada";
                        estado.Text = "Estado: " + NormalizarEstado(m.Estado);
                        estado.ForeColor = CorTextoEstado(m.Estado);
                        CarregarAgenda();
                        MessageBox.Show("Marcação confirmada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show("Não foi possível confirmar: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                };
                f.Controls.Add(confirmar);

                Guna2Button cancelar = new Guna2Button();
                cancelar.Text = "Cancelar";
                cancelar.BorderRadius = 18;
                cancelar.FillColor = Color.FromArgb(231, 76, 60);
                cancelar.ForeColor = Color.White;
                cancelar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                cancelar.Location = new Point(204, 450);
                cancelar.Size = new Size(150, 42);
                cancelar.Click += delegate
                {
                    try
                    {
                        ProfissionalRepository.CancelarMarcacao(m.IdMarcacao, UsuarioLogado.Id);
                        m.Estado = "Cancelado";
                        estado.Text = "Estado: " + NormalizarEstado(m.Estado);
                        estado.ForeColor = CorTextoEstado(m.Estado);
                        CarregarAgenda();
                        MessageBox.Show("Marcação cancelada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show("Não foi possível cancelar: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                };
                f.Controls.Add(cancelar);

                Guna2Button fechar = new Guna2Button();
                fechar.Text = "Fechar";
                fechar.BorderRadius = 18;
                fechar.FillColor = Color.FromArgb(235, 235, 235);
                fechar.ForeColor = ProfissionalSharedUi.Texto;
                fechar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                fechar.Location = new Point(374, 450);
                fechar.Size = new Size(150, 42);
                fechar.Click += delegate { f.Close(); };
                f.Controls.Add(fechar);

                f.ShowDialog(this);
            }
        }

        private Color CorEstado(string estado, bool faixa)
        {
            estado = NormalizarEstado(estado).ToLowerInvariant();
            if (estado.Contains("cancel")) return faixa ? Color.FromArgb(220, 70, 110) : Color.FromArgb(255, 230, 240);
            if (estado.Contains("pend")) return faixa ? Color.FromArgb(255, 174, 66) : Color.FromArgb(255, 244, 220);
            if (estado.Contains("confirm") || estado.Contains("concl")) return faixa ? Color.FromArgb(39, 174, 96) : Color.FromArgb(224, 248, 232);
            return faixa ? ProfissionalSharedUi.Rosa : ProfissionalSharedUi.RosaClaro;
        }

        private Color CorTextoEstado(string estado)
        {
            estado = NormalizarEstado(estado).ToLowerInvariant();
            if (estado.Contains("pend")) return Color.FromArgb(170, 105, 0);
            if (estado.Contains("confirm") || estado.Contains("concl")) return Color.FromArgb(45, 130, 75);
            if (estado.Contains("cancel")) return Color.FromArgb(190, 45, 65);
            return ProfissionalSharedUi.Rosa;
        }

        private string NormalizarEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "Pendente";
            if (estado.Equals("Cacelado", StringComparison.OrdinalIgnoreCase) || estado.Equals("Cancelada", StringComparison.OrdinalIgnoreCase)) return "Cancelado";
            if (estado.Equals("Confirmada", StringComparison.OrdinalIgnoreCase)) return "Confirmado";
            return estado;
        }

        private void AbrirMenuMarcacao(Control anchor, MarcacaoInfo m)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Font = new Font("Segoe UI", 9F);

            ToolStripMenuItem confirmar = new ToolStripMenuItem("Confirmar marcação");
            confirmar.ForeColor = Color.FromArgb(45, 140, 85);
            confirmar.Enabled = !NormalizarEstado(m.Estado).ToLowerInvariant().Contains("concl");
            confirmar.Click += delegate
            {
                try
                {
                    ProfissionalRepository.ConfirmarMarcacao(m.IdMarcacao, UsuarioLogado.Id);
                    CarregarAgenda();
                    MessageBox.Show("Marcação confirmada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível confirmar: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            ToolStripMenuItem cancelar = new ToolStripMenuItem("Cancelar marcação");
            cancelar.ForeColor = ProfissionalSharedUi.Rosa;
            cancelar.Enabled = !NormalizarEstado(m.Estado).ToLowerInvariant().Contains("concl");
            cancelar.Click += delegate
            {
                DialogResult confirm = MessageBox.Show("Tem a certeza que pretende cancelar esta marcação?", "Cancelar marcação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;
                try
                {
                    ProfissionalRepository.CancelarMarcacao(m.IdMarcacao, UsuarioLogado.Id);
                    CarregarAgenda();
                    MessageBox.Show("Marcação cancelada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível cancelar: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            menu.Items.Add(confirmar);
            menu.Items.Add(cancelar);
            menu.Show(anchor, new Point(0, anchor.Height));
        }

        private void ProximoDia_Click(object sender, EventArgs e)
        {
            dataAtual = dataAtual.AddDays(1);
            guna2DateTimePicker1.Value = dataAtual;
            CarregarAgenda();
        }

        private void DiaAnterior_Click(object sender, EventArgs e)
        {
            dataAtual = dataAtual.AddDays(-1);
            guna2DateTimePicker1.Value = dataAtual;
            CarregarAgenda();
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            dataAtual = guna2DateTimePicker1.Value.Date;
            CarregarAgenda();
        }

        private void NovaMarcacao_Click(object sender, EventArgs e)
        {
            using (NovaMarcacaoProfissional form = new NovaMarcacaoProfissional(UsuarioLogado.Id, dataAtual))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    CarregarAgenda();
                    MessageBox.Show("Marcação criada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void TxtPesquisar_TextChanged(object sender, EventArgs e)
        {
            string termo = txtPesquisar.Text.Trim().ToLowerInvariant();
            if (agendaHost == null) return;
            foreach (Control c in agendaHost.Controls)
            {
                c.Visible = termo.Length == 0 || c.Text.ToLowerInvariant().Contains(termo) || ControlTemTexto(c, termo);
            }
        }

        private bool ControlTemTexto(Control control, string termo)
        {
            foreach (Control child in control.Controls)
            {
                if (!string.IsNullOrWhiteSpace(child.Text) && child.Text.ToLowerInvariant().Contains(termo)) return true;
                if (ControlTemTexto(child, termo)) return true;
            }
            return false;
        }

        private void guna2Panel22_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
