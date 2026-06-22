using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public class NovaMarcacaoAdmin : Form
    {
        private int passo = 1;
        private bool paraMimAdmin = false;
        private AdminOpcao clienteSelecionada;
        private AdminProfissional profissionalSelecionada;
        private List<AdminServico> servicos;
        private readonly List<AdminServico> servicosSelecionados = new List<AdminServico>();
        private string categoriaAtual = "Todos";

        private Guna2ShadowPanel container;
        private Panel conteudo;
        private Label lblPasso;
        private Guna2Button btnAnterior;
        private Guna2Button btnProximo;
        private Guna2DateTimePicker dtData;
        private Guna2ComboBox cbHora;
        private Guna2ComboBox cbCliente;
        private Guna2ComboBox cbPagamento;
        private Label lblResumoSelecionados;
        private DateTime dataEscolhida = DateTime.Today;
        private string horaEscolhida = "09:00";

        public NovaMarcacaoAdmin()
        {
            servicos = AdminRepository.GetServicos("", "Ativo", 5000);
            InicializarJanela();
            RenderizarPasso();
        }

        private void InicializarJanela()
        {
            Text = "Nova Marcação";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Size = new Size(960, 725);
            AutoScaleMode = AutoScaleMode.None;

            container = new Guna2ShadowPanel();
            container.Dock = DockStyle.Fill;
            container.Radius = 26;
            container.FillColor = Color.White;
            container.ShadowColor = Color.Gray;
            container.ShadowDepth = 20;
            Controls.Add(container);

            Label titulo = CriarLabel("+ Nova Marcação", 36, 25, 390, 42, 19F, FontStyle.Bold, AdminSharedUi.Texto);
            container.Controls.Add(titulo);

            lblPasso = CriarLabel("", 38, 72, 760, 28, 9.5F, FontStyle.Bold, AdminSharedUi.Rosa);
            container.Controls.Add(lblPasso);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            fechar.FillColor = Color.WhiteSmoke;
            fechar.ForeColor = AdminSharedUi.Texto;
            fechar.BorderRadius = 16;
            fechar.Size = new Size(45, 40);
            fechar.Location = new Point(870, 25);
            fechar.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            container.Controls.Add(fechar);

            conteudo = new Panel();
            conteudo.Location = new Point(36, 112);
            conteudo.Size = new Size(880, 515);
            conteudo.AutoScroll = true;
            conteudo.BackColor = Color.White;
            container.Controls.Add(conteudo);

            btnAnterior = new Guna2Button();
            btnAnterior.Text = "Anterior";
            btnAnterior.BorderRadius = 20;
            btnAnterior.FillColor = Color.WhiteSmoke;
            btnAnterior.ForeColor = AdminSharedUi.Texto;
            btnAnterior.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAnterior.Size = new Size(140, 44);
            btnAnterior.Location = new Point(600, 650);
            btnAnterior.Click += delegate { if (passo > 1) { passo--; RenderizarPasso(); } };
            container.Controls.Add(btnAnterior);

            btnProximo = new Guna2Button();
            btnProximo.Text = "Próximo";
            btnProximo.BorderRadius = 20;
            btnProximo.FillColor = AdminSharedUi.Rosa;
            btnProximo.ForeColor = Color.White;
            btnProximo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnProximo.Size = new Size(165, 44);
            btnProximo.Location = new Point(752, 650);
            btnProximo.Click += BtnProximo_Click;
            container.Controls.Add(btnProximo);
        }

        private void RenderizarPasso()
        {
            conteudo.Controls.Clear();
            btnAnterior.Visible = passo > 1;
            btnProximo.Text = passo == 4 ? "Confirmar marcação" : "Próximo";

            if (passo == 1)
            {
                lblPasso.Text = "Passo 1 de 4 · Para quem é a marcação e quando será";
                RenderizarPassoClienteData();
            }
            else if (passo == 2)
            {
                lblPasso.Text = "Passo 2 de 4 · Escolha os procedimentos";
                RenderizarPassoServicos();
            }
            else if (passo == 3)
            {
                lblPasso.Text = "Passo 3 de 4 · Escolha a profissional";
                RenderizarPassoProfissional();
            }
            else
            {
                lblPasso.Text = "Passo 4 de 4 · Pré-fatura e pagamento";
                RenderizarPassoFatura();
            }
        }

        private void RenderizarPassoClienteData()
        {
            conteudo.Controls.Add(CriarLabel("Escolha se a marcação é para uma cliente ou para a própria administradora.", 0, 0, 820, 30, 11F, FontStyle.Regular, AdminSharedUi.Cinza));

            Guna2Button btnCliente = CriarBotaoEscolha("Para cliente", 0, 50, !paraMimAdmin);
            Guna2Button btnAdmin = CriarBotaoEscolha("Para mim", 220, 50, paraMimAdmin);
            btnCliente.Click += delegate { paraMimAdmin = false; RenderizarPasso(); };
            btnAdmin.Click += delegate { paraMimAdmin = true; RenderizarPasso(); };
            conteudo.Controls.Add(btnCliente);
            conteudo.Controls.Add(btnAdmin);

            if (!paraMimAdmin)
            {
                conteudo.Controls.Add(CriarLabel("Cliente", 0, 128, 320, 24, 10F, FontStyle.Bold, AdminSharedUi.Texto));
                cbCliente = CriarCombo(0, 158, 500);
                foreach (AdminOpcao c in AdminRepository.GetClientesOpcoes()) cbCliente.Items.Add(c);
                if (clienteSelecionada != null) SelecionarCombo(cbCliente, clienteSelecionada.Id);
                else if (cbCliente.Items.Count > 0) cbCliente.SelectedIndex = 0;
                conteudo.Controls.Add(cbCliente);
            }
            else
            {
                clienteSelecionada = null;
                AdminInfo admin = AdminRepository.GetAdmin(AdminRepository.ResolverIdAdmin());
                Guna2Panel aviso = CriarMiniCard(0, 130, 660, 70);
                aviso.Controls.Add(CriarLabel("A marcação ficará registada para a própria administração.", 18, 10, 610, 24, 10F, FontStyle.Bold, AdminSharedUi.Texto));
                aviso.Controls.Add(CriarLabel(string.IsNullOrWhiteSpace(admin.Nome) ? "Administradora" : admin.Nome, 18, 36, 610, 24, 10F, FontStyle.Regular, AdminSharedUi.Cinza));
                conteudo.Controls.Add(aviso);
            }

            conteudo.Controls.Add(CriarLabel("Data", 0, 250, 120, 24, 10F, FontStyle.Bold, AdminSharedUi.Texto));
            dtData = new Guna2DateTimePicker();
            dtData.BorderRadius = 18;
            dtData.FillColor = AdminSharedUi.Rosa;
            dtData.ForeColor = Color.White;
            dtData.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dtData.Format = DateTimePickerFormat.Long;
            dtData.Size = new Size(300, 46);
            dtData.Location = new Point(0, 280);
            dtData.Value = dataEscolhida;
            conteudo.Controls.Add(dtData);

            conteudo.Controls.Add(CriarLabel("Hora", 350, 250, 120, 24, 10F, FontStyle.Bold, AdminSharedUi.Texto));
            cbHora = CriarCombo(350, 280, 175);
            for (TimeSpan h = new TimeSpan(9, 0, 0); h <= new TimeSpan(19, 0, 0); h = h.Add(TimeSpan.FromMinutes(30)))
                cbHora.Items.Add(h.ToString(@"hh\:mm"));
            int idx = cbHora.Items.IndexOf(horaEscolhida);
            cbHora.SelectedIndex = idx >= 0 ? idx : 0;
            conteudo.Controls.Add(cbHora);
        }

        private void RenderizarPassoServicos()
        {
            conteudo.Controls.Add(CriarLabel("Adicione um ou mais serviços. Use as categorias para encontrar rapidamente o que precisa.", 0, 0, 830, 28, 11F, FontStyle.Regular, AdminSharedUi.Cinza));
            lblResumoSelecionados = CriarLabel(ResumoServicosSelecionados(), 0, 32, 830, 28, 10F, FontStyle.Bold, AdminSharedUi.Rosa);
            conteudo.Controls.Add(lblResumoSelecionados);

            FlowLayoutPanel categorias = new FlowLayoutPanel();
            categorias.Location = new Point(0, 70);
            categorias.Size = new Size(840, 58);
            categorias.WrapContents = false;
            categorias.AutoScroll = true;
            categorias.BackColor = Color.White;
            conteudo.Controls.Add(categorias);

            List<string> cats = new List<string>();
            cats.Add("Todos");
            foreach (AdminServico s in servicos)
            {
                string c = string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria.Trim();
                if (!cats.Any(x => string.Equals(x, c, StringComparison.OrdinalIgnoreCase))) cats.Add(c);
            }
            foreach (string cat in cats)
            {
                Guna2Button chip = CriarChipCategoria(cat, string.Equals(cat, categoriaAtual, StringComparison.OrdinalIgnoreCase));
                chip.Click += delegate { categoriaAtual = cat; RenderizarPasso(); };
                categorias.Controls.Add(chip);
            }

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 138);
            flow.Size = new Size(850, 355);
            flow.AutoScroll = true;
            flow.WrapContents = true;
            flow.BackColor = Color.White;
            conteudo.Controls.Add(flow);

            foreach (AdminServico s in servicos)
            {
                string cat = string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria.Trim();
                if (!categoriaAtual.Equals("Todos", StringComparison.OrdinalIgnoreCase) && !cat.Equals(categoriaAtual, StringComparison.OrdinalIgnoreCase)) continue;
                flow.Controls.Add(CriarCardServico(s));
            }
        }

        private void RenderizarPassoProfissional()
        {
            conteudo.Controls.Add(CriarLabel("Escolha a profissional que realizará o atendimento. Profissionais ocupadas neste horário não aparecem aqui.", 0, 0, 830, 38, 11F, FontStyle.Regular, AdminSharedUi.Cinza));

            FlowLayoutPanel profissionais = new FlowLayoutPanel();
            profissionais.Location = new Point(0, 52);
            profissionais.Size = new Size(850, 420);
            profissionais.AutoScroll = true;
            profissionais.WrapContents = true;
            profissionais.BackColor = Color.White;
            conteudo.Controls.Add(profissionais);

            TimeSpan hora = TimeSpan.Parse(horaEscolhida);
            int duracao = Math.Max(30, servicosSelecionados.Sum(s => s.DuracaoMinutos));
            List<AdminProfissional> disponiveis = AdminRepository.GetProfissionaisDisponiveis(dataEscolhida, hora, duracao);
            if (profissionalSelecionada != null && !disponiveis.Any(p => p.IdUsuario == profissionalSelecionada.IdUsuario)) profissionalSelecionada = null;

            if (disponiveis.Count == 0)
            {
                Guna2Panel vazio = CriarMiniCard(0, 0, 820, 130);
                vazio.Controls.Add(CriarLabel("Nenhuma profissional disponível neste horário.", 26, 26, 760, 30, 13F, FontStyle.Bold, AdminSharedUi.Texto));
                vazio.Controls.Add(CriarLabel("Volte ao passo anterior e escolha outro dia ou outra hora.", 26, 62, 760, 26, 10F, FontStyle.Regular, AdminSharedUi.Cinza));
                profissionais.Controls.Add(vazio);
                return;
            }

            foreach (AdminProfissional p in disponiveis) profissionais.Controls.Add(CriarCardProfissional(p));
        }

        private void RenderizarPassoFatura()
        {
            Guna2Panel fatura = new Guna2Panel();
            fatura.Size = new Size(815, 330);
            fatura.Location = new Point(0, 0);
            fatura.BorderRadius = 20;
            fatura.FillColor = Color.White;
            fatura.BorderColor = Color.FromArgb(255, 210, 225);
            fatura.BorderThickness = 1;
            conteudo.Controls.Add(fatura);

            fatura.Controls.Add(CriarLabel("Pré-fatura", 22, 18, 300, 30, 14F, FontStyle.Bold, AdminSharedUi.Texto));
            string clienteTexto;
            if (paraMimAdmin)
            {
                AdminInfo admin = AdminRepository.GetAdmin(AdminRepository.ResolverIdAdmin());
                clienteTexto = (string.IsNullOrWhiteSpace(admin.Nome) ? "Administradora" : admin.Nome) + " (admin)";
            }
            else clienteTexto = clienteSelecionada == null ? "Cliente" : clienteSelecionada.Nome;

            fatura.Controls.Add(CriarLabel("Cliente: " + clienteTexto, 22, 54, 520, 24, 9.5F, FontStyle.Bold, AdminSharedUi.Texto));
            fatura.Controls.Add(CriarLabel("Profissional: " + (profissionalSelecionada == null ? "Não selecionada" : profissionalSelecionada.Nome), 22, 80, 520, 24, 9.5F, FontStyle.Regular, AdminSharedUi.Cinza));
            fatura.Controls.Add(CriarLabel("Data: " + dataEscolhida.ToString("dd/MM/yyyy") + " · Hora: " + horaEscolhida, 22, 106, 520, 24, 9.5F, FontStyle.Regular, AdminSharedUi.Cinza));

            int y = 138;
            foreach (AdminServico s in servicosSelecionados.Take(5))
            {
                fatura.Controls.Add(CriarLabel(s.Nome + " · " + s.DuracaoMinutos + " min", 22, y, 480, 24, 9F, FontStyle.Regular, AdminSharedUi.Texto));
                Label valor = CriarLabel(AdminRepository.Money(s.Preco), 625, y, 140, 24, 9F, FontStyle.Bold, AdminSharedUi.Texto);
                valor.TextAlign = ContentAlignment.MiddleRight;
                fatura.Controls.Add(valor);
                y += 25;
            }
            if (servicosSelecionados.Count > 5) fatura.Controls.Add(CriarLabel("+ " + (servicosSelecionados.Count - 5) + " serviço(s)", 22, y, 480, 24, 9F, FontStyle.Bold, AdminSharedUi.Rosa));

            decimal total = servicosSelecionados.Sum(s => s.Preco);
            int duracao = servicosSelecionados.Sum(s => s.DuracaoMinutos);
            fatura.Controls.Add(CriarLabel("Duração total: " + duracao + " min", 22, 280, 300, 24, 9F, FontStyle.Bold, AdminSharedUi.Cinza));
            Label totalLbl = CriarLabel("Total: " + AdminRepository.Money(total), 525, 272, 240, 34, 14F, FontStyle.Bold, AdminSharedUi.Rosa);
            totalLbl.TextAlign = ContentAlignment.MiddleRight;
            fatura.Controls.Add(totalLbl);

            conteudo.Controls.Add(CriarLabel("Método de pagamento", 0, 365, 260, 24, 10F, FontStyle.Bold, AdminSharedUi.Texto));
            cbPagamento = CriarCombo(0, 395, 250);
            cbPagamento.Items.Add("Cartão");
            cbPagamento.Items.Add("MBWay");
            cbPagamento.Items.Add("Dinheiro");
            cbPagamento.Items.Add("Multibanco");
            cbPagamento.SelectedIndex = 0;
            conteudo.Controls.Add(cbPagamento);

            Label nota = CriarLabel("O estado da fatura fica como Pendente. Depois pode alterar na tabela de faturas.", 290, 399, 500, 38, 9F, FontStyle.Bold, AdminSharedUi.Cinza);
            conteudo.Controls.Add(nota);
        }

        private Control CriarCardServico(AdminServico s)
        {
            bool selecionado = servicosSelecionados.Exists(x => x.IdServico == s.IdServico);
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(258, 255);
            card.BorderRadius = 22;
            card.FillColor = selecionado ? Color.FromArgb(255, 235, 243) : Color.FromArgb(255, 248, 251);
            card.BorderThickness = selecionado ? 2 : 0;
            card.BorderColor = AdminSharedUi.Rosa;
            card.Margin = new Padding(0, 0, 18, 18);

            PictureBox foto = new PictureBox();
            foto.Image = AdminSharedUi.CarregarImagemServico(s.Foto, s.Categoria);
            foto.SizeMode = PictureBoxSizeMode.Zoom;
            foto.Location = new Point(18, 16);
            foto.Size = new Size(72, 72);
            card.Controls.Add(foto);

            card.Controls.Add(CriarLabel(s.Nome, 102, 16, 140, 42, 10.5F, FontStyle.Bold, AdminSharedUi.Texto));
            card.Controls.Add(CriarLabel(AdminRepository.Money(s.Preco), 102, 58, 140, 24, 9.5F, FontStyle.Bold, AdminSharedUi.Rosa));
            card.Controls.Add(CriarLabel(string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria, 18, 98, 210, 24, 9F, FontStyle.Regular, AdminSharedUi.Cinza));
            card.Controls.Add(CriarLabel("Duração: " + s.DuracaoMinutos + " min", 18, 122, 210, 24, 9F, FontStyle.Regular, AdminSharedUi.Cinza));

            Guna2RatingStar rating = new Guna2RatingStar();
            rating.Location = new Point(18, 154);
            rating.Size = new Size(130, 28);
            rating.Value = (float)Math.Max(0, Math.Min(5, s.Avaliacao));
            rating.Enabled = false;
            card.Controls.Add(rating);
            card.Controls.Add(CriarLabel(s.Avaliacao.ToString("0.0") + "/5", 155, 158, 70, 22, 8.5F, FontStyle.Bold, AdminSharedUi.Cinza));

            Guna2Button add = new Guna2Button();
            add.Text = selecionado ? "Adicionado ✓" : "Adicionar à marcação";
            add.BorderRadius = 17;
            add.FillColor = selecionado ? Color.FromArgb(60, 170, 100) : AdminSharedUi.Rosa;
            add.ForeColor = Color.White;
            add.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            add.Size = new Size(210, 38);
            add.Location = new Point(22, 202);
            add.Click += delegate
            {
                AdminServico existente = servicosSelecionados.Find(x => x.IdServico == s.IdServico);
                if (existente == null) servicosSelecionados.Add(s);
                else servicosSelecionados.Remove(existente);
                RenderizarPasso();
            };
            card.Controls.Add(add);
            return card;
        }

        private Control CriarCardProfissional(AdminProfissional p)
        {
            bool selecionada = profissionalSelecionada != null && profissionalSelecionada.IdUsuario == p.IdUsuario;
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(260, 128);
            card.BorderRadius = 22;
            card.FillColor = selecionada ? Color.FromArgb(255, 235, 243) : Color.FromArgb(255, 248, 251);
            card.BorderColor = AdminSharedUi.Rosa;
            card.BorderThickness = selecionada ? 2 : 0;
            card.Margin = new Padding(0, 0, 16, 0);
            card.Cursor = Cursors.Hand;
            card.Click += delegate { profissionalSelecionada = p; RenderizarPasso(); };

            Guna2CirclePictureBox foto = new Guna2CirclePictureBox();
            foto.Image = AdminSharedUi.CarregarImagemPerfil(p.Foto);
            foto.SizeMode = PictureBoxSizeMode.Zoom;
            foto.Location = new Point(18, 24);
            foto.Size = new Size(72, 72);
            foto.Click += delegate { profissionalSelecionada = p; RenderizarPasso(); };
            card.Controls.Add(foto);

            Label nome = CriarLabel(p.Nome, 105, 20, 140, 28, 10.5F, FontStyle.Bold, AdminSharedUi.Texto);
            nome.Click += delegate { profissionalSelecionada = p; RenderizarPasso(); };
            card.Controls.Add(nome);
            Label esp = CriarLabel(string.IsNullOrWhiteSpace(p.Especialidade) ? "Profissional BeauteCare" : p.Especialidade, 105, 48, 140, 22, 8.7F, FontStyle.Regular, AdminSharedUi.Cinza);
            esp.Click += delegate { profissionalSelecionada = p; RenderizarPasso(); };
            card.Controls.Add(esp);

            Guna2RatingStar rating = new Guna2RatingStar();
            rating.Location = new Point(105, 72);
            rating.Size = new Size(104, 24);
            rating.Value = (float)Math.Max(0, Math.Min(5, p.Avaliacao));
            rating.Enabled = false;
            card.Controls.Add(rating);
            card.Controls.Add(CriarLabel(p.Avaliacao.ToString("0.0"), 212, 75, 40, 20, 8F, FontStyle.Bold, Color.FromArgb(200, 160, 70)));
            Label estado = CriarLabel(selecionada ? "Selecionada ✓" : "Selecionar", 105, 100, 120, 22, 8.7F, FontStyle.Bold, selecionada ? AdminSharedUi.Verde : AdminSharedUi.Rosa);
            card.Controls.Add(estado);
            return card;
        }

        private string ResumoServicosSelecionados()
        {
            if (servicosSelecionados.Count == 0) return "Nenhum procedimento adicionado ainda.";
            int min = servicosSelecionados.Sum(s => s.DuracaoMinutos);
            decimal total = servicosSelecionados.Sum(s => s.Preco);
            return servicosSelecionados.Count + " procedimento(s) · " + min + " min · " + AdminRepository.Money(total);
        }

        private Guna2ComboBox CriarCombo(int x, int y, int w)
        {
            Guna2ComboBox c = new Guna2ComboBox();
            c.BorderRadius = 16;
            c.Size = new Size(w, 44);
            c.Location = new Point(x, y);
            c.Font = new Font("Segoe UI", 9.5F);
            c.DropDownStyle = ComboBoxStyle.DropDownList;
            return c;
        }

        private void SelecionarCombo(Guna2ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                AdminOpcao op = combo.Items[i] as AdminOpcao;
                if (op != null && op.Id == id) { combo.SelectedIndex = i; return; }
            }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private Guna2Button CriarBotaoEscolha(string texto, int x, int y, bool ativo)
        {
            Guna2Button b = new Guna2Button();
            b.Text = texto;
            b.BorderRadius = 22;
            b.Size = new Size(190, 56);
            b.Location = new Point(x, y);
            b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            b.FillColor = ativo ? AdminSharedUi.Rosa : AdminSharedUi.RosaClaro;
            b.ForeColor = ativo ? Color.White : AdminSharedUi.Rosa;
            return b;
        }

        private Guna2Button CriarChipCategoria(string texto, bool ativo)
        {
            Guna2Button b = new Guna2Button();
            b.Text = texto;
            b.BorderRadius = 18;
            b.Size = new Size(Math.Max(118, texto.Length * 9 + 40), 38);
            b.Margin = new Padding(0, 0, 10, 0);
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            b.FillColor = ativo ? AdminSharedUi.Rosa : Color.FromArgb(255, 240, 246);
            b.ForeColor = ativo ? Color.White : AdminSharedUi.Rosa;
            return b;
        }

        private Guna2Panel CriarMiniCard(int x, int y, int w, int h)
        {
            Guna2Panel p = new Guna2Panel();
            p.Location = new Point(x, y);
            p.Size = new Size(w, h);
            p.BorderRadius = 18;
            p.FillColor = Color.FromArgb(255, 248, 251);
            return p;
        }

        private Label CriarLabel(string text, int x, int y, int w, int h, float size, FontStyle style, Color color)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.Font = new Font("Segoe UI", size, style);
            l.ForeColor = color;
            l.BackColor = Color.Transparent;
            return l;
        }

        private void BtnProximo_Click(object sender, EventArgs e)
        {
            if (passo == 1)
            {
                clienteSelecionada = cbCliente == null ? null : cbCliente.SelectedItem as AdminOpcao;
                if (!paraMimAdmin && clienteSelecionada == null)
                {
                    MessageBox.Show("Escolha a cliente antes de avançar.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cbHora == null || cbHora.SelectedItem == null)
                {
                    MessageBox.Show("Escolha uma hora.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                dataEscolhida = dtData.Value.Date;
                horaEscolhida = cbHora.SelectedItem.ToString();
                passo = 2;
                RenderizarPasso();
            }
            else if (passo == 2)
            {
                if (servicosSelecionados.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um procedimento à marcação.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                passo = 3;
                RenderizarPasso();
            }
            else if (passo == 3)
            {
                if (profissionalSelecionada == null)
                {
                    MessageBox.Show("Escolha a profissional que fará o atendimento.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                passo = 4;
                RenderizarPasso();
            }
            else ConfirmarMarcacao();
        }

        private void ConfirmarMarcacao()
        {
            try
            {
                if (profissionalSelecionada == null)
                {
                    MessageBox.Show("Escolha a profissional que fará o atendimento.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                TimeSpan hora = TimeSpan.Parse(horaEscolhida);
                int? idCliente = paraMimAdmin ? (int?)AdminRepository.ResolverIdAdmin() : clienteSelecionada.Id;
                int? idProfissional = profissionalSelecionada.IdUsuario;
                AdminInfo admin = AdminRepository.GetAdmin(AdminRepository.ResolverIdAdmin());
                string nomeCliente = paraMimAdmin ? ((string.IsNullOrWhiteSpace(admin.Nome) ? "Administradora" : admin.Nome) + " (admin)") : clienteSelecionada.Nome;
                string nomeProfissional = profissionalSelecionada.Nome;
                string servicosTexto = string.Join(", ", servicosSelecionados.Select(s => s.Nome).ToArray());
                int duracao = servicosSelecionados.Sum(s => s.DuracaoMinutos);
                decimal total = servicosSelecionados.Sum(s => s.Preco);
                AdminServico primeiro = servicosSelecionados[0];
                string metodo = cbPagamento == null || cbPagamento.SelectedItem == null ? "Cartão" : cbPagamento.SelectedItem.ToString();
                if (!AdminRepository.ProfissionalDisponivel(idProfissional.Value, dataEscolhida.Date, hora, duracao))
                {
                    MessageBox.Show("Esta profissional deixou de estar disponível neste horário. Volte ao passo da profissional e escolha outra opção.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passo = 3;
                    RenderizarPasso();
                    return;
                }

                int idMarc = AdminRepository.CriarMarcacao(idCliente, idProfissional, primeiro.IdServico, nomeCliente, nomeProfissional, servicosTexto, dataEscolhida.Date, hora, duracao, total, "Confirmada", paraMimAdmin ? "Criada pela administração para a própria admin" : "Criada pela administração");
                AdminRepository.CriarFaturaMarcacao(idMarc, idCliente, idProfissional, nomeCliente, nomeProfissional, servicosTexto, total, 0, total, metodo, "Pendente", dataEscolhida.Date, hora);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível criar a marcação: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
