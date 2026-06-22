using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public class DetalheMarcacaoClienteForm : Form
    {
        private readonly int idMarcacao;
        private ClienteMarcacaoInfo marcacao;

        public DetalheMarcacaoClienteForm(int idMarcacao)
        {
            this.idMarcacao = idMarcacao;
            Inicializar();
            Carregar();
        }

        private void Inicializar()
        {
            Text = "Detalhe da Marcação";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Size = new Size(520, 520);
            Padding = new Padding(15);
        }

        private void Carregar()
        {
            marcacao = ClienteRepository.GetMarcacao(idMarcacao);
            if (marcacao == null)
            {
                MessageBox.Show("Marcação não encontrada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            Montar();
        }

        private void Montar()
        {
            Guna2ShadowPanel painel = new Guna2ShadowPanel();
            painel.Dock = DockStyle.Fill;
            painel.Radius = 22;
            painel.FillColor = Color.White;
            painel.ShadowColor = Color.Gray;
            painel.ShadowDepth = 18;
            Controls.Add(painel);

            Guna2Button fechar = new Guna2Button();
            fechar.Text = "×";
            fechar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            fechar.FillColor = Color.WhiteSmoke;
            fechar.ForeColor = ClienteSharedUi.Texto;
            fechar.BorderRadius = 14;
            fechar.Size = new Size(42, 38);
            fechar.Location = new Point(435, 20);
            fechar.Click += delegate { Close(); };
            painel.Controls.Add(fechar);

            Label titulo = Label("Detalhe da Marcação", 30, 25, 360, 34, 16F, FontStyle.Bold, ClienteSharedUi.Rosa);
            painel.Controls.Add(titulo);

            Guna2CirclePictureBox fotoServico = new Guna2CirclePictureBox();
            fotoServico.Location = new Point(35, 85);
            fotoServico.Size = new Size(88, 88);
            fotoServico.SizeMode = PictureBoxSizeMode.Zoom;
            ServicoInfo fake = new ServicoInfo { Nome = marcacao.Servico, Foto = marcacao.ServicoFoto, Categoria = marcacao.Servico };
            fotoServico.Image = ProfissionalSharedUi.CarregarImagemServico(fake);
            painel.Controls.Add(fotoServico);

            painel.Controls.Add(Label(marcacao.Servico, 145, 92, 300, 28, 12F, FontStyle.Bold, ClienteSharedUi.Texto));
            painel.Controls.Add(Label(marcacao.DataMarcacao.ToString("dd/MM/yyyy") + " às " + marcacao.Hora.ToString(@"hh\:mm"), 145, 122, 300, 22, 9.5F, FontStyle.Regular, ClienteSharedUi.Cinza));
            painel.Controls.Add(Label("Duração: " + marcacao.DuracaoMinutos + " min", 145, 146, 180, 22, 9.5F, FontStyle.Regular, ClienteSharedUi.Cinza));

            Guna2Panel estado = new Guna2Panel();
            estado.Location = new Point(330, 145);
            estado.Size = new Size(115, 30);
            estado.BorderRadius = 12;
            estado.FillColor = ClienteSharedUi.CorEstado(marcacao.Estado);
            Label lblEstado = Label(marcacao.Estado, 0, 5, 115, 20, 8F, FontStyle.Bold, Color.White);
            lblEstado.TextAlign = ContentAlignment.MiddleCenter;
            estado.Controls.Add(lblEstado);
            painel.Controls.Add(estado);

            AddLine(painel, 35, 205);
            int y = 230;
            AddRow(painel, "Profissional", marcacao.Profissional, ref y);
            AddRow(painel, "Avaliação profissional", MontarEstrelas(marcacao.AvaliacaoProfissional), ref y);
            AddRow(painel, "Valor", ClienteRepository.FormatarMoeda(marcacao.Valor), ref y);
            AddRow(painel, "Estado", marcacao.Estado, ref y);

            Guna2Button cancelar = new Guna2Button();
            cancelar.Text = "Cancelar marcação";
            cancelar.FillColor = Color.FromArgb(220, 78, 92);
            cancelar.ForeColor = Color.White;
            cancelar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            cancelar.BorderRadius = 18;
            cancelar.Size = new Size(190, 44);
            cancelar.Location = new Point(160, 415);
            cancelar.Visible = !marcacao.Estado.ToLowerInvariant().Contains("cancel") && !marcacao.Estado.ToLowerInvariant().Contains("concl");
            cancelar.Click += delegate
            {
                if (MessageBox.Show("Deseja cancelar esta marcação?", "BeauteCare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ClienteRepository.CancelarMarcacao(idMarcacao, UsuarioLogado.Id);
                    MessageBox.Show("Marcação cancelada.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            };
            painel.Controls.Add(cancelar);
        }

        private void AddRow(Control parent, string label, string valor, ref int y)
        {
            parent.Controls.Add(Label(label, 35, y, 170, 24, 9.5F, FontStyle.Bold, ClienteSharedUi.Cinza));
            Label v = Label(valor, 215, y, 230, 24, 9.5F, FontStyle.Regular, ClienteSharedUi.Texto);
            v.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(v);
            y += 35;
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

        private void AddLine(Control parent, int x, int y)
        {
            Panel line = new Panel();
            line.BackColor = Color.FromArgb(255, 218, 232);
            line.Location = new Point(x, y);
            line.Size = new Size(420, 1);
            parent.Controls.Add(line);
        }
    }
}
