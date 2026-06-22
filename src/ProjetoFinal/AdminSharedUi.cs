using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    internal static class AdminSharedUi
    {
        public static readonly Color Rosa = Color.FromArgb(255, 79, 135);
        public static readonly Color RosaEscuro = Color.FromArgb(240, 98, 146);
        public static readonly Color RosaClaro = Color.FromArgb(255, 230, 240);
        public static readonly Color Texto = Color.FromArgb(55, 45, 55);
        public static readonly Color Cinza = Color.FromArgb(125, 125, 125);
        public static readonly Color Verde = Color.FromArgb(39, 174, 96);
        public static readonly Color Vermelho = Color.FromArgb(231, 76, 60);
        public static readonly Color Laranja = Color.FromArgb(243, 156, 18);

        public static void PrepararPagina(Form form, string paginaAtiva)
        {
            try
            {
                AdminRepository.EnsureSchema();
                int idAdmin = AdminRepository.ResolverIdAdmin();
                AtualizarCabecalho(form, idAdmin);
                ConfigurarNavegacao(form, paginaAtiva);
                ConfigurarPesquisaGlobal(form);
                ConfigurarNotificacoes(form);
                CorrigirTextos(form);
                ProtegerDataGridViews(form);
                AjustarJanelaEQualidade(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar os dados da administração: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static T Find<T>(Control parent, string name) where T : Control
        {
            if (parent == null) return null;
            Control[] controls = parent.Controls.Find(name, true);
            if (controls.Length == 0) return null;
            return controls[0] as T;
        }

        public static void AtualizarCabecalho(Form form, int idAdmin)
        {
            AdminInfo a = AdminRepository.GetAdmin(idAdmin);
            UsuarioLogado.Id = a.IdUsuario;
            UsuarioLogado.Nome = a.Nome;
            UsuarioLogado.Email = a.Email;
            UsuarioLogado.Tipo = "Admin";
            UsuarioLogado.Foto = a.Foto;

            Label labelNome = Find<Label>(form, "label5");
            Label labelSub = Find<Label>(form, "label4");
            Label labelTipo = Find<Label>(form, "label3");
            Label labelOla = Find<Label>(form, "label1");
            Label labelBem = Find<Label>(form, "label2");

            string nome = string.IsNullOrWhiteSpace(a.Nome) ? "Administrador" : a.Nome;
            if (labelNome != null) labelNome.Text = nome;
            if (labelSub != null) labelSub.Text = nome;
            if (labelTipo != null) labelTipo.Text = "Administrador";
            if (labelOla != null)
            {
                ColorirPrimeiroNome(labelOla, AdminRepository.PrimeiroNome(nome));
            }
            if (labelBem != null)
            {
                labelBem.Text = "Bem-vinda de volta!";
                labelBem.AutoSize = false;
                labelBem.Width = Math.Max(labelBem.Width, 310);
                if (labelOla != null && labelBem.Top <= labelOla.Top + 22) labelBem.Top = labelOla.Bottom + 2;
            }

            Guna2CirclePictureBox foto = Find<Guna2CirclePictureBox>(form, "guna2CirclePictureBox1");
            if (foto != null)
            {
                foto.Cursor = Cursors.Hand;
                foto.SizeMode = PictureBoxSizeMode.Zoom;
                foto.Image = CarregarImagemPerfil(a.Foto);
                foto.Click -= FotoAdmin_Click;
                foto.Click += FotoAdmin_Click;
            }
        }

        private static void FotoAdmin_Click(object sender, EventArgs e)
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
                    string destino = Path.Combine(pasta, "admin_" + UsuarioLogado.Id + ext);
                    File.Copy(ofd.FileName, destino, true);
                    AdminRepository.AtualizarFotoAdmin(UsuarioLogado.Id, destino);
                    pic.Image = CarregarImagemPerfil(destino);
                    MessageBox.Show("Foto atualizada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível guardar a foto: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static void ColorirPrimeiroNome(Label labelOla, string primeiroNome)
        {
            if (labelOla == null) return;
            string nome = string.IsNullOrWhiteSpace(primeiroNome) ? "nome" : primeiroNome.Trim().Split(' ')[0];
            Control parent = labelOla.Parent;
            if (parent == null) return;

            Font fonteBase = labelOla.Font;
            string overlayName = labelOla.Name + "_NomeRosaRuntime";

            // Remove possíveis duplicados criados em execuções anteriores no mesmo topbar.
            Control[] duplicados = parent.Controls.Find(overlayName, false);
            Label nomeRosa = duplicados.Length > 0 ? duplicados[0] as Label : null;
            for (int i = 1; i < duplicados.Length; i++)
            {
                parent.Controls.Remove(duplicados[i]);
                duplicados[i].Dispose();
            }

            labelOla.AutoSize = false;
            labelOla.Text = "Olá,";
            labelOla.ForeColor = Texto;
            labelOla.TextAlign = ContentAlignment.MiddleLeft;
            labelOla.BackColor = Color.White;
            labelOla.Width = Math.Max(58, TextRenderer.MeasureText("Olá,", fonteBase).Width + 10);
            labelOla.Height = Math.Max(34, labelOla.Height);
            labelOla.AutoEllipsis = false;

            if (nomeRosa == null)
            {
                nomeRosa = new Label();
                nomeRosa.Name = overlayName;
                nomeRosa.AutoSize = false;
                parent.Controls.Add(nomeRosa);
            }

            int nomeWidth = Math.Min(175, TextRenderer.MeasureText(nome, fonteBase).Width + 14);
            nomeRosa.Text = nome;
            nomeRosa.Font = fonteBase;
            nomeRosa.ForeColor = Rosa;
            nomeRosa.BackColor = Color.White;
            nomeRosa.TextAlign = ContentAlignment.MiddleLeft;
            nomeRosa.AutoEllipsis = true;
            nomeRosa.Size = new Size(nomeWidth, labelOla.Height);
            nomeRosa.Location = new Point(labelOla.Left + labelOla.Width + 2, labelOla.Top);

            // Evita que o nome por cima do topbar invada a caixa de pesquisa.
            Guna2TextBox pesquisa = Find<Guna2TextBox>(parent, "txtPesquisar");
            if (pesquisa == null) pesquisa = Find<Guna2TextBox>(labelOla.FindForm(), "txtPesquisar");
            if (pesquisa != null)
            {
                int limiteDireito = pesquisa.Parent == parent ? pesquisa.Left - 10 : parent.Width - 20;
                if (nomeRosa.Right > limiteDireito)
                    nomeRosa.Width = Math.Max(60, limiteDireito - nomeRosa.Left);
                pesquisa.BringToFront();
            }

            Label sombraNome = Find<Label>(parent, "label4");
            if (sombraNome != null && sombraNome != nomeRosa && sombraNome.Left < nomeRosa.Right + 40 && sombraNome.Top < labelOla.Bottom + 10)
            {
                sombraNome.Visible = false;
                sombraNome.Text = "";
            }

            Label labelBem = Find<Label>(parent, "label2");
            if (labelBem != null && labelBem.Top < labelOla.Bottom + 2)
            {
                labelBem.AutoSize = false;
                labelBem.Top = labelOla.Bottom + 2;
                labelBem.Width = Math.Max(labelBem.Width, 260);
                labelBem.BackColor = Color.White;
                labelBem.BringToFront();
            }

            labelOla.BringToFront();
            nomeRosa.BringToFront();
        }

        public static Image CarregarImagemPerfil(string caminho)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(caminho) && File.Exists(caminho))
                {
                    using (Image img = Image.FromFile(caminho)) return new Bitmap(img);
                }
            }
            catch { }
            return Properties.Resources.icons8_profile_48;
        }

        public static Image CarregarImagemServico(string caminho, string categoria)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(caminho) && File.Exists(caminho))
                {
                    using (Image img = Image.FromFile(caminho)) return new Bitmap(img);
                }
            }
            catch { }
            string cat = categoria == null ? "" : categoria.ToLowerInvariant();
            if (cat.Contains("unha")) return Properties.Resources.icons8_nail_polish_50;
            if (cat.Contains("rosto") || cat.Contains("pele")) return Properties.Resources.limpeza_de_pele;
            return Properties.Resources.facial;
        }

        public static void ConfigurarGrid(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Rosa;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = RosaClaro;
            dgv.DefaultCellStyle.SelectionForeColor = Texto;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 248, 251);
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 58;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                DataGridViewImageColumn imgCol = col as DataGridViewImageColumn;
                if (imgCol != null)
                {
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    imgCol.MinimumWidth = 56;
                    if (imgCol.Width < 60) imgCol.Width = 60;
                }
            }
            dgv.DataError -= Grid_DataError;
            dgv.DataError += Grid_DataError;
            AtivarDoubleBuffer(dgv);
        }

        private static void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        public static void ProtegerDataGridViews(Control parent)
        {
            if (parent == null) return;
            DataGridView dgv = parent as DataGridView;
            if (dgv != null)
            {
                dgv.DataError -= Grid_DataError;
                dgv.DataError += Grid_DataError;
                dgv.EditingControlShowing -= Dgv_EditingControlShowing;
                dgv.EditingControlShowing += Dgv_EditingControlShowing;
            }
            foreach (Control child in parent.Controls) ProtegerDataGridViews(child);
        }

        private static void Dgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox cb = e.Control as ComboBox;
            if (cb == null) return;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public static void PintarEstado(DataGridView dgv, string columnName)
        {
            if (dgv == null || !dgv.Columns.Contains(columnName)) return;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string estado = Convert.ToString(row.Cells[columnName].Value);
                DataGridViewCell cell = row.Cells[columnName];
                cell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                if (IsPagoOuAtivo(estado)) { cell.Style.ForeColor = Verde; cell.Style.BackColor = Color.FromArgb(229, 247, 239); }
                else if (estado != null && (estado.ToLowerInvariant().Contains("pend") || estado.ToLowerInvariant().Contains("aguard"))) { cell.Style.ForeColor = Laranja; cell.Style.BackColor = Color.FromArgb(255, 247, 225); }
                else if (estado != null && (estado.ToLowerInvariant().Contains("cancel") || estado.ToLowerInvariant().Contains("inativo") || estado.ToLowerInvariant().Contains("não") || estado.ToLowerInvariant().Contains("nao"))) { cell.Style.ForeColor = Vermelho; cell.Style.BackColor = Color.FromArgb(253, 235, 233); }
                else { cell.Style.ForeColor = Texto; cell.Style.BackColor = Color.White; }
            }
        }

        private static bool IsPagoOuAtivo(string estado)
        {
            if (estado == null) return false;
            string e = estado.ToLowerInvariant();
            return e == "ativo" || e.Contains("paga") || e.Contains("pago") || e.Contains("confirm") || e.Contains("conclu");
        }

        private static void ConfigurarNavegacao(Form form, string paginaAtiva)
        {
            ConfigurarBotao(form, "btnDashboard", "Dashboard", paginaAtiva == "dashboard", AbrirDashboard);
            ConfigurarBotao(form, "btnClientes", "Clientes", paginaAtiva == "clientes", AbrirClientes);
            ConfigurarBotao(form, "btnMarcacoes", "Marcações", paginaAtiva == "marcacoes", AbrirMarcacoes);
            ConfigurarBotao(form, "btnServicos", "Serviços", paginaAtiva == "servicos", AbrirServicos);
            ConfigurarBotao(form, "btnProfissionais", "Profissionais", paginaAtiva == "profissionais", AbrirProfissionais);
            ConfigurarBotao(form, "btnFaturas", "Faturação", paginaAtiva == "faturas", AbrirFaturas);
            ConfigurarBotao(form, "btnSair", "Logout", false, Logout);

            // Segundo sidebar existente apenas no dashboard antigo.
            if (form is FormAdminPrinc)
            {
                ConfigurarBotao(form, "guna2Button11", "Dashboard", paginaAtiva == "dashboard", AbrirDashboard);
                ConfigurarBotao(form, "guna2Button10", "Clientes", paginaAtiva == "clientes", AbrirClientes);
                ConfigurarBotao(form, "guna2Button9", "Marcações", paginaAtiva == "marcacoes", AbrirMarcacoes);
                ConfigurarBotao(form, "guna2Button8", "Serviços", paginaAtiva == "servicos", AbrirServicos);
                ConfigurarBotao(form, "guna2Button7", "Profissionais", paginaAtiva == "profissionais", AbrirProfissionais);
                ConfigurarBotao(form, "guna2Button6", "Faturação", paginaAtiva == "faturas", AbrirFaturas);
                ConfigurarBotao(form, "guna2Button5", "Logout", false, Logout);
            }

            ConfigurarBotoesMenuPorTexto(form, paginaAtiva);
        }

        private static void ConfigurarBotoesMenuPorTexto(Control parent, string paginaAtiva)
        {
            foreach (Control c in parent.Controls)
            {
                Guna2Button btn = c as Guna2Button;
                if (btn != null)
                {
                    string t = (btn.Text ?? "").Trim().ToLowerInvariant();
                    if (t == "dashboard") ConfigurarBotaoDireto(btn, "Dashboard", paginaAtiva == "dashboard", AbrirDashboard);
                    else if (t == "clientes") ConfigurarBotaoDireto(btn, "Clientes", paginaAtiva == "clientes", AbrirClientes);
                    else if (t == "marcacoes" || t == "marcações") ConfigurarBotaoDireto(btn, "Marcações", paginaAtiva == "marcacoes", AbrirMarcacoes);
                    else if (t == "servicos" || t == "serviços") ConfigurarBotaoDireto(btn, "Serviços", paginaAtiva == "servicos", AbrirServicos);
                    else if (t == "profissionais") ConfigurarBotaoDireto(btn, "Profissionais", paginaAtiva == "profissionais", AbrirProfissionais);
                    else if (t == "faturacao" || t == "faturação") ConfigurarBotaoDireto(btn, "Faturação", paginaAtiva == "faturas", AbrirFaturas);
                    else if (t == "logout") ConfigurarBotaoDireto(btn, "Logout", false, Logout);
                }
                ConfigurarBotoesMenuPorTexto(c, paginaAtiva);
            }
        }

        private static void ConfigurarBotao(Form form, string name, string text, bool ativo, EventHandler click)
        {
            Guna2Button btn = Find<Guna2Button>(form, name);
            if (btn == null) return;
            ConfigurarBotaoDireto(btn, text, ativo, click);
        }

        private static void ConfigurarBotaoDireto(Guna2Button btn, string text, bool ativo, EventHandler click)
        {
            if (btn == null) return;
            btn.Text = text;
            btn.Cursor = Cursors.Hand;
            btn.FillColor = ativo ? Rosa : Color.White;
            btn.ForeColor = ativo ? Color.White : Texto;
            btn.BorderRadius = 18;
            btn.Click -= click;
            btn.Click += click;
        }

        private static void AbrirDashboard(object sender, EventArgs e) { Navegar(sender, typeof(FormAdminPrinc)); }
        private static void AbrirClientes(object sender, EventArgs e) { Navegar(sender, typeof(ClientesAdmin)); }
        private static void AbrirMarcacoes(object sender, EventArgs e) { Navegar(sender, typeof(MarcacoesAdmin)); }
        private static void AbrirServicos(object sender, EventArgs e) { Navegar(sender, typeof(ServicosAdmin)); }
        private static void AbrirProfissionais(object sender, EventArgs e) { Navegar(sender, typeof(ProfissionaisAdmin)); }
        private static void AbrirFaturas(object sender, EventArgs e) { Navegar(sender, typeof(FaturasAdmin)); }

        private static void Navegar(object sender, Type tipo)
        {
            Control c = sender as Control;
            if (c == null) return;
            Form atual = c.FindForm();
            if (atual != null && atual.GetType() == tipo) return;
            Form novo = (Form)Activator.CreateInstance(tipo);
            novo.Show();
            if (atual != null) atual.Hide();
        }

        private static void Logout(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Form atual = c == null ? null : c.FindForm();
            FormLogin login = new FormLogin();
            login.Show();
            if (atual != null) atual.Hide();
        }

        public static void ConfigurarPesquisaGlobal(Form form)
        {
            Guna2TextBox txt = Find<Guna2TextBox>(form, "txtPesquisar");
            if (txt == null) return;
            txt.PlaceholderText = "Pesquisar clientes, profissionais, serviços, marcações e faturas...";
            txt.TextChanged -= Pesquisa_TextChanged;
            txt.TextChanged += Pesquisa_TextChanged;
        }

        private static void Pesquisa_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox txt = sender as Guna2TextBox;
            if (txt == null) return;
            Form form = txt.FindForm();
            MostrarResultadosPesquisa(form, txt, txt.Text.Trim());
        }

        private static void MostrarResultadosPesquisa(Form form, Control txt, string termo)
        {
            Control anterior = Find<Control>(form, "pnlPesquisaAdmin");
            if (anterior != null) { form.Controls.Remove(anterior); anterior.Dispose(); }
            if (termo.Length < 2) return;

            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Name = "pnlPesquisaAdmin";
            painel.Size = new Size(txt.Width, 330);
            painel.Radius = 18;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 14;
            Point local = form.PointToClient(txt.Parent.PointToScreen(new Point(txt.Left, txt.Bottom + 5)));
            painel.Location = local;
            painel.Padding = new Padding(12);
            painel.BringToFront();

            FlowLayoutPanel lista = new FlowLayoutPanel();
            lista.Dock = DockStyle.Fill;
            lista.AutoScroll = true;
            lista.FlowDirection = FlowDirection.TopDown;
            lista.WrapContents = false;
            painel.Controls.Add(lista);

            List<AdminSearchItem> resultados = new List<AdminSearchItem>();
            try { resultados = AdminRepository.PesquisarTudo(termo); } catch { }
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
                foreach (AdminSearchItem item in resultados)
                {
                    Guna2Panel card = new Guna2Panel();
                    card.Size = new Size(txt.Width - 35, 72);
                    card.BorderRadius = 14;
                    card.FillColor = Color.FromArgb(255, 248, 251);
                    card.Margin = new Padding(0, 0, 0, 8);
                    card.Cursor = Cursors.Hand;

                    Label tipo = new Label();
                    tipo.Text = item.Tipo;
                    tipo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                    tipo.ForeColor = Rosa;
                    tipo.Location = new Point(12, 8);
                    tipo.Size = new Size(card.Width - 24, 18);

                    Label titulo = new Label();
                    titulo.Text = item.Titulo;
                    titulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    titulo.ForeColor = Texto;
                    titulo.Location = new Point(12, 27);
                    titulo.Size = new Size(card.Width - 24, 20);

                    Label sub = new Label();
                    sub.Text = item.Subtitulo;
                    sub.Font = new Font("Segoe UI", 8.5F);
                    sub.ForeColor = Cinza;
                    sub.Location = new Point(12, 48);
                    sub.Size = new Size(card.Width - 24, 18);

                    card.Controls.Add(tipo); card.Controls.Add(titulo); card.Controls.Add(sub);
                    card.Click += delegate { AbrirPaginaPesquisa(form, item.Tipo); };
                    titulo.Click += delegate { AbrirPaginaPesquisa(form, item.Tipo); };
                    lista.Controls.Add(card);
                }
            }
            form.Controls.Add(painel);
            painel.BringToFront();
        }

        private static void AbrirPaginaPesquisa(Form atual, string tipo)
        {
            Form novo;
            string t = tipo == null ? "" : tipo.ToLowerInvariant();
            if (t.Contains("cliente")) novo = new ClientesAdmin();
            else if (t.Contains("prof")) novo = new ProfissionaisAdmin();
            else if (t.Contains("serv")) novo = new ServicosAdmin();
            else if (t.Contains("marc")) novo = new MarcacoesAdmin();
            else novo = new FaturasAdmin();
            novo.Show();
            if (atual != null) atual.Hide();
        }

        public static void ConfigurarNotificacoes(Form form)
        {
            Guna2CircleButton btn = Find<Guna2CircleButton>(form, "guna2CircleButton2");
            if (btn != null)
            {
                btn.Text = "";
                btn.FillColor = Color.WhiteSmoke;
                btn.ForeColor = Texto;
                btn.Cursor = Cursors.Hand;
                btn.Click -= Notificacoes_Click;
                btn.Click += Notificacoes_Click;
                AtualizarBadgeNotificacoes(form, btn);
            }

            Label verTodos = Find<Label>(form, "label35");
            if (verTodos != null && verTodos.Text.ToLowerInvariant().Contains("ver"))
            {
                verTodos.Cursor = Cursors.Hand;
                verTodos.Click -= Notificacoes_Click;
                verTodos.Click += Notificacoes_Click;
            }

            PreencherNotificacoesResumo(form);
        }

        private static void AtualizarBadgeNotificacoes(Form form, Control bell)
        {
            if (form == null || bell == null || bell.Parent == null) return;
            Label badge = Find<Label>(form, "badgeNotificacoesAdmin");
            if (badge == null)
            {
                badge = new Label();
                badge.Name = "badgeNotificacoesAdmin";
                badge.AutoSize = false;
                badge.TextAlign = ContentAlignment.MiddleCenter;
                badge.Font = new Font("Segoe UI", 7F, FontStyle.Bold);
                badge.BackColor = Color.FromArgb(220, 20, 80);
                badge.ForeColor = Color.White;
                badge.Size = new Size(22, 22);
                badge.Cursor = Cursors.Hand;
                badge.Click += delegate { MostrarPainelNotificacoes(form, bell); };
                bell.Parent.Controls.Add(badge);
            }

            int count = 0;
            try { count = AdminRepository.GetNotificacoesNaoLidasAdmin(); } catch { }
            badge.Text = count > 9 ? "9+" : count.ToString();
            badge.Visible = count > 0;
            badge.Location = new Point(bell.Left + bell.Width - 17, bell.Top - 1);
            badge.BringToFront();
        }

        private static void Notificacoes_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Form form = c == null ? null : c.FindForm();
            MostrarPainelNotificacoes(form, c);
        }

        public static void PreencherNotificacoesResumo(Form form)
        {
            try
            {
                List<NotificacaoInfo> ns = AdminRepository.GetNotificacoesAdmin();
                Label msg1 = Find<Label>(form, "label28");
                Label data1 = Find<Label>(form, "label30");
                Label msg2 = Find<Label>(form, "label31");
                Label data2 = Find<Label>(form, "label32");
                Control ponto1 = Find<Control>(form, "guna2CirclePictureBox15");
                Control ponto2 = Find<Control>(form, "guna2CirclePictureBox16");
                if (ponto1 != null) ponto1.Visible = false;
                if (ponto2 != null) ponto2.Visible = false;

                if (msg1 != null)
                {
                    if (ns.Count > 0)
                    {
                        msg1.Text = ns[0].Mensagem;
                        msg1.Tag = ns[0].Id;
                        msg1.Cursor = Cursors.Hand;
                        msg1.Font = new Font("Segoe UI Semibold", 9F, ns[0].Lida ? FontStyle.Regular : FontStyle.Bold);
                        msg1.ForeColor = Texto;
                        msg1.Click -= ResumoNotif_Click; msg1.Click += ResumoNotif_Click;
                    }
                    else msg1.Text = "Sem notificações";
                }
                if (data1 != null) data1.Text = ns.Count > 0 ? TempoRelativo(ns[0].DataNotificacao) : "";
                if (msg2 != null)
                {
                    if (ns.Count > 1)
                    {
                        msg2.Text = ns[1].Mensagem;
                        msg2.Tag = ns[1].Id;
                        msg2.Cursor = Cursors.Hand;
                        msg2.Font = new Font("Segoe UI Semibold", 9F, ns[1].Lida ? FontStyle.Regular : FontStyle.Bold);
                        msg2.ForeColor = Texto;
                        msg2.Click -= ResumoNotif_Click; msg2.Click += ResumoNotif_Click;
                    }
                    else msg2.Text = "";
                }
                if (data2 != null) data2.Text = ns.Count > 1 ? TempoRelativo(ns[1].DataNotificacao) : "";
            }
            catch { }
        }

        private static void ResumoNotif_Click(object sender, EventArgs e)
        {
            Label l = sender as Label;
            if (l == null || l.Tag == null) return;
            int id;
            if (int.TryParse(l.Tag.ToString(), out id)) AdminRepository.MarcarNotificacaoLida(id);
            Form f = l.FindForm();
            ConfigurarNotificacoes(f);
        }

        public static void MostrarPainelNotificacoes(Form owner)
        {
            MostrarPainelNotificacoes(owner, Find<Guna2CircleButton>(owner, "guna2CircleButton2"));
        }

        public static void MostrarPainelNotificacoes(Form owner, Control bell)
        {
            if (owner == null) return;
            Control existente = Find<Control>(owner, "pnlNotificacoesAdmin");
            if (existente != null)
            {
                owner.Controls.Remove(existente);
                existente.Dispose();
                return;
            }

            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Name = "pnlNotificacoesAdmin";
            painel.Size = new Size(390, 430);
            painel.Radius = 20;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 18;
            painel.Padding = new Padding(16);

            Point local;
            if (bell != null && bell.Parent != null)
                local = owner.PointToClient(bell.Parent.PointToScreen(new Point(bell.Left, bell.Bottom + 8)));
            else
                local = new Point(owner.Width - 430, 96);
            painel.Location = new Point(Math.Max(10, Math.Min(local.X - 290, owner.Width - 420)), Math.Max(70, local.Y));

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
            fechar.Click += delegate { owner.Controls.Remove(painel); painel.Dispose(); };
            painel.Controls.Add(fechar);

            Label marcarTodas = new Label();
            marcarTodas.Text = "Marcar todas como lidas";
            marcarTodas.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            marcarTodas.ForeColor = Rosa;
            marcarTodas.Location = new Point(18, 62);
            marcarTodas.Size = new Size(210, 26);
            marcarTodas.Cursor = Cursors.Hand;
            marcarTodas.Click += delegate
            {
                AdminRepository.MarcarTodasNotificacoesAdminLidas();
                owner.Controls.Remove(painel);
                painel.Dispose();
                ConfigurarNotificacoes(owner);
                MostrarPainelNotificacoes(owner, bell);
            };
            painel.Controls.Add(marcarTodas);

            FlowLayoutPanel lista = new FlowLayoutPanel();
            lista.Location = new Point(18, 98);
            lista.Size = new Size(350, 310);
            lista.AutoScroll = true;
            lista.FlowDirection = FlowDirection.TopDown;
            lista.WrapContents = false;
            lista.BackColor = Color.White;
            painel.Controls.Add(lista);

            List<NotificacaoInfo> notificacoes = new List<NotificacaoInfo>();
            try { notificacoes = AdminRepository.GetNotificacoesAdmin(); } catch { }
            if (notificacoes.Count == 0)
            {
                Guna2Panel vazio = new Guna2Panel();
                vazio.Size = new Size(325, 120);
                vazio.BorderRadius = 15;
                vazio.FillColor = Color.FromArgb(255, 248, 251);
                vazio.Controls.Add(MakeLabel("♡", 138, 14, 46, 38, 24, false, Rosa));
                Label l = MakeLabel("Sem notificações no momento.", 26, 66, 275, 26, 10, true, Texto);
                l.TextAlign = ContentAlignment.MiddleCenter;
                vazio.Controls.Add(l);
                lista.Controls.Add(vazio);
            }
            else
            {
                foreach (NotificacaoInfo n in notificacoes)
                    lista.Controls.Add(CriarCardNotificacaoAdmin(n, owner, painel, bell));
            }

            owner.Controls.Add(painel);
            painel.BringToFront();
        }

        private static Control CriarCardNotificacaoAdmin(NotificacaoInfo n, Form owner, Control painel, Control bell)
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
                    AdminRepository.MarcarNotificacaoLida(n.Id);
                    owner.Controls.Remove(painel);
                    painel.Dispose();
                    ConfigurarNotificacoes(owner);
                    MostrarPainelNotificacoes(owner, bell);
                };
                card.Controls.Add(lida);
            }
            return card;
        }

        public static void CorrigirTextos(Control parent)
        {
            if (parent == null) return;
            foreach (Control c in parent.Controls)
            {
                if (!string.IsNullOrEmpty(c.Text)) c.Text = CorrigirTexto(c.Text);
                CorrigirTextos(c);
            }
        }

        private static string CorrigirTexto(string text)
        {
            string t = text;
            t = t.Replace("Ola", "Olá");
            t = t.Replace("Marcacoes", "Marcações");
            t = t.Replace("marcacoes", "marcações");
            t = t.Replace("Faturacao", "Faturação");
            t = t.Replace("Faturemento", "Faturamento");
            t = t.Replace("Servicos", "Serviços");
            t = t.Replace("servicos", "serviços");
            t = t.Replace("Acoes", "Ações");
            t = t.Replace("Ultima", "Última");
            t = t.Replace("Preco", "Preço");
            t = t.Replace("Duracao", "Duração");
            t = t.Replace("Metodo", "Método");
            t = t.Replace("Pagar", "Pagamento");
            t = t.Replace("Avaliacao", "Avaliação");
            t = t.Replace("Confirmados", "Confirmadas");
            t = t.Replace("Aguardam confirmacao", "Aguardam confirmação");
            t = t.Replace("Profissional", "Profissional");
            t = Regex.Replace(t, @"(?<num>[\+\-]?\d[\d\.\,]*)\s*E\b", "${num} €");
            return t;
        }

        public static void AplicarPaginacaoSetas(Form form, string botaoAnterior, string botaoSeguinte)
        {
            if (form == null) return;
            foreach (Control c in TodosControles(form))
            {
                Guna2Button b = c as Guna2Button;
                if (b == null) continue;
                string t = (b.Text ?? "").Trim();
                bool paginacao = t == "<" || t == ">" || t == "‹" || t == "›" ||
                                 t == "«" || t == "»" || t == "1" || t == "2" || t == "3" ||
                                 t == "4" || t == "5" || t == "6" || t == "7" || t == "8" || t == "9";
                if (!paginacao) continue;
                b.Visible = false;
                b.Enabled = false;
            }
        }

        private static IEnumerable<Control> TodosControles(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (Control child in TodosControles(c)) yield return child;
            }
        }

        public static void AjustarJanelaEQualidade(Form form)
        {
            if (form == null) return;
            try
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.AutoScroll = true;
                Rectangle area = Screen.FromControl(form).WorkingArea;
                int originalW = form.Width;
                int originalH = form.Height;
                int w = Math.Min(originalW, Math.Max(980, area.Width - 24));
                int h = Math.Min(originalH, Math.Max(700, area.Height - 40));
                if (w < originalW || h < originalH)
                {
                    form.AutoScrollMinSize = new Size(originalW, originalH);
                    form.Size = new Size(w, h);
                }
                form.MinimumSize = new Size(Math.Min(w, 980), Math.Min(h, 650));
                form.Load -= CentralizarNoEcra_Load;
                form.Load += CentralizarNoEcra_Load;
                form.Shown -= CentralizarNoEcra_Load;
                form.Shown += CentralizarNoEcra_Load;
                AjustarTopbarPesquisa(form);
                AtivarDoubleBuffer(form);
            }
            catch { }
        }

        private static void CentralizarNoEcra_Load(object sender, EventArgs e)
        {
            Form f = sender as Form;
            if (f == null) return;
            try
            {
                Rectangle area = Screen.FromControl(f).WorkingArea;
                int x = area.Left + Math.Max(0, (area.Width - f.Width) / 2);
                int y = area.Top + Math.Max(0, (area.Height - f.Height) / 2);
                f.Location = new Point(x, y);
            }
            catch { }
        }

        public static void AjustarTopbarPesquisa(Control parent)
        {
            if (parent == null) return;
            try
            {
                Label ola = Find<Label>(parent, "label1");
                if (ola != null)
                {
                    ola.AutoSize = false;
                    ola.Width = Math.Min(Math.Max(ola.Width, 54), 72);
                    ola.Height = Math.Max(ola.Height, 30);
                }
                Label nomeRosa = null;
                if (ola != null)
                {
                    Control[] arr = ola.Parent == null ? new Control[0] : ola.Parent.Controls.Find(ola.Name + "_NomeRosaRuntime", false);
                    if (arr.Length > 0) nomeRosa = arr[0] as Label;
                }
                Guna2TextBox pesquisa = Find<Guna2TextBox>(parent, "txtPesquisar");
                if (pesquisa != null)
                {
                    if (nomeRosa != null && nomeRosa.Right >= pesquisa.Left)
                    {
                        nomeRosa.Width = Math.Max(50, pesquisa.Left - nomeRosa.Left - 8);
                    }
                    pesquisa.BringToFront();
                }
            }
            catch { }
        }

        private static void AtivarDoubleBuffer(Control c)
        {
            if (c == null) return;
            try
            {
                System.Reflection.PropertyInfo prop = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (prop != null) prop.SetValue(c, true, null);
            }
            catch { }
            foreach (Control child in c.Controls) AtivarDoubleBuffer(child);
        }

        public static string TempoRelativo(DateTime data)
        {
            TimeSpan diff = DateTime.Now - data;
            if (diff.TotalMinutes < 1) return "Agora";
            if (diff.TotalMinutes < 60) return "Há " + Math.Max(1, (int)diff.TotalMinutes) + " min";
            if (diff.TotalHours < 24) return "Há " + Math.Max(1, (int)diff.TotalHours) + " h";
            if (diff.TotalDays < 2) return "Ontem";
            return data.ToString("dd/MM/yyyy HH:mm");
        }

        public static void MostrarContacto(AdminCliente c)
        {
            if (c == null) return;
            using (Form f = new Form())
            {
                f.Text = "Contactar cliente";
                f.Size = new Size(500, 330);
                f.StartPosition = FormStartPosition.CenterParent;
                f.BackColor = Color.White;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;

                Label titulo = MakeLabel("Contactar cliente", 28, 24, 360, 32, 16, true, Texto);
                PictureBox pic = new PictureBox();
                pic.Image = CarregarImagemPerfil(c.Foto);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Location = new Point(28, 74);
                pic.Size = new Size(78, 78);

                Panel card = new Panel();
                card.Location = new Point(124, 72);
                card.Size = new Size(320, 130);
                card.BackColor = Color.FromArgb(255, 248, 251);
                card.Controls.Add(MakeLabel(c.Nome, 18, 14, 280, 25, 12, true, Texto));
                card.Controls.Add(MakeLabel("Email: " + c.Email, 18, 50, 280, 22, 9.5f, false, Texto));
                card.Controls.Add(MakeLabel("Telefone: " + c.Telefone, 18, 78, 280, 22, 9.5f, false, Texto));

                Button msg = new Button();
                msg.Text = "Enviar mensagem";
                msg.Location = new Point(124, 222);
                msg.Size = new Size(180, 42);
                msg.BackColor = Rosa;
                msg.ForeColor = Color.White;
                msg.FlatStyle = FlatStyle.Flat;
                msg.FlatAppearance.BorderSize = 0;
                msg.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                msg.Click += delegate { AdminDialogos.MostrarEnviarMensagem(f, new List<int> { c.IdUsuario }); };

                Button fechar = new Button();
                fechar.Text = "Fechar";
                fechar.Location = new Point(318, 222);
                fechar.Size = new Size(125, 42);
                fechar.BackColor = Color.FromArgb(240, 240, 240);
                fechar.ForeColor = Texto;
                fechar.FlatStyle = FlatStyle.Flat;
                fechar.FlatAppearance.BorderSize = 0;
                fechar.Click += delegate { f.Close(); };

                f.Controls.Add(titulo); f.Controls.Add(pic); f.Controls.Add(card); f.Controls.Add(msg); f.Controls.Add(fechar);
                f.ShowDialog();
            }
        }

        public static Label MakeLabel(string text, int x, int y, int w, int h, float size, bool bold, Color color)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
            l.ForeColor = color;
            return l;
        }
    }
}
