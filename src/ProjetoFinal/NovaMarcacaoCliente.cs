using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public class NovaMarcacaoCliente : Form
    {
        private int passo = 1;
        private readonly int idCliente;
        private readonly int? idServicoInicial;
        private List<ServicoInfo> servicos = new List<ServicoInfo>();
        private List<ServicoInfo> selecionados = new List<ServicoInfo>();
        private List<ProfissionalInfo> profissionais = new List<ProfissionalInfo>();
        private int idProfissionalSelecionado = 0;
        private DateTime dataSelecionada = DateTime.Today.AddDays(1);
        private TimeSpan horaSelecionada = new TimeSpan(10, 0, 0);
        private string metodo = "Cartão";
        private CupaoInfo cupaoAplicado = null;
        private string categoriaAtual = "Todas";

        private Guna2ShadowPanel painel;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private FlowLayoutPanel conteudo;
        private Guna2Button btnVoltar;
        private Guna2Button btnNext;

        public NovaMarcacaoCliente(int idCliente) : this(idCliente, null) { }

        public NovaMarcacaoCliente(int idCliente, int? idServicoInicial)
        {
            this.idCliente = idCliente;
            this.idServicoInicial = idServicoInicial;
            Inicializar();
            CarregarDados();
            MontarPasso();
        }

        private void Inicializar()
        {
            Text = "+ Nova Marcação";
            Size = new Size(1020, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Padding = new Padding(18);

            painel = new Guna2ShadowPanel();
            painel.Dock = DockStyle.Fill;
            painel.Radius = 28;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 18;
            Controls.Add(painel);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            fechar.FillColor = Color.WhiteSmoke;
            fechar.ForeColor = ClienteSharedUi.Texto;
            fechar.BorderRadius = 16;
            fechar.Size = new Size(45, 40);
            fechar.Location = new Point(925, 20);
            fechar.Click += delegate { Close(); };
            painel.Controls.Add(fechar);

            lblTitulo = new Label();
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = ClienteSharedUi.Rosa;
            lblTitulo.Location = new Point(38, 26);
            lblTitulo.Size = new Size(680, 42);
            painel.Controls.Add(lblTitulo);

            lblSubtitulo = new Label();
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = ClienteSharedUi.Cinza;
            lblSubtitulo.Location = new Point(40, 68);
            lblSubtitulo.Size = new Size(760, 26);
            painel.Controls.Add(lblSubtitulo);

            conteudo = new FlowLayoutPanel();
            conteudo.Location = new Point(38, 110);
            conteudo.Size = new Size(910, 490);
            conteudo.AutoScroll = true;
            conteudo.WrapContents = true;
            conteudo.FlowDirection = FlowDirection.LeftToRight;
            painel.Controls.Add(conteudo);

            btnVoltar = new Guna2Button();
            btnVoltar.Text = "Voltar";
            btnVoltar.FillColor = Color.WhiteSmoke;
            btnVoltar.ForeColor = ClienteSharedUi.Texto;
            btnVoltar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnVoltar.BorderRadius = 18;
            btnVoltar.Size = new Size(130, 45);
            btnVoltar.Location = new Point(620, 620);
            btnVoltar.Click += delegate { if (passo > 1) { passo--; MontarPasso(); } };
            painel.Controls.Add(btnVoltar);

            btnNext = new Guna2Button();
            btnNext.Text = "Next";
            btnNext.FillColor = ClienteSharedUi.Rosa;
            btnNext.ForeColor = Color.White;
            btnNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNext.BorderRadius = 18;
            btnNext.Size = new Size(170, 45);
            btnNext.Location = new Point(770, 620);
            btnNext.Click += BtnNext_Click;
            painel.Controls.Add(btnNext);
        }

        private void CarregarDados()
        {
            ClienteRepository.EnsureSchema();
            servicos = ProfissionalRepository.GetServicos();
            profissionais = ClienteRepository.GetProfissionais();
            if (idServicoInicial.HasValue)
            {
                ServicoInfo s = servicos.FirstOrDefault(x => x.IdServico == idServicoInicial.Value);
                if (s != null) selecionados.Add(s);
            }
            if (profissionais.Count > 0) idProfissionalSelecionado = profissionais[0].IdUsuario;
        }

        private void MontarPasso()
        {
            conteudo.Controls.Clear();
            btnVoltar.Visible = passo > 1;
            btnNext.Text = passo == 3 ? "Confirmar marcação" : "Next";
            if (passo == 1) MontarPassoServicos();
            else if (passo == 2) MontarPassoProfissionalData();
            else MontarPassoPagamento();
        }

        private void MontarPassoServicos()
        {
            lblTitulo.Text = "1. Escolha os procedimentos";
            lblSubtitulo.Text = "Adicione um ou mais serviços. A foto, duração, avaliação e preço aparecem em cada card.";

            Guna2Panel areaCategorias = new Guna2Panel();
            areaCategorias.Size = new Size(880, 58);
            areaCategorias.BorderRadius = 18;
            areaCategorias.FillColor = Color.FromArgb(255, 248, 251);
            areaCategorias.Margin = new Padding(8, 8, 8, 12);

            FlowLayoutPanel chips = new FlowLayoutPanel();
            chips.Location = new Point(16, 12);
            chips.Size = new Size(850, 36);
            chips.WrapContents = false;
            chips.AutoScroll = true;
            chips.BackColor = Color.Transparent;
            areaCategorias.Controls.Add(chips);

            List<string> cats = new List<string>();
            cats.Add("Todas");
            cats.AddRange(servicos.Select(s => CategoriaServico(s)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x));
            foreach (string cat in cats)
                chips.Controls.Add(CriarChipCategoria(cat, string.Equals(cat, categoriaAtual, StringComparison.OrdinalIgnoreCase)));
            conteudo.Controls.Add(areaCategorias);

            IEnumerable<ServicoInfo> lista = servicos;
            if (!string.Equals(categoriaAtual, "Todas", StringComparison.OrdinalIgnoreCase))
                lista = servicos.Where(s => string.Equals(CategoriaServico(s), categoriaAtual, StringComparison.OrdinalIgnoreCase));
            foreach (ServicoInfo s in lista)
                conteudo.Controls.Add(CriarCardServico(s));
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
            chip.FillColor = ativo ? ClienteSharedUi.Rosa : ClienteSharedUi.RosaClaro;
            chip.ForeColor = ativo ? Color.White : ClienteSharedUi.Rosa;
            chip.Click += delegate { categoriaAtual = texto; MontarPasso(); };
            return chip;
        }

        private Control CriarCardServico(ServicoInfo s)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(280, 220);
            card.BorderRadius = 20;
            card.FillColor = selecionados.Any(x => x.IdServico == s.IdServico) ? ClienteSharedUi.RosaClaro : Color.FromArgb(255, 248, 251);
            card.Margin = new Padding(8, 8, 12, 12);
            card.Tag = s;

            Guna2PictureBox img = new Guna2PictureBox();
            img.Location = new Point(16, 16);
            img.Size = new Size(86, 86);
            img.BorderRadius = 18;
            img.SizeMode = PictureBoxSizeMode.Zoom;
            img.Image = ProfissionalSharedUi.CarregarImagemServico(s);
            card.Controls.Add(img);

            card.Controls.Add(Label(s.Nome, 115, 18, 145, 26, 10F, FontStyle.Bold, ClienteSharedUi.Texto));
            card.Controls.Add(Label(s.DuracaoMinutos + " min", 115, 47, 80, 22, 8.5F, FontStyle.Regular, ClienteSharedUi.Cinza));
            card.Controls.Add(Label(MontarEstrelas(s.Avaliacao), 115, 70, 140, 22, 9F, FontStyle.Bold, Color.FromArgb(245, 170, 32)));
            card.Controls.Add(Label(ClienteRepository.FormatarMoeda(s.Preco), 115, 96, 130, 24, 10F, FontStyle.Bold, ClienteSharedUi.Rosa));

            Label desc = Label(string.IsNullOrWhiteSpace(s.Categoria) ? "Procedimento BeauteCare" : s.Categoria, 18, 112, 230, 32, 8.5F, FontStyle.Regular, ClienteSharedUi.Cinza);
            card.Controls.Add(desc);

            Guna2Button btn = new Guna2Button();
            bool ja = selecionados.Any(x => x.IdServico == s.IdServico);
            btn.Text = ja ? "Adicionado ✓" : "Adicionar à marcação";
            btn.FillColor = ja ? Color.FromArgb(64, 168, 92) : ClienteSharedUi.Rosa;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btn.BorderRadius = 15;
            btn.Size = new Size(210, 36);
            btn.Location = new Point(35, 166);
            btn.Click += delegate
            {
                if (selecionados.Any(x => x.IdServico == s.IdServico)) selecionados.RemoveAll(x => x.IdServico == s.IdServico);
                else selecionados.Add(s);
                MontarPasso();
            };
            card.Controls.Add(btn);
            return card;
        }

        private void MontarPassoProfissionalData()
        {
            lblTitulo.Text = "2. Escolha a profissional, o dia e a hora";
            lblSubtitulo.Text = "Escolha quem vai realizar o atendimento. Depois selecione a data e o horário da marcação.";

            Guna2Panel topo = new Guna2Panel();
            topo.Size = new Size(880, 82);
            topo.BorderRadius = 18;
            topo.FillColor = ClienteSharedUi.RosaClaro;
            topo.Margin = new Padding(8, 8, 8, 14);

            DateTimePicker dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Short;
            dtp.Value = dataSelecionada;
            dtp.MinDate = DateTime.Today;
            dtp.Location = new Point(26, 28);
            dtp.Width = 145;
            dtp.ValueChanged += delegate { dataSelecionada = dtp.Value.Date; idProfissionalSelecionado = 0; MontarPasso(); };
            topo.Controls.Add(dtp);

            ComboBox horas = new ComboBox();
            horas.DropDownStyle = ComboBoxStyle.DropDownList;
            horas.Location = new Point(195, 28);
            horas.Width = 120;
            string[] hs = new string[] { "09:00", "10:00", "11:00", "12:00", "14:00", "15:00", "16:00", "17:00" };
            horas.Items.AddRange(hs);
            horas.SelectedItem = horaSelecionada.ToString(@"hh\:mm");
            if (horas.SelectedIndex < 0) horas.SelectedIndex = 1;
            horas.SelectedIndexChanged += delegate { horaSelecionada = TimeSpan.Parse(horas.SelectedItem.ToString()); idProfissionalSelecionado = 0; MontarPasso(); };
            topo.Controls.Add(horas);

            topo.Controls.Add(Label("Total selecionado: " + ClienteRepository.FormatarMoeda(selecionados.Sum(s => s.Preco)), 350, 27, 260, 25, 10F, FontStyle.Bold, ClienteSharedUi.Rosa));
            topo.Controls.Add(Label("Duração: " + selecionados.Sum(s => s.DuracaoMinutos) + " min", 640, 27, 170, 25, 10F, FontStyle.Bold, ClienteSharedUi.Texto));
            conteudo.Controls.Add(topo);

            int duracao = Math.Max(30, selecionados.Sum(s => s.DuracaoMinutos));
            List<ProfissionalInfo> disponiveis = profissionais.Where(p => AdminRepository.ProfissionalDisponivel(p.IdUsuario, dataSelecionada, horaSelecionada, duracao)).ToList();
            if (idProfissionalSelecionado > 0 && !disponiveis.Any(p => p.IdUsuario == idProfissionalSelecionado)) idProfissionalSelecionado = 0;
            if (idProfissionalSelecionado == 0 && disponiveis.Count > 0) idProfissionalSelecionado = disponiveis[0].IdUsuario;

            if (disponiveis.Count == 0)
            {
                Guna2Panel vazio = new Guna2Panel();
                vazio.Size = new Size(860, 120);
                vazio.BorderRadius = 20;
                vazio.FillColor = Color.FromArgb(255, 248, 251);
                vazio.Margin = new Padding(8, 8, 12, 12);
                vazio.Controls.Add(Label("Nenhuma profissional disponível neste horário.", 26, 26, 760, 30, 13F, FontStyle.Bold, ClienteSharedUi.Texto));
                vazio.Controls.Add(Label("Escolha outra data ou outra hora para continuar.", 26, 62, 760, 26, 10F, FontStyle.Regular, ClienteSharedUi.Cinza));
                conteudo.Controls.Add(vazio);
                return;
            }

            foreach (ProfissionalInfo p in disponiveis)
                conteudo.Controls.Add(CriarCardProfissional(p));
        }

        private Control CriarCardProfissional(ProfissionalInfo p)
        {
            Guna2Panel card = new Guna2Panel();
            card.Size = new Size(280, 170);
            card.BorderRadius = 20;
            card.FillColor = idProfissionalSelecionado == p.IdUsuario ? ClienteSharedUi.RosaClaro : Color.FromArgb(255, 248, 251);
            card.Margin = new Padding(8, 8, 12, 12);
            card.Cursor = Cursors.Hand;
            card.Click += delegate { idProfissionalSelecionado = p.IdUsuario; MontarPasso(); };

            Guna2CirclePictureBox foto = new Guna2CirclePictureBox();
            foto.Location = new Point(18, 22);
            foto.Size = new Size(72, 72);
            foto.SizeMode = PictureBoxSizeMode.Zoom;
            foto.Image = ProfissionalSharedUi.CarregarImagemPerfil(p.Foto);
            foto.Click += delegate { idProfissionalSelecionado = p.IdUsuario; MontarPasso(); };
            card.Controls.Add(foto);

            card.Controls.Add(Label(p.Nome, 105, 25, 150, 24, 10F, FontStyle.Bold, ClienteSharedUi.Texto));
            card.Controls.Add(Label(string.IsNullOrWhiteSpace(p.Especialidade) ? "Profissional BeauteCare" : p.Especialidade, 105, 53, 160, 38, 8.3F, FontStyle.Regular, ClienteSharedUi.Cinza));
            card.Controls.Add(Label(MontarEstrelas(p.Avaliacao), 105, 95, 150, 22, 9F, FontStyle.Bold, Color.FromArgb(245, 170, 32)));

            Label escolher = Label(idProfissionalSelecionado == p.IdUsuario ? "Selecionada ✓" : "Selecionar", 105, 125, 150, 24, 9F, FontStyle.Bold, idProfissionalSelecionado == p.IdUsuario ? Color.FromArgb(64, 168, 92) : ClienteSharedUi.Rosa);
            card.Controls.Add(escolher);
            return card;
        }

        private void MontarPassoPagamento()
        {
            lblTitulo.Text = "3. Pré-fatura e pagamento";
            lblSubtitulo.Text = "Confirme o resumo, escolha o método de pagamento e aplique um cupão antes de pagar.";

            Guna2Panel resumo = new Guna2Panel();
            resumo.Size = new Size(520, 420);
            resumo.BorderRadius = 22;
            resumo.FillColor = Color.FromArgb(255, 248, 251);
            resumo.Margin = new Padding(8, 8, 20, 12);
            conteudo.Controls.Add(resumo);

            int y = 22;
            resumo.Controls.Add(Label("Pré-fatura", 25, y, 200, 30, 14F, FontStyle.Bold, ClienteSharedUi.Rosa));
            y += 45;
            foreach (ServicoInfo s in selecionados)
            {
                resumo.Controls.Add(Label(s.Nome, 25, y, 290, 24, 9.2F, FontStyle.Regular, ClienteSharedUi.Texto));
                Label valor = Label(ClienteRepository.FormatarMoeda(s.Preco), 350, y, 130, 24, 9.2F, FontStyle.Bold, ClienteSharedUi.Texto);
                valor.TextAlign = ContentAlignment.MiddleRight;
                resumo.Controls.Add(valor);
                y += 28;
            }
            y += 10;
            AddLine(resumo, 25, y, 455);
            y += 25;
            decimal subtotal = selecionados.Sum(s => s.Preco);
            decimal desconto = 0m;
            try { desconto = ClienteRepository.CalcularDescontoCupao(cupaoAplicado, selecionados); } catch { desconto = 0m; }
            if (desconto > subtotal) desconto = subtotal;
            AddRow(resumo, "Subtotal", ClienteRepository.FormatarMoeda(subtotal), ref y);
            AddRow(resumo, "Desconto", ClienteRepository.FormatarMoeda(desconto), ref y);
            AddRow(resumo, "Total", ClienteRepository.FormatarMoeda(subtotal - desconto), ref y, true);
            y += 10;
            resumo.Controls.Add(Label("Data", 25, y, 120, 22, 8.5F, FontStyle.Bold, ClienteSharedUi.Cinza));
            resumo.Controls.Add(Label(dataSelecionada.ToString("dd/MM/yyyy") + " às " + horaSelecionada.ToString(@"hh\:mm"), 165, y, 250, 22, 8.5F, FontStyle.Regular, ClienteSharedUi.Texto));
            y += 28;
            ProfissionalInfo prof = profissionais.FirstOrDefault(p => p.IdUsuario == idProfissionalSelecionado);
            resumo.Controls.Add(Label("Profissional", 25, y, 120, 22, 8.5F, FontStyle.Bold, ClienteSharedUi.Cinza));
            resumo.Controls.Add(Label(prof == null ? "" : prof.Nome, 165, y, 250, 22, 8.5F, FontStyle.Regular, ClienteSharedUi.Texto));

            Guna2Panel pagamento = new Guna2Panel();
            pagamento.Size = new Size(330, 420);
            pagamento.BorderRadius = 22;
            pagamento.FillColor = Color.White;
            pagamento.Margin = new Padding(8, 8, 12, 12);
            conteudo.Controls.Add(pagamento);

            pagamento.Controls.Add(Label("Pagamento", 25, 22, 200, 30, 14F, FontStyle.Bold, ClienteSharedUi.Rosa));
            string[] metodos = new string[] { "Cartão", "MBWay", "Dinheiro" };
            int yy = 70;
            foreach (string m in metodos)
            {
                Guna2Button b = new Guna2Button();
                b.Text = m;
                b.Tag = m;
                b.FillColor = metodo == m ? ClienteSharedUi.Rosa : ClienteSharedUi.RosaClaro;
                b.ForeColor = metodo == m ? Color.White : ClienteSharedUi.Rosa;
                b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                b.BorderRadius = 15;
                b.Size = new Size(125, 38);
                b.Location = new Point(25, yy);
                b.Click += delegate { metodo = (string)b.Tag; MontarPasso(); };
                pagamento.Controls.Add(b);
                yy += 48;
            }

            pagamento.Controls.Add(Label("Cupão", 25, 230, 100, 22, 9F, FontStyle.Bold, ClienteSharedUi.Texto));
            Guna2TextBox txtCupao = new Guna2TextBox();
            txtCupao.Name = "txtCupaoCliente";
            txtCupao.PlaceholderText = "Código do cupão";
            txtCupao.Text = cupaoAplicado == null ? "" : cupaoAplicado.Codigo;
            txtCupao.BorderRadius = 14;
            txtCupao.Location = new Point(25, 258);
            txtCupao.Size = new Size(190, 40);
            pagamento.Controls.Add(txtCupao);

            Guna2Button aplicar = new Guna2Button();
            aplicar.Text = "Aplicar";
            aplicar.FillColor = ClienteSharedUi.Rosa;
            aplicar.ForeColor = Color.White;
            aplicar.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            aplicar.BorderRadius = 14;
            aplicar.Size = new Size(78, 40);
            aplicar.Location = new Point(222, 258);
            aplicar.Click += delegate
            {
                try
                {
                    CupaoInfo c = ClienteRepository.GetCupao(idCliente, txtCupao.Text.Trim());
                    if (c == null) { MessageBox.Show("Cupão inválido, usado ou expirado.", "BeauteCare"); return; }
                    ClienteRepository.CalcularDescontoCupao(c, selecionados);
                    cupaoAplicado = c;
                    MessageBox.Show("Cupão aplicado: " + c.Codigo, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MontarPasso();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };
            pagamento.Controls.Add(aplicar);

            Label cupao = Label(cupaoAplicado == null ? "Nenhum cupão aplicado." : cupaoAplicado.Codigo + " · " + cupaoAplicado.PercentualDesconto.ToString("0") + "%", 25, 308, 270, 44, 8.7F, FontStyle.Bold, cupaoAplicado == null ? ClienteSharedUi.Cinza : Color.FromArgb(64, 168, 92));
            pagamento.Controls.Add(cupao);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (passo == 1)
                {
                    if (selecionados.Count == 0) { MessageBox.Show("Escolha pelo menos um procedimento.", "BeauteCare"); return; }
                    if (idServicoInicial.HasValue && selecionados.Count == 1)
                    {
                        DialogResult r = MessageBox.Show("Quer adicionar mais algum procedimento?", "BeauteCare", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (r == DialogResult.Yes) return;
                    }
                    passo++;
                    MontarPasso();
                }
                else if (passo == 2)
                {
                    if (idProfissionalSelecionado <= 0) { MessageBox.Show("Escolha a profissional.", "BeauteCare"); return; }
                    passo++;
                    MontarPasso();
                }
                else
                {
                    int duracao = Math.Max(30, selecionados.Sum(s => s.DuracaoMinutos));
                    if (!AdminRepository.ProfissionalDisponivel(idProfissionalSelecionado, dataSelecionada, horaSelecionada, duracao))
                    {
                        MessageBox.Show("Esta profissional deixou de estar disponível neste horário. Escolha outra profissional ou outro horário.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        passo = 2;
                        MontarPasso();
                        return;
                    }
                    int idMarcacao = ClienteRepository.CriarMarcacaoCliente(idCliente, idProfissionalSelecionado, dataSelecionada, horaSelecionada, selecionados, metodo, cupaoAplicado);
                    MessageBox.Show("Marcação criada com sucesso.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível criar a marcação: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddRow(Control parent, string label, string valor, ref int y)
        {
            AddRow(parent, label, valor, ref y, false);
        }

        private void AddRow(Control parent, string label, string valor, ref int y, bool total)
        {
            parent.Controls.Add(Label(label, 25, y, 150, 25, total ? 11F : 9F, FontStyle.Bold, total ? ClienteSharedUi.Rosa : ClienteSharedUi.Cinza));
            Label v = Label(valor, 330, y, 150, 25, total ? 11F : 9F, FontStyle.Bold, total ? ClienteSharedUi.Rosa : ClienteSharedUi.Texto);
            v.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(v);
            y += total ? 36 : 28;
        }

        private string MontarEstrelas(decimal valor)
        {
            int cheias = (int)Math.Round(valor, MidpointRounding.AwayFromZero);
            if (cheias < 0) cheias = 0;
            if (cheias > 5) cheias = 5;
            return new string('★', cheias) + new string('☆', 5 - cheias) + " " + valor.ToString("0.0");
        }

        private Label Label(string text, int x, int y, int w, int h, float size, FontStyle style, Color color)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.Font = new Font("Segoe UI", size, style);
            l.ForeColor = color;
            return l;
        }

        private void AddLine(Control parent, int x, int y, int w)
        {
            Panel line = new Panel();
            line.BackColor = Color.FromArgb(255, 218, 232);
            line.Location = new Point(x, y);
            line.Size = new Size(w, 1);
            parent.Controls.Add(line);
        }
    }
}
