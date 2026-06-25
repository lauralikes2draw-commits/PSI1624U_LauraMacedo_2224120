using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    internal static class ProfissionalSharedUi
    {
        public static readonly Color Rosa = Color.FromArgb(255, 79, 135);
        public static readonly Color RosaEscuro = Color.FromArgb(240, 98, 146);
        public static readonly Color RosaClaro = Color.FromArgb(255, 230, 240);
        public static readonly Color Texto = Color.FromArgb(55, 45, 55);
        public static readonly Color Cinza = Color.FromArgb(125, 125, 125);

        public static void PrepararPagina(Form form, string paginaAtiva)
        {
            try
            {
                ProfissionalRepository.EnsureSchema();
                int id = ProfissionalRepository.ResolverIdProfissional();
                ProfissionalRepository.GarantirNotificacoesIniciais(id);
                AtualizarCabecalho(form, id);
                ConfigurarNavegacao(form, paginaAtiva);
                ConfigurarNotificacoes(form, id);
                AplicarTextosBase(form);
                AdminSharedUi.AjustarJanelaEQualidade(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar os dados da profissional: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static T Find<T>(Control parent, string name) where T : Control
        {
            Control[] controls = parent.Controls.Find(name, true);
            if (controls.Length == 0) return null;
            return controls[0] as T;
        }

        public static void AtualizarCabecalho(Form form, int idProfissional)
        {
            ProfissionalInfo p = ProfissionalRepository.GetProfissional(idProfissional);
            UsuarioLogado.Id = p.IdUsuario;
            UsuarioLogado.Nome = p.Nome;
            UsuarioLogado.Email = p.Email;
            UsuarioLogado.Tipo = "Profissional";
            UsuarioLogado.Foto = p.Foto;

            Label labelNome = Find<Label>(form, "label5");
            Label labelSub = Find<Label>(form, "label4");
            Label labelTipo = Find<Label>(form, "label3");
            Label labelOla = Find<Label>(form, "label1");
            Label labelBemVinda = Find<Label>(form, "label2");

            if (labelNome != null) labelNome.Text = string.IsNullOrWhiteSpace(p.Nome) ? "Profissional" : p.Nome;
            if (labelSub != null) labelSub.Text = string.IsNullOrWhiteSpace(p.Especialidade) ? p.Email : p.Especialidade;
            if (labelTipo != null) labelTipo.Text = "Profissional";
            if (labelOla != null) AdminSharedUi.ColorirPrimeiroNome(labelOla, ProfissionalRepository.PrimeiroNome(p.Nome));
            if (labelBemVinda != null) labelBemVinda.Text = "Bem-vinda de volta!";

            Guna2CirclePictureBox foto = Find<Guna2CirclePictureBox>(form, "guna2CirclePictureBox1");
            if (foto != null)
            {
                foto.Cursor = Cursors.Hand;
                foto.SizeMode = PictureBoxSizeMode.Zoom;
                foto.Image = CarregarImagemPerfil(p.Foto);
                foto.Click -= Foto_Click;
                foto.Click += Foto_Click;
            }
        }

        private static void Foto_Click(object sender, EventArgs e)
        {
            Guna2CirclePictureBox pic = sender as Guna2CirclePictureBox;
            if (pic == null) return;
            Form form = pic.FindForm();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Escolher foto da profissional";
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog(form) != DialogResult.OK) return;

                try
                {
                    string pasta = Path.Combine(Application.StartupPath, "FotosUtilizadores");
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                    string ext = Path.GetExtension(ofd.FileName);
                    string destino = Path.Combine(pasta, "profissional_" + UsuarioLogado.Id + ext);
                    File.Copy(ofd.FileName, destino, true);
                    ProfissionalRepository.AtualizarFotoProfissional(UsuarioLogado.Id, destino);
                    pic.Image = CarregarImagemPerfil(destino);
                    MessageBox.Show("Foto atualizada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível guardar a foto: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static Image CarregarImagemPerfil(string caminho)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(caminho) && File.Exists(caminho))
                {
                    using (Image img = Image.FromFile(caminho))
                        return new Bitmap(img);
                }
            }
            catch { }
            return Properties.Resources.icons8_profile_48;
        }

        public static Image CarregarImagemServico(ServicoInfo servico)
        {
            try
            {
                if (servico != null && !string.IsNullOrWhiteSpace(servico.Foto) && File.Exists(servico.Foto))
                {
                    using (Image img = Image.FromFile(servico.Foto))
                        return new Bitmap(img);
                }
            }
            catch { }

            if (servico != null && !string.IsNullOrWhiteSpace(servico.Categoria))
            {
                string cat = servico.Categoria.ToLowerInvariant();
                if (cat.Contains("rosto") || cat.Contains("pele")) return Properties.Resources.limpeza_de_pele;
                if (cat.Contains("unha")) return Properties.Resources.icons8_nail_polish_50;
                if (cat.Contains("corpo")) return Properties.Resources.facial;
            }
            return Properties.Resources.facial;
        }

        private static void ConfigurarNavegacao(Form form, string paginaAtiva)
        {
            Guna2Button btnDashboard = Find<Guna2Button>(form, "btnDashboard");
            Guna2Button btnMarcacoes = Find<Guna2Button>(form, "btnMarcacoes");
            Guna2Button btnFaturas = Find<Guna2Button>(form, "btnFaturas");
            Guna2Button btnSair = Find<Guna2Button>(form, "btnSair");

            ConfigurarBotaoMenu(btnDashboard, paginaAtiva == "dashboard");
            ConfigurarBotaoMenu(btnMarcacoes, paginaAtiva == "marcacoes");
            ConfigurarBotaoMenu(btnFaturas, paginaAtiva == "faturacao");

            if (btnDashboard != null)
            {
                btnDashboard.Text = "Dashboard";
                btnDashboard.Click -= BtnDashboard_Click;
                btnDashboard.Click += BtnDashboard_Click;
            }
            if (btnMarcacoes != null)
            {
                btnMarcacoes.Text = "Marcações";
                btnMarcacoes.Click -= BtnMarcacoes_Click;
                btnMarcacoes.Click += BtnMarcacoes_Click;
            }
            if (btnFaturas != null)
            {
                btnFaturas.Text = "Faturação";
                btnFaturas.Click -= BtnFaturas_Click;
                btnFaturas.Click += BtnFaturas_Click;
            }
            if (btnSair != null)
            {
                btnSair.Text = "Logout";
                btnSair.Click -= BtnSair_Click;
                btnSair.Click += BtnSair_Click;
            }
        }

        private static void ConfigurarBotaoMenu(Guna2Button btn, bool ativo)
        {
            if (btn == null) return;
            btn.Cursor = Cursors.Hand;
            btn.FillColor = ativo ? Rosa : Color.White;
            btn.ForeColor = ativo ? Color.White : Texto;
            btn.BorderRadius = 18;
        }

        private static void BtnDashboard_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is DashboardProfissional) return;
            DashboardProfissional f = new DashboardProfissional();
            f.Show();
            atual.Hide();
        }

        private static void BtnMarcacoes_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is MarcacoesProfissionais) return;
            MarcacoesProfissionais f = new MarcacoesProfissionais();
            f.Show();
            atual.Hide();
        }

        private static void BtnFaturas_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is FaturacaoProfissional) return;
            FaturacaoProfissional f = new FaturacaoProfissional();
            f.Show();
            atual.Hide();
        }

        private static void BtnSair_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            FormLogin login = new FormLogin();
            login.Show();
            atual.Hide();
        }

        private static void ConfigurarNotificacoes(Form form, int idProfissional)
        {
            Guna2CircleButton bell = Find<Guna2CircleButton>(form, "guna2CircleButton2");
            if (bell == null) return;
            bell.Cursor = Cursors.Hand;
            bell.Click -= Bell_Click;
            bell.Click += Bell_Click;
            AtualizarBadgeNotificacoes(form, idProfissional);
        }

        private static void Bell_Click(object sender, EventArgs e)
        {
            Control bell = sender as Control;
            if (bell == null) return;
            Form form = bell.FindForm();
            MostrarPainelNotificacoes(form, UsuarioLogado.Id, bell);
        }

        public static void AtualizarBadgeNotificacoes(Form form, int idProfissional)
        {
            Guna2CircleButton bell = Find<Guna2CircleButton>(form, "guna2CircleButton2");
            if (bell == null || bell.Parent == null) return;

            Label badge = Find<Label>(form, "lblBadgeNotificacoes");
            if (badge == null)
            {
                badge = new Label();
                badge.Name = "lblBadgeNotificacoes";
                badge.AutoSize = false;
                badge.TextAlign = ContentAlignment.MiddleCenter;
                badge.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
                badge.BackColor = Color.FromArgb(220, 20, 80);
                badge.ForeColor = Color.White;
                badge.Size = new Size(22, 22);
                badge.Cursor = Cursors.Hand;
                badge.Click += delegate { MostrarPainelNotificacoes(form, idProfissional, bell); };
                bell.Parent.Controls.Add(badge);
                badge.BringToFront();
            }

            int count = 0;
            try { count = ProfissionalRepository.GetNotificacoesNaoLidas(idProfissional); } catch { }
            badge.Text = count > 9 ? "9+" : count.ToString();
            badge.Visible = count > 0;
            badge.Location = new Point(bell.Left + bell.Width - 17, bell.Top - 1);
            badge.BringToFront();
        }

        public static void MostrarPainelNotificacoes(Form form, int idProfissional, Control bell)
        {
            Control existente = Find<Control>(form, "pnlNotificacoesProf");
            if (existente != null)
            {
                form.Controls.Remove(existente);
                existente.Dispose();
                return;
            }

            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Name = "pnlNotificacoesProf";
            painel.Size = new Size(390, 430);
            painel.Radius = 20;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 18;
            painel.Padding = new Padding(16);

            Point screen = bell.Parent.PointToScreen(new Point(bell.Left, bell.Bottom + 8));
            Point local = form.PointToClient(screen);
            painel.Location = new Point(Math.Max(10, Math.Min(local.X - 290, form.ClientSize.Width - painel.Width - 10)), Math.Max(70, Math.Min(local.Y, form.ClientSize.Height - painel.Height - 10)));

            Label titulo = new Label();
            titulo.Text = "Notificações";
            titulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titulo.ForeColor = Texto;
            titulo.Location = new Point(18, 18);
            titulo.AutoSize = true;
            painel.Controls.Add(titulo);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            fechar.FillColor = Color.WhiteSmoke;
            fechar.ForeColor = Texto;
            fechar.BorderRadius = 12;
            fechar.Size = new Size(38, 34);
            fechar.Location = new Point(330, 15);
            fechar.Click += delegate { form.Controls.Remove(painel); painel.Dispose(); };
            painel.Controls.Add(fechar);

            Guna2Button marcarTodas = new Guna2Button();
            marcarTodas.Text = "Marcar todas como lidas";
            marcarTodas.FillColor = RosaClaro;
            marcarTodas.ForeColor = Rosa;
            marcarTodas.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            marcarTodas.BorderRadius = 14;
            marcarTodas.Size = new Size(180, 32);
            marcarTodas.Location = new Point(18, 55);
            marcarTodas.Click += delegate
            {
                ProfissionalRepository.MarcarTodasNotificacoesComoLidas(idProfissional);
                form.Controls.Remove(painel);
                painel.Dispose();
                AtualizarBadgeNotificacoes(form, idProfissional);
                MostrarPainelNotificacoes(form, idProfissional, bell);
            };
            painel.Controls.Add(marcarTodas);

            FlowLayoutPanel lista = new FlowLayoutPanel();
            lista.Location = new Point(18, 98);
            lista.Size = new Size(350, 310);
            lista.AutoScroll = true;
            lista.FlowDirection = FlowDirection.TopDown;
            lista.WrapContents = false;
            painel.Controls.Add(lista);

            List<NotificacaoInfo> notificacoes = new List<NotificacaoInfo>();
            try { notificacoes = ProfissionalRepository.GetNotificacoes(idProfissional); } catch { }
            if (notificacoes.Count == 0)
            {
                Label vazio = new Label();
                vazio.Text = "Sem notificações no momento.";
                vazio.ForeColor = Cinza;
                vazio.Font = new Font("Segoe UI", 10F);
                vazio.Size = new Size(330, 40);
                lista.Controls.Add(vazio);
            }
            else
            {
                foreach (NotificacaoInfo n in notificacoes)
                    lista.Controls.Add(CriarCardNotificacao(n, form, painel, idProfissional, bell));
            }

            form.Controls.Add(painel);
            painel.BringToFront();
        }

        private static Control CriarCardNotificacao(NotificacaoInfo n, Form form, Control painel, int idProfissional, Control bell)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(325, n.Lida ? 76 : 102);
            card.BorderRadius = 15;
            card.FillColor = Color.FromArgb(255, 248, 251);
            card.Margin = new Padding(0, 0, 0, 10);

            Label msg = new Label();
            msg.Text = n.Mensagem;
            msg.Font = new Font("Segoe UI", 9.2F, n.Lida ? FontStyle.Regular : FontStyle.Bold);
            msg.ForeColor = Texto;
            msg.Location = new Point(12, 10);
            msg.Size = new Size(295, 38);
            card.Controls.Add(msg);

            Label data = new Label();
            data.Text = n.DataNotificacao.ToString("dd/MM/yyyy HH:mm");
            data.Font = new Font("Segoe UI", 8F);
            data.ForeColor = Cinza;
            data.Location = new Point(12, 50);
            data.Size = new Size(150, 22);
            card.Controls.Add(data);

            if (!n.Lida)
            {
                Guna2Button lida = new Guna2Button();
                lida.Text = "Marcar como lida";
                lida.FillColor = Rosa;
                lida.ForeColor = Color.White;
                lida.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                lida.BorderRadius = 12;
                lida.Size = new Size(135, 28);
                lida.Location = new Point(178, 64);
                lida.Click += delegate
                {
                    ProfissionalRepository.MarcarNotificacaoComoLida(n.Id);
                    form.Controls.Remove(painel);
                    painel.Dispose();
                    AtualizarBadgeNotificacoes(form, idProfissional);
                    MostrarPainelNotificacoes(form, idProfissional, bell);
                };
                card.Controls.Add(lida);
            }
            return card;
        }

        public static void ConfigurarGridRosa(DataGridView grid)
        {
            if (grid == null) return;
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Color.FromArgb(255, 210, 225);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Rosa;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Texto;
            grid.DefaultCellStyle.SelectionBackColor = RosaClaro;
            grid.DefaultCellStyle.SelectionForeColor = Texto;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 248, 251);
            grid.RowTemplate.Height = 58;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                DataGridViewImageColumn imgCol = col as DataGridViewImageColumn;
                if (imgCol != null)
                {
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    imgCol.MinimumWidth = 56;
                    if (imgCol.Width < 60) imgCol.Width = 60;
                }
            }
            grid.DataError -= Grid_DataError;
            grid.DataError += Grid_DataError;
        }

        private static void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        public static void AplicarTextosBase(Form form)
        {
            SetText(form, "btnFaturas", "Faturação");
            SetText(form, "btnMarcacoes", "Marcações");
            SetText(form, "label9", GetText(form, "label9").Replace("Marcacoes", "Marcações").Replace("Servicos", "Serviços"));
            var txtPesquisa = Find<Guna2TextBox>(form, "txtPesquisar");
            if (txtPesquisa != null) txtPesquisa.PlaceholderText = "Pesquisar clientes, marcações...";
        }

        private static string GetText(Form form, string name)
        {
            Control c = Find<Control>(form, name);
            return c == null ? "" : c.Text;
        }

        private static void SetText(Form form, string name, string text)
        {
            Control c = Find<Control>(form, name);
            if (c != null) c.Text = text;
        }
    }
}
