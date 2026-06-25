using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    internal static class ClienteSharedUi
    {
        public static readonly Color Rosa = ProfissionalSharedUi.Rosa;
        public static readonly Color RosaClaro = ProfissionalSharedUi.RosaClaro;
        public static readonly Color Texto = ProfissionalSharedUi.Texto;
        public static readonly Color Cinza = ProfissionalSharedUi.Cinza;

        public static void PrepararPagina(Form form, string paginaAtiva)
        {
            try
            {
                ClienteRepository.EnsureSchema();
                int id = ClienteRepository.ResolverIdCliente();
                ClienteRepository.GarantirNotificacoesIniciais(id);
                AtualizarCabecalho(form, id);
                ConfigurarNavegacao(form, paginaAtiva);
                ConfigurarNotificacoes(form, id);
                ConfigurarPesquisaGlobal(form, id);
                AplicarTextosBase(form);
                AdminSharedUi.AjustarJanelaEQualidade(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar os dados da cliente: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static T Find<T>(Control parent, string name) where T : Control
        {
            Control[] controls = parent.Controls.Find(name, true);
            if (controls.Length == 0) return null;
            return controls[0] as T;
        }

        public static void AtualizarCabecalho(Form form, int idCliente)
        {
            ClienteInfo c = ClienteRepository.GetCliente(idCliente);
            UsuarioLogado.Id = c.IdUsuario;
            UsuarioLogado.Nome = c.Nome;
            UsuarioLogado.Email = c.Email;
            UsuarioLogado.Tipo = "Cliente";
            UsuarioLogado.Foto = c.Foto;

            Label labelNome = Find<Label>(form, "label5");
            Label labelSub = Find<Label>(form, "label4");
            Label labelData = Find<Label>(form, "label3");
            Label labelOla = Find<Label>(form, "label1");
            Label labelBem = Find<Label>(form, "label2");

            if (labelNome != null) labelNome.Text = string.IsNullOrWhiteSpace(c.Nome) ? "Cliente" : c.Nome;
            if (labelSub != null) labelSub.Text = "nome";
            if (labelData != null) labelData.Text = "Cliente desde: " + c.DataCriacao.ToString("dd/MM/yyyy");
            if (labelOla != null) AdminSharedUi.ColorirPrimeiroNome(labelOla, ProfissionalRepository.PrimeiroNome(c.Nome));
            if (labelBem != null) labelBem.Text = "Bem-vinda de volta!";

            Guna2CirclePictureBox foto = Find<Guna2CirclePictureBox>(form, "guna2CirclePictureBox1");
            if (foto != null)
            {
                foto.Cursor = Cursors.Hand;
                foto.SizeMode = PictureBoxSizeMode.Zoom;
                foto.Image = ProfissionalSharedUi.CarregarImagemPerfil(c.Foto);
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
                ofd.Title = "Escolher foto de perfil";
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog(form) != DialogResult.OK) return;
                try
                {
                    string pasta = Path.Combine(Application.StartupPath, "FotosUtilizadores");
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                    string ext = Path.GetExtension(ofd.FileName);
                    string destino = Path.Combine(pasta, "cliente_" + UsuarioLogado.Id + ext);
                    File.Copy(ofd.FileName, destino, true);
                    ClienteRepository.AtualizarFotoCliente(UsuarioLogado.Id, destino);
                    pic.Image = ProfissionalSharedUi.CarregarImagemPerfil(destino);
                    MessageBox.Show("Foto atualizada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível guardar a foto: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static void ConfigurarNavegacao(Form form, string paginaAtiva)
        {
            Guna2Button btnDashboard = Find<Guna2Button>(form, "btnDashboard");
            Guna2Button btnMarcacoes = Find<Guna2Button>(form, "btnMarcacoes");
            Guna2Button btnFaturas = Find<Guna2Button>(form, "btnFaturas");
            Guna2Button btnAvaliacoes = Find<Guna2Button>(form, "btnClientes");
            Guna2Button btnServicos = Find<Guna2Button>(form, "btnServicos");
            Guna2Button btnProfissionais = Find<Guna2Button>(form, "btnProfissionais");
            Guna2Button btnSair = Find<Guna2Button>(form, "btnSair");

            ConfigurarBotaoMenu(btnDashboard, paginaAtiva == "dashboard");
            ConfigurarBotaoMenu(btnMarcacoes, paginaAtiva == "marcacoes");
            ConfigurarBotaoMenu(btnFaturas, paginaAtiva == "faturacao");
            ConfigurarBotaoMenu(btnAvaliacoes, paginaAtiva == "avaliacoes");
            ConfigurarBotaoMenu(btnServicos, false);
            ConfigurarBotaoMenu(btnProfissionais, false);

            if (btnDashboard != null) { btnDashboard.Text = "Dashboard"; btnDashboard.Click -= BtnDashboard_Click; btnDashboard.Click += BtnDashboard_Click; }
            if (btnMarcacoes != null) { btnMarcacoes.Text = "Marcações"; btnMarcacoes.Click -= BtnMarcacoes_Click; btnMarcacoes.Click += BtnMarcacoes_Click; }
            if (btnFaturas != null) { btnFaturas.Text = "Faturação"; btnFaturas.Click -= BtnFaturas_Click; btnFaturas.Click += BtnFaturas_Click; }
            if (btnAvaliacoes != null) { btnAvaliacoes.Text = "Avaliações"; btnAvaliacoes.Click -= BtnAvaliacoes_Click; btnAvaliacoes.Click += BtnAvaliacoes_Click; }
            if (btnServicos != null) { btnServicos.Text = "Serviços"; btnServicos.Click -= BtnServicos_Click; btnServicos.Click += BtnServicos_Click; }
            if (btnProfissionais != null) { btnProfissionais.Text = "Profissionais"; btnProfissionais.Click -= BtnMarcacoes_Click; btnProfissionais.Click += BtnMarcacoes_Click; }
            if (btnSair != null) { btnSair.Text = "Logout"; btnSair.Click -= BtnSair_Click; btnSair.Click += BtnSair_Click; }
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
            if (atual is FormClientePrinc) return;
            FormClientePrinc f = new FormClientePrinc();
            f.Show();
            atual.Hide();
        }

        private static void BtnMarcacoes_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is MinhasMarcacoes) return;
            MinhasMarcacoes f = new MinhasMarcacoes();
            f.Show();
            atual.Hide();
        }

        private static void BtnFaturas_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is FaturasClientes) return;
            FaturasClientes f = new FaturasClientes();
            f.Show();
            atual.Hide();
        }

        private static void BtnAvaliacoes_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            if (atual is AvaliacoesCliente) return;
            AvaliacoesCliente f = new AvaliacoesCliente();
            f.Show();
            atual.Hide();
        }

        private static void BtnServicos_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            MessageBox.Show("Escolha um serviço recomendado no dashboard ou clique em + Nova Marcação.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void BtnSair_Click(object sender, EventArgs e)
        {
            Form atual = ((Control)sender).FindForm();
            FormLogin login = new FormLogin();
            login.Show();
            atual.Hide();
        }

        public static void ConfigurarPesquisaGlobal(Form form, int idCliente)
        {
            Guna2TextBox txt = Find<Guna2TextBox>(form, "txtPesquisar");
            if (txt == null) return;
            txt.PlaceholderText = "Pesquisar marcações, serviços, faturas...";
            txt.TextChanged -= Pesquisa_TextChanged;
            txt.TextChanged += Pesquisa_TextChanged;
        }

        private static void Pesquisa_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox txt = sender as Guna2TextBox;
            if (txt == null) return;
            Form form = txt.FindForm();
            if (form == null) return;
            MostrarResultadosPesquisa(form, txt, UsuarioLogado.Id, txt.Text.Trim());
        }

        private static void MostrarResultadosPesquisa(Form form, Control txt, int idCliente, string termo)
        {
            Control anterior = Find<Control>(form, "pnlPesquisaCliente");
            if (anterior != null)
            {
                form.Controls.Remove(anterior);
                anterior.Dispose();
            }
            if (termo.Length < 2) return;

            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Name = "pnlPesquisaCliente";
            painel.Size = new Size(txt.Width, 300);
            painel.Radius = 18;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 14;
            Point local = form.PointToClient(txt.Parent.PointToScreen(new Point(txt.Left, txt.Bottom + 5)));
            painel.Location = local;
            painel.Padding = new Padding(12);

            FlowLayoutPanel lista = new FlowLayoutPanel();
            lista.Dock = DockStyle.Fill;
            lista.AutoScroll = true;
            lista.FlowDirection = FlowDirection.TopDown;
            lista.WrapContents = false;
            painel.Controls.Add(lista);

            List<SearchItem> resultados = new List<SearchItem>();
            try { resultados = ClienteRepository.PesquisarTudo(idCliente, termo); } catch { }
            if (resultados.Count == 0)
            {
                Label vazio = new Label();
                vazio.Text = "Nenhum resultado encontrado.";
                vazio.Font = new Font("Segoe UI", 10F);
                vazio.ForeColor = Cinza;
                vazio.Size = new Size(txt.Width - 35, 45);
                vazio.TextAlign = ContentAlignment.MiddleCenter;
                lista.Controls.Add(vazio);
            }
            else
            {
                foreach (SearchItem item in resultados)
                {
                    Guna2Panel card = new Guna2Panel();
                    card.Size = new Size(txt.Width - 35, 72);
                    card.BorderRadius = 14;
                    card.FillColor = Color.FromArgb(255, 248, 251);
                    card.Margin = new Padding(0, 0, 0, 8);

                    Label tipo = new Label();
                    tipo.Text = item.Tipo;
                    tipo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                    tipo.ForeColor = Rosa;
                    tipo.Location = new Point(12, 8);
                    tipo.Size = new Size(120, 20);
                    card.Controls.Add(tipo);

                    Label titulo = new Label();
                    titulo.Text = item.Titulo;
                    titulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    titulo.ForeColor = Texto;
                    titulo.Location = new Point(12, 28);
                    titulo.Size = new Size(txt.Width - 70, 22);
                    card.Controls.Add(titulo);

                    Label sub = new Label();
                    sub.Text = item.Subtitulo;
                    sub.Font = new Font("Segoe UI", 8F);
                    sub.ForeColor = Cinza;
                    sub.Location = new Point(12, 50);
                    sub.Size = new Size(txt.Width - 70, 20);
                    card.Controls.Add(sub);
                    lista.Controls.Add(card);
                }
            }

            form.Controls.Add(painel);
            painel.BringToFront();
        }

        private static void ConfigurarNotificacoes(Form form, int idCliente)
        {
            Guna2CircleButton bell = Find<Guna2CircleButton>(form, "guna2CircleButton2");
            if (bell == null) return;
            bell.Cursor = Cursors.Hand;
            bell.Click -= Bell_Click;
            bell.Click += Bell_Click;
            AtualizarBadgeNotificacoes(form, idCliente);
        }

        private static void Bell_Click(object sender, EventArgs e)
        {
            Control bell = sender as Control;
            if (bell == null) return;
            MostrarPainelNotificacoes(bell.FindForm(), UsuarioLogado.Id, bell);
        }

        public static void AtualizarBadgeNotificacoes(Form form, int idCliente)
        {
            Guna2CircleButton bell = Find<Guna2CircleButton>(form, "guna2CircleButton2");
            if (bell == null || bell.Parent == null) return;
            Label badge = Find<Label>(form, "lblBadgeNotificacoesCliente");
            if (badge == null)
            {
                badge = new Label();
                badge.Name = "lblBadgeNotificacoesCliente";
                badge.AutoSize = false;
                badge.TextAlign = ContentAlignment.MiddleCenter;
                badge.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
                badge.BackColor = Color.FromArgb(220, 20, 80);
                badge.ForeColor = Color.White;
                badge.Size = new Size(22, 22);
                badge.Cursor = Cursors.Hand;
                badge.Click += delegate { MostrarPainelNotificacoes(form, idCliente, bell); };
                bell.Parent.Controls.Add(badge);
            }
            int count = 0;
            try { count = ClienteRepository.GetNotificacoesNaoLidas(idCliente); } catch { }
            badge.Text = count > 9 ? "9+" : count.ToString();
            badge.Visible = count > 0;
            badge.Location = new Point(bell.Left + bell.Width - 17, bell.Top - 1);
            badge.BringToFront();
        }

        public static void MostrarPainelNotificacoes(Form form, int idCliente, Control bell)
        {
            if (form == null) return;
            Control existente = Find<Control>(form, "pnlNotificacoesCliente");
            if (existente != null)
            {
                form.Controls.Remove(existente);
                existente.Dispose();
                return;
            }

            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Name = "pnlNotificacoesCliente";
            painel.Size = new Size(420, 470);
            painel.Radius = 22;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 20;
            painel.Padding = new Padding(18);
            painel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            int x = Math.Max(12, form.ClientSize.Width - painel.Width - 26);
            int y = 86;
            if (bell != null && bell.Parent != null)
            {
                Point local = form.PointToClient(bell.Parent.PointToScreen(new Point(bell.Right, bell.Bottom + 8)));
                x = Math.Max(12, Math.Min(local.X - painel.Width, form.ClientSize.Width - painel.Width - 18));
                y = Math.Max(70, Math.Min(local.Y, form.ClientSize.Height - painel.Height - 12));
            }
            painel.Location = new Point(x, y);

            Label titulo = new Label();
            titulo.Text = "Notificações";
            titulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titulo.ForeColor = Texto;
            titulo.BackColor = Color.White;
            titulo.Location = new Point(22, 18);
            titulo.Size = new Size(270, 34);
            painel.Controls.Add(titulo);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            fechar.FillColor = Color.FromArgb(248, 248, 248);
            fechar.ForeColor = Texto;
            fechar.BorderRadius = 13;
            fechar.Size = new Size(38, 34);
            fechar.Location = new Point(360, 16);
            fechar.Click += delegate { form.Controls.Remove(painel); painel.Dispose(); };
            painel.Controls.Add(fechar);

            Guna2Button marcarTodas = new Guna2Button();
            marcarTodas.Text = "Marcar todas como lidas";
            marcarTodas.FillColor = RosaClaro;
            marcarTodas.ForeColor = Rosa;
            marcarTodas.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            marcarTodas.BorderRadius = 15;
            marcarTodas.Size = new Size(198, 34);
            marcarTodas.Location = new Point(22, 58);
            marcarTodas.Click += delegate
            {
                ClienteRepository.MarcarTodasNotificacoesComoLidas(idCliente);
                form.Controls.Remove(painel);
                painel.Dispose();
                AtualizarBadgeNotificacoes(form, idCliente);
                MostrarPainelNotificacoes(form, idCliente, bell);
            };
            painel.Controls.Add(marcarTodas);

            FlowLayoutPanel lista = new FlowLayoutPanel();
            lista.Location = new Point(22, 106);
            lista.Size = new Size(376, 340);
            lista.BackColor = Color.White;
            lista.AutoScroll = true;
            lista.FlowDirection = FlowDirection.TopDown;
            lista.WrapContents = false;
            lista.Padding = new Padding(0, 0, 8, 0);
            painel.Controls.Add(lista);

            List<NotificacaoInfo> notificacoes = new List<NotificacaoInfo>();
            try { notificacoes = ClienteRepository.GetNotificacoes(idCliente); } catch { }
            if (notificacoes.Count == 0)
            {
                Label vazio = new Label();
                vazio.Text = "Sem notificações no momento.";
                vazio.ForeColor = Cinza;
                vazio.Font = new Font("Segoe UI", 10F);
                vazio.BackColor = Color.White;
                vazio.Size = new Size(350, 42);
                lista.Controls.Add(vazio);
            }
            else
            {
                foreach (NotificacaoInfo n in notificacoes)
                    lista.Controls.Add(CriarCardNotificacao(n, form, painel, idCliente, bell));
            }

            form.Controls.Add(painel);
            painel.BringToFront();
        }

        private static Control CriarCardNotificacao(NotificacaoInfo n, Form form, Control painel, int idCliente, Control bell)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(348, n.Lida ? 88 : 118);
            card.BorderRadius = 17;
            card.FillColor = Color.FromArgb(255, 248, 251);
            card.Margin = new Padding(0, 0, 0, 12);

            Label msg = new Label();
            msg.Text = n.Mensagem;
            msg.Font = new Font("Segoe UI", 9.2F, n.Lida ? FontStyle.Regular : FontStyle.Bold);
            msg.ForeColor = Texto;
            msg.BackColor = card.FillColor;
            msg.Location = new Point(14, 10);
            msg.Size = new Size(318, 48);
            card.Controls.Add(msg);

            Label data = new Label();
            data.Text = n.DataNotificacao.ToString("dd/MM/yyyy HH:mm");
            data.Font = new Font("Segoe UI", 8F);
            data.ForeColor = Cinza;
            data.BackColor = card.FillColor;
            data.Location = new Point(14, 60);
            data.Size = new Size(160, 22);
            card.Controls.Add(data);

            if (!n.Lida)
            {
                Guna2Button lida = new Guna2Button();
                lida.Text = "Marcar como lida";
                lida.FillColor = Rosa;
                lida.ForeColor = Color.White;
                lida.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                lida.BorderRadius = 13;
                lida.Size = new Size(145, 30);
                lida.Location = new Point(188, 78);
                lida.Click += delegate
                {
                    ClienteRepository.MarcarNotificacaoComoLida(n.Id);
                    form.Controls.Remove(painel);
                    painel.Dispose();
                    AtualizarBadgeNotificacoes(form, idCliente);
                    MostrarPainelNotificacoes(form, idCliente, bell);
                };
                card.Controls.Add(lida);
            }
            return card;
        }

        public static void ConfigurarGridRosa(DataGridView grid)
        {
            ProfissionalSharedUi.ConfigurarGridRosa(grid);
            if (grid != null)
            {
                grid.RowTemplate.Height = 56;
                grid.CellFormatting -= Grid_CellFormatting;
                grid.CellFormatting += Grid_CellFormatting;
            }
        }

        private static void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (grid.Columns[e.ColumnIndex].Name.ToLower().Contains("estado"))
            {
                string estado = Convert.ToString(e.Value);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                e.CellStyle.BackColor = CorEstado(estado);
                e.CellStyle.SelectionBackColor = CorEstado(estado);
            }
        }

        public static Color CorEstado(string estado)
        {
            string e = (estado ?? "").ToLowerInvariant();
            if (e.Contains("confirm")) return Color.FromArgb(64, 168, 92);
            if (e.Contains("paga") || e.Contains("pago") || e.Contains("concl")) return Color.FromArgb(46, 160, 91);
            if (e.Contains("pend") || e.Contains("aguard")) return Color.FromArgb(232, 177, 44);
            if (e.Contains("cancel")) return Color.FromArgb(220, 78, 92);
            return Rosa;
        }

        public static void AplicarTextosBase(Form form)
        {
            SetText(form, "btnFaturas", "Faturação");
            SetText(form, "btnMarcacoes", "Marcações");
            SetText(form, "btnClientes", "Avaliações");
            SetText(form, "btnServicos", "Serviços");
            SetText(form, "label1", GetText(form, "label1").Replace("Ola", "Olá"));
            CorrigirTodosTextos(form);
        }

        private static void CorrigirTodosTextos(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!string.IsNullOrEmpty(c.Text))
                {
                    c.Text = c.Text.Replace("Marcacoes", "Marcações")
                                   .Replace("Marcacao", "Marcação")
                                   .Replace("Proxima", "Próxima")
                                   .Replace("Proximas", "Próximas")
                                   .Replace("Servicos", "Serviços")
                                   .Replace("Servico", "Serviço")
                                   .Replace("Promocoes", "Promoções")
                                   .Replace("Promocao", "Promoção")
                                   .Replace("Avaliacoes", "Avaliações")
                                   .Replace("Avaliacao", "Avaliação")
                                   .Replace("Voce", "Você")
                                   .Replace("voce", "você")
                                   .Replace("Faturacao", "Faturação")
                                   .Replace("Media", "Média")
                                   .Replace("Disponiveis", "Disponíveis")
                                   .Replace("Remocao", "Remoção")
                                   .Replace("renovacao", "renovação")
                                   .Replace("organizacao", "organização")
                                   .Replace("espaco", "espaço")
                                   .Replace("anoniam", "anónima");
                    c.Text = Regex.Replace(c.Text, @"(?<=\d)\s*E\b", " €");
                }
                if (c.HasChildren) CorrigirTodosTextos(c);
            }
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
