using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class ReciboFaturaForm : Form
    {
        private readonly int idFatura;
        private readonly bool mostrarComissao;
        private FaturaInfo fatura;

        public ReciboFaturaForm()
        {
            this.idFatura = 0;
            this.mostrarComissao = true;
            InitializeComponent();
        }

        public ReciboFaturaForm(int idFatura) : this(idFatura, true)
        {
        }

        public ReciboFaturaForm(int idFatura, bool mostrarComissao)
        {
            this.idFatura = idFatura;
            this.mostrarComissao = mostrarComissao;
            InitializeComponent();
            CarregarFatura();
         }

        private void CarregarFatura()
        {
            fatura = ProfissionalRepository.GetFatura(idFatura);
            if (fatura == null)
            {
                MessageBox.Show("Fatura não encontrada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            MontarRecibo();
        }

        private void MontarRecibo()
        {
            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Dock = DockStyle.Fill;
            painel.Radius = 22;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 18;
            this.Controls.Add(painel);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            fechar.FillColor = Color.WhiteSmoke;
            fechar.ForeColor = ProfissionalSharedUi.Texto;
            fechar.BorderRadius = 14;
            fechar.Size = new Size(42, 38);
            fechar.Location = new Point(435, 20);
            fechar.Click += Fechar_Click;
            painel.Controls.Add(fechar);

            Label titulo = CriarLabel("BEAUTECARE", 0, 28, 490, 32, 18F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            painel.Controls.Add(titulo);

            Label sub = CriarLabel("Fatura / Recibo", 0, 62, 490, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            sub.TextAlign = ContentAlignment.MiddleCenter;
            painel.Controls.Add(sub);

            AddLine(painel, 36, 100);

            int y = 118;
            painel.Controls.Add(CriarLabel("N.º", 36, y, 120, 22, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            painel.Controls.Add(CriarLabel(fatura.NumeroFatura, 160, y, 280, 22, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            y += 28;
            painel.Controls.Add(CriarLabel("Data", 36, y, 120, 22, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            string dataHora = fatura.DataFatura.ToString("dd/MM/yyyy");
            if (fatura.Hora.HasValue) dataHora += " · " + fatura.Hora.Value.ToString(@"hh\:mm");
            painel.Controls.Add(CriarLabel(dataHora, 160, y, 280, 22, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
            y += 28;
            painel.Controls.Add(CriarLabel("Cliente", 36, y, 120, 22, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            painel.Controls.Add(CriarLabel(fatura.Cliente, 160, y, 280, 22, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
            y += 28;
            painel.Controls.Add(CriarLabel("Profissional", 36, y, 120, 22, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            painel.Controls.Add(CriarLabel(fatura.Profissional, 160, y, 280, 22, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
            y += 38;

            AddLine(painel, 36, y);
            y += 18;

            painel.Controls.Add(CriarLabel("Serviço(s)", 36, y, 250, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            painel.Controls.Add(CriarLabel("Valor", 330, y, 110, 24, 10F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            y += 32;

            string[] servicos = fatura.Servicos.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (servicos.Length == 0) servicos = new string[] { fatura.Servicos };
            decimal valorPorLinha = servicos.Length > 0 ? fatura.Subtotal / servicos.Length : fatura.Subtotal;
            foreach (string s in servicos)
            {
                painel.Controls.Add(CriarLabel(s.Trim(), 36, y, 285, 24, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
                Label valor = CriarLabel(ProfissionalRepository.FormatarMoeda(valorPorLinha), 330, y, 110, 24, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto);
                valor.TextAlign = ContentAlignment.MiddleRight;
                painel.Controls.Add(valor);
                y += 28;
            }

            y += 12;
            AddLine(painel, 36, y);
            y += 24;

            painel.Controls.Add(CriarLabel("Subtotal", 230, y, 100, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            Label subtotal = CriarLabel(ProfissionalRepository.FormatarMoeda(fatura.Subtotal), 330, y, 110, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            subtotal.TextAlign = ContentAlignment.MiddleRight;
            painel.Controls.Add(subtotal);
            y += 28;

            painel.Controls.Add(CriarLabel("Desconto", 230, y, 100, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            Label desconto = CriarLabel(ProfissionalRepository.FormatarMoeda(fatura.Desconto), 330, y, 110, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto);
            desconto.TextAlign = ContentAlignment.MiddleRight;
            painel.Controls.Add(desconto);
            y += 36;

            Guna2Panel totalBox = new Guna2Panel();
            totalBox.Location = new Point(36, y);
            totalBox.Size = new Size(405, 62);
            totalBox.BorderRadius = 18;
            totalBox.FillColor = ProfissionalSharedUi.RosaClaro;
            painel.Controls.Add(totalBox);

            totalBox.Controls.Add(CriarLabel("TOTAL", 18, 17, 120, 28, 13F, FontStyle.Bold, ProfissionalSharedUi.Rosa));
            Label total = CriarLabel(ProfissionalRepository.FormatarMoeda(fatura.Total), 220, 15, 160, 32, 14F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            total.TextAlign = ContentAlignment.MiddleRight;
            totalBox.Controls.Add(total);
            y += 82;

            painel.Controls.Add(CriarLabel("Método de pagamento", 36, y, 180, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            painel.Controls.Add(CriarLabel(fatura.MetodoPagamento, 235, y, 200, 24, 9F, FontStyle.Regular, ProfissionalSharedUi.Texto));
            y += 30;
            painel.Controls.Add(CriarLabel("Estado", 36, y, 180, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
            painel.Controls.Add(CriarLabel(fatura.Estado, 235, y, 200, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Rosa));
            y += 30;
            if (mostrarComissao)
            {
                painel.Controls.Add(CriarLabel("Comissão profissional", 36, y, 180, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
                decimal comissao = fatura.Total * fatura.ComissaoPercentual / 100m;
                painel.Controls.Add(CriarLabel(fatura.ComissaoPercentual.ToString("0") + "% · " + ProfissionalRepository.FormatarMoeda(comissao), 235, y, 205, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            }
            else
            {
                painel.Controls.Add(CriarLabel("Cupão/desconto", 36, y, 180, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Cinza));
                painel.Controls.Add(CriarLabel(ProfissionalRepository.FormatarMoeda(fatura.Desconto), 235, y, 205, 24, 9F, FontStyle.Bold, ProfissionalSharedUi.Texto));
            }

            Label rodape = CriarLabel("Obrigada pela preferência ♡", 0, 640, 490, 28, 10F, FontStyle.Bold, ProfissionalSharedUi.Rosa);
            rodape.TextAlign = ContentAlignment.MiddleCenter;
            painel.Controls.Add(rodape);
        }

        private void Fechar_Click(object sender, EventArgs e)
        {
            Close();
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

        private void AddLine(Control parent, int x, int y)
        {
            Panel line = new Panel();
            line.BackColor = Color.FromArgb(255, 218, 232);
            line.Location = new Point(x, y);
            line.Size = new Size(405, 1);
            parent.Controls.Add(line);
        }


    }
}
