using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public class NovaMarcacaoProfissional : Form
    {
        private readonly int idProfissional;
        private int passo = 1;
        private bool paraPropriaProfissional = false;
        private UsuarioInfo clienteSelecionada;
        private List<ServicoInfo> servicos;
        private readonly List<ServicoInfo> servicosSelecionados = new List<ServicoInfo>();
        private ProfissionalInfo profissional;

        private Guna2ShadowPanel container;
        private Panel conteudo;
        private Label lblPasso;
        private Guna2Button btnAnterior;
        private Guna2Button btnProximo;
        private Guna2Button btnFechar;
        private Guna2DateTimePicker dtData;
        private Guna2ComboBox cbHora;
        private Guna2TextBox txtCliente;
        private Label lblClienteEncontrada;
        private Guna2ComboBox cbPagamento;
        private Label lblResumoSelecionados;
        private DateTime dataEscolhida;
        private string horaEscolhida = "09:00";
        private string categoriaAtual = "Todas";

        public NovaMarcacaoProfissional(int idProfissional, DateTime dataInicial)
        {
            this.idProfissional = idProfissional;
            dataEscolhida = dataInicial.Date;
            profissional = ProfissionalRepository.GetProfissional(idProfissional);
            servicos = ProfissionalRepository.GetServicos();
            InicializarJanela(dataInicial);
            RenderizarPasso();
        }

        private void InicializarJanela(DateTime dataInicial)
        {
            this.Text = "Nova Marcação";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(930, 700);
            this.Padding = new Padding(18);

            container = new Guna2ShadowPanel();
            container.Dock = DockStyle.Fill;
            container.Radius = 24;
            container.FillColor = Color.White;
            container.ShadowColor = Color.Gray;
            container.ShadowDepth = 20;
            this.Controls.Add(container);

            Label titulo = new Label();
            titulo.Text = "+ Nova Marcação";
            titulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titulo.ForeColor = ProfissionalSharedUi.Texto;
            titulo.Location = new Point(32, 24);
            titulo.Size = new Size(360, 42);
            container.Controls.Add(titulo);

            lblPasso = new Label();
            lblPasso.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPasso.ForeColor = ProfissionalSharedUi.Rosa;
            lblPasso.Location = new Point(36, 68);
            lblPasso.Size = new Size(720, 28);
            container.Controls.Add(lblPasso);

            btnFechar = new Guna2Button();
            btnFechar.Text = "×";
            btnFechar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            btnFechar.FillColor = Color.WhiteSmoke;
            btnFechar.ForeColor = ProfissionalSharedUi.Texto;
            btnFechar.BorderRadius = 15;
            btnFechar.Size = new Size(45, 40);
            btnFechar.Location = new Point(840, 25);
            btnFechar.Click += delegate { this.DialogResult = DialogResult.Cancel; Close(); };
            container.Controls.Add(btnFechar);

            conteudo = new Panel();
            conteudo.Location = new Point(32, 108);
            conteudo.Size = new Size(855, 495);
            conteudo.AutoScroll = true;
            conteudo.BackColor = Color.White;
            container.Controls.Add(conteudo);

            btnAnterior = new Guna2Button();
            btnAnterior.Text = "Anterior";
            btnAnterior.BorderRadius = 18;
            btnAnterior.FillColor = Color.WhiteSmoke;
            btnAnterior.ForeColor = ProfissionalSharedUi.Texto;
            btnAnterior.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAnterior.Size = new Size(140, 42);
            btnAnterior.Location = new Point(575, 620);
            btnAnterior.Click += delegate { if (passo > 1) { passo--; RenderizarPasso(); } };
            container.Controls.Add(btnAnterior);

            btnProximo = new Guna2Button();
            btnProximo.Text = "Próximo";
            btnProximo.BorderRadius = 18;
            btnProximo.FillColor = ProfissionalSharedUi.Rosa;
            btnProximo.ForeColor = Color.White;
            btnProximo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnProximo.Size = new Size(160, 42);
            btnProximo.Location = new Point(725, 620);
            btnProximo.Click += BtnProximo_Click;
            container.Controls.Add(btnProximo);

            dtData = new Guna2DateTimePicker();
            dtData.Value = dataEscolhida;
        }

        private void RenderizarPasso()
        {
            conteudo.Controls.Clear();
            btnAnterior.Visible = passo > 1;
            btnProximo.Text = passo == 3 ? "Confirmar" : "Próximo";

            if (passo == 1)
            {
                lblPasso.Text = "Passo 1 de 3 · Para quem é a marcação e quando será";
                RenderizarPassoCliente();
            }
            else if (passo == 2)
            {
                lblPasso.Text = "Passo 2 de 3 · Escolher procedimentos";
                RenderizarPassoServicos();
            }
            else
            {
                lblPasso.Text = "Passo 3 de 3 · Profissional, fatura e pagamento";
                RenderizarPassoFatura();
            }
        }

        private void RenderizarPassoCliente()
        {
            Label instrucao = CriarLabel("Escolha se a marcação é para uma cliente ou para a própria profissional.", 0, 0, 820, 30, 11F, FontStyle.Regular, ProfissionalSharedUi.Cinza);
            conteudo.Controls.Add(instrucao);

            Guna2Button btnCliente = CriarBotaoEscolha("Para cliente", 0, 50, !paraPropriaProfissional);
            Guna2Button btnPropria = CriarBotaoEscolha("Para mim", 220, 50, paraPropriaProfissional);
            btnCliente.Click += delegate { paraPropriaProfissional = false; RenderizarPasso(); };
            btnPropria.Click += delegate { paraPropriaProfissional = true; clienteSelecionada = null; RenderizarPasso(); };
            conteudo.Controls.Add(btnCliente);
            conteudo.Controls.Add(btnPropria);

            Label lblCliente = CriarLabel("User / e-mail / nome da cliente", 0, 120, 320, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            conteudo.Controls.Add(lblCliente);

            txtCliente = new Guna2TextBox();
            txtCliente.PlaceholderText = "Ex.: cliente@beautecare.pt ou Maria";
            txtCliente.BorderRadius = 14;
            txtCliente.Size = new Size(450, 42);
            txtCliente.Location = new Point(0, 150);
            txtCliente.Enabled = !paraPropriaProfissional;
            conteudo.Controls.Add(txtCliente);

            Guna2Button btnProcurar = new Guna2Button();
            btnProcurar.Text = "Identificar";
            btnProcurar.BorderRadius = 14;
            btnProcurar.FillColor = ProfissionalSharedUi.Rosa;
            btnProcurar.ForeColor = Color.White;
            btnProcurar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnProcurar.Size = new Size(130, 42);
            btnProcurar.Location = new Point(465, 150);
            btnProcurar.Enabled = !paraPropriaProfissional;
            btnProcurar.Click += ProcurarCliente_Click;
            conteudo.Controls.Add(btnProcurar);

            lblClienteEncontrada = CriarLabel(ClienteResumo(), 0, 202, 780, 28, 10F, FontStyle.Bold, paraPropriaProfissional || clienteSelecionada != null ? Color.FromArgb(45, 140, 85) : ProfissionalSharedUi.Rosa);
            conteudo.Controls.Add(lblClienteEncontrada);

            Label lblData = CriarLabel("Data", 0, 260, 120, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            conteudo.Controls.Add(lblData);

            dtData = new Guna2DateTimePicker();
            dtData.BorderRadius = 14;
            dtData.FillColor = ProfissionalSharedUi.Rosa;
            dtData.ForeColor = Color.White;
            dtData.Font = new Font("Segoe UI", 9F);
            dtData.Format = DateTimePickerFormat.Long;
            dtData.Size = new Size(260, 42);
            dtData.Location = new Point(0, 290);
            dtData.Value = dataEscolhida;
            conteudo.Controls.Add(dtData);

            Label lblHora = CriarLabel("Hora", 300, 260, 120, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            conteudo.Controls.Add(lblHora);

            cbHora = new Guna2ComboBox();
            cbHora.BorderRadius = 14;
            cbHora.Size = new Size(160, 42);
            cbHora.Location = new Point(300, 290);
            cbHora.DropDownStyle = ComboBoxStyle.DropDownList;
            for (TimeSpan h = new TimeSpan(9, 0, 0); h <= new TimeSpan(19, 0, 0); h = h.Add(TimeSpan.FromMinutes(30)))
                cbHora.Items.Add(h.ToString(@"hh\:mm"));
            if (cbHora.Items.Count > 0)
            {
                int idx = cbHora.Items.IndexOf(horaEscolhida);
                cbHora.SelectedIndex = idx >= 0 ? idx : 0;
            }
            conteudo.Controls.Add(cbHora);
        }

        private string ClienteResumo()
        {
            if (paraPropriaProfissional) return "A marcação será guardada como agenda pessoal da profissional.";
            if (clienteSelecionada != null) return "Cliente identificada: " + clienteSelecionada.Nome + " · " + clienteSelecionada.Email;
            return "Identifique a cliente antes de avançar.";
        }

        private Guna2Button CriarBotaoEscolha(string texto, int x, int y, bool ativo)
        {
            Guna2Button b = new Guna2Button();
            b.Text = texto;
            b.BorderRadius = 20;
            b.Size = new Size(190, 50);
            b.Location = new Point(x, y);
            b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            b.FillColor = ativo ? ProfissionalSharedUi.Rosa : ProfissionalSharedUi.RosaClaro;
            b.ForeColor = ativo ? Color.White : ProfissionalSharedUi.Rosa;
            return b;
        }

        private void ProcurarCliente_Click(object sender, EventArgs e)
        {
            clienteSelecionada = ProfissionalRepository.ProcurarCliente(txtCliente.Text);
            if (clienteSelecionada == null)
                lblClienteEncontrada.Text = "Cliente não encontrada. Verifique o user, e-mail ou nome.";
            else
                lblClienteEncontrada.Text = ClienteResumo();
        }

        private void RenderizarPassoServicos()
        {
            Label instrucao = CriarLabel("Escolha um ou mais procedimentos. Cada cartão mostra imagem, duração, avaliação e preço.", 0, 0, 820, 28, 11F, FontStyle.Regular, ProfissionalSharedUi.Cinza);
            conteudo.Controls.Add(instrucao);

            lblResumoSelecionados = CriarLabel(ResumoServicosSelecionados(), 0, 34, 820, 28, 10F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            conteudo.Controls.Add(lblResumoSelecionados);

            FlowLayoutPanel categorias = new FlowLayoutPanel();
            categorias.Location = new Point(0, 66);
            categorias.Size = new Size(830, 38);
            categorias.WrapContents = false;
            categorias.AutoScroll = true;
            categorias.BackColor = Color.White;
            conteudo.Controls.Add(categorias);

            List<string> cats = new List<string>();
            cats.Add("Todas");
            cats.AddRange(servicos.Select(s => CategoriaServico(s)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x));
            foreach (string cat in cats)
                categorias.Controls.Add(CriarChipCategoria(cat, string.Equals(cat, categoriaAtual, StringComparison.OrdinalIgnoreCase)));

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 112);
            flow.Size = new Size(830, 360);
            flow.AutoScroll = true;
            flow.WrapContents = true;
            conteudo.Controls.Add(flow);

            IEnumerable<ServicoInfo> lista = servicos;
            if (!string.Equals(categoriaAtual, "Todas", StringComparison.OrdinalIgnoreCase))
                lista = servicos.Where(s => string.Equals(CategoriaServico(s), categoriaAtual, StringComparison.OrdinalIgnoreCase));
            foreach (ServicoInfo s in lista)
                flow.Controls.Add(CriarCardServico(s));
        }

        private string CategoriaServico(ServicoInfo s)
        {
            return s == null || string.IsNullOrWhiteSpace(s.Categoria) ? "Outros" : s.Categoria.Trim();
        }

        private Guna2Button CriarChipCategoria(string texto, bool ativo)
        {
            Guna2Button chip = new Guna2Button();
            chip.Text = texto;
            chip.BorderRadius = 15;
            chip.Size = new Size(Math.Max(92, Math.Min(180, 22 + texto.Length * 8)), 32);
            chip.Margin = new Padding(0, 0, 10, 0);
            chip.Font = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            chip.FillColor = ativo ? ProfissionalSharedUi.Rosa : ProfissionalSharedUi.RosaClaro;
            chip.ForeColor = ativo ? Color.White : ProfissionalSharedUi.Rosa;
            chip.Click += delegate { categoriaAtual = texto; RenderizarPasso(); };
            return chip;
        }

        private Control CriarCardServico(ServicoInfo s)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(255, 255);
            card.BorderRadius = 18;
            card.FillColor = Color.FromArgb(255, 248, 251);
            card.Margin = new Padding(0, 0, 18, 18);

            PictureBox foto = new PictureBox();
            foto.Image = ProfissionalSharedUi.CarregarImagemServico(s);
            foto.SizeMode = PictureBoxSizeMode.Zoom;
            foto.Location = new Point(18, 14);
            foto.Size = new Size(72, 72);
            card.Controls.Add(foto);

            Label nome = CriarLabel(s.Nome, 100, 16, 140, 42, 10.5F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            card.Controls.Add(nome);

            Label preco = CriarLabel(ProfissionalRepository.FormatarMoeda(s.Preco), 100, 58, 140, 24, 9.5F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            card.Controls.Add(preco);

            Label dur = CriarLabel("Duração: " + s.DuracaoMinutos + " min", 18, 100, 210, 24, 9F, FontStyle.Regular, ProfissionalSharedUi.Cinza);
            card.Controls.Add(dur);

            Guna2RatingStar rating = new Guna2RatingStar();
            rating.Location = new Point(18, 130);
            rating.Size = new Size(130, 28);
            rating.Value = (float)Math.Max(0, Math.Min(5, s.Avaliacao));
            rating.Enabled = false;
            card.Controls.Add(rating);

            Label avaliacao = CriarLabel(s.Avaliacao.ToString("0.0") + "/5", 155, 134, 70, 22, 8.5F, FontStyle.Bold, ProfissionalSharedUi.Cinza);
            card.Controls.Add(avaliacao);

            Guna2Button add = new Guna2Button();
            add.Text = "Adicionar à marcação";
            add.BorderRadius = 16;
            add.FillColor = ProfissionalSharedUi.Rosa;
            add.ForeColor = Color.White;
            add.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            add.Size = new Size(205, 38);
            add.Location = new Point(22, 188);
            add.Click += delegate
            {
                if (!servicosSelecionados.Exists(x => x.IdServico == s.IdServico))
                    servicosSelecionados.Add(s);
                if (lblResumoSelecionados != null) lblResumoSelecionados.Text = ResumoServicosSelecionados();
                add.Text = "Adicionado ✓";
                add.FillColor = Color.FromArgb(60, 170, 100);
            };
            card.Controls.Add(add);
            return card;
        }

        private string ResumoServicosSelecionados()
        {
            if (servicosSelecionados.Count == 0) return "Nenhum procedimento adicionado ainda.";
            int min = servicosSelecionados.Sum(s => s.DuracaoMinutos);
            decimal total = servicosSelecionados.Sum(s => s.Preco);
            return servicosSelecionados.Count + " procedimento(s) · " + min + " min · " + ProfissionalRepository.FormatarMoeda(total);
        }

        private void RenderizarPassoFatura()
        {
            Guna2Panel profCard = new Guna2Panel();
            profCard.Size = new Size(800, 105);
            profCard.Location = new Point(0, 0);
            profCard.BorderRadius = 18;
            profCard.FillColor = Color.FromArgb(255, 248, 251);
            conteudo.Controls.Add(profCard);

            Guna2CirclePictureBox foto = new Guna2CirclePictureBox();
            foto.Image = ProfissionalSharedUi.CarregarImagemPerfil(profissional.Foto);
            foto.SizeMode = PictureBoxSizeMode.Zoom;
            foto.Location = new Point(20, 18);
            foto.Size = new Size(68, 68);
            profCard.Controls.Add(foto);

            Label nome = CriarLabel(profissional.Nome, 105, 20, 350, 30, 13F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            profCard.Controls.Add(nome);

            Guna2RatingStar rating = new Guna2RatingStar();
            rating.Location = new Point(105, 55);
            rating.Size = new Size(135, 28);
            rating.Value = (float)Math.Max(0, Math.Min(5, profissional.Avaliacao));
            rating.Enabled = false;
            profCard.Controls.Add(rating);

            Label aval = CriarLabel(profissional.Avaliacao.ToString("0.0") + "/5", 245, 58, 80, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza);
            profCard.Controls.Add(aval);

            Guna2Panel fatura = new Guna2Panel();
            fatura.Size = new Size(800, 300);
            fatura.Location = new Point(0, 125);
            fatura.BorderRadius = 18;
            fatura.FillColor = Color.White;
            fatura.BorderColor = Color.FromArgb(255, 210, 225);
            fatura.BorderThickness = 1;
            conteudo.Controls.Add(fatura);

            Label titulo = CriarLabel("Pré-fatura", 22, 18, 300, 30, 14F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            fatura.Controls.Add(titulo);

            string clienteTexto = paraPropriaProfissional ? profissional.Nome + " (pessoal)" : clienteSelecionada.Nome;
            fatura.Controls.Add(CriarLabel("Cliente: " + clienteTexto, 22, 56, 520, 24, 9.5F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            fatura.Controls.Add(CriarLabel("Data: " + dataEscolhida.ToString("dd/MM/yyyy") + " · Hora: " + horaEscolhida, 22, 82, 520, 24, 9.5F, FontStyle.Regular, ProfissionalSharedUi.Cinza));

            int y = 118;
            foreach (ServicoInfo s in servicosSelecionados)
            {
                fatura.Controls.Add(CriarLabel(s.Nome + " · " + s.DuracaoMinutos + " min", 22, y, 480, 24, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
                Label valor = CriarLabel(ProfissionalRepository.FormatarMoeda(s.Preco), 620, y, 140, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto);
                valor.TextAlign = ContentAlignment.MiddleRight;
                fatura.Controls.Add(valor);
                y += 25;
            }

            decimal total = servicosSelecionados.Sum(s => s.Preco);
            int duracao = servicosSelecionados.Sum(s => s.DuracaoMinutos);
            fatura.Controls.Add(CriarLabel("Duração total: " + duracao + " min", 22, 220, 300, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            Label totalLbl = CriarLabel("Total: " + ProfissionalRepository.FormatarMoeda(total), 520, 218, 240, 34, 14F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            totalLbl.TextAlign = ContentAlignment.MiddleRight;
            fatura.Controls.Add(totalLbl);

            Label lblPagamento = CriarLabel("Método de pagamento", 0, 445, 260, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            conteudo.Controls.Add(lblPagamento);

            cbPagamento = new Guna2ComboBox();
            cbPagamento.BorderRadius = 14;
            cbPagamento.Size = new Size(240, 42);
            cbPagamento.Location = new Point(0, 475);
            cbPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPagamento.Items.Add("Cartão");
            cbPagamento.Items.Add("MBWay");
            cbPagamento.Items.Add("Dinheiro");
            cbPagamento.SelectedIndex = 0;
            conteudo.Controls.Add(cbPagamento);
        }

        private Label CriarLabel(string text, int x, int y, int w, int h, float size, FontStyle style, Color color)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.Font = new Font("Segoe UI", size, style);
            l.ForeColor = color;
            return l;
        }

        private void BtnProximo_Click(object sender, EventArgs e)
        {
            if (passo == 1)
            {
                if (!paraPropriaProfissional && clienteSelecionada == null)
                {
                    MessageBox.Show("Identifique primeiro a cliente.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            else
            {
                ConfirmarMarcacao();
            }
        }

        private void ConfirmarMarcacao()
        {
            try
            {
                TimeSpan hora = TimeSpan.Parse(horaEscolhida);
                string metodo = cbPagamento.SelectedItem == null ? "Dinheiro" : cbPagamento.SelectedItem.ToString();
                ProfissionalRepository.CriarMarcacaoComFatura(idProfissional, clienteSelecionada, paraPropriaProfissional, dataEscolhida.Date, hora, servicosSelecionados, metodo);
                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível criar a marcação: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
