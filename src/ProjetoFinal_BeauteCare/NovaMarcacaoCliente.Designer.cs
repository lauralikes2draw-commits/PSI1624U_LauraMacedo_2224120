using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class NovaMarcacaoCliente
    {
        private Guna2Button btnFechar;

        private void InitializeComponent()
        {
            this.painel = new Guna2ShadowPanel();
            this.btnFechar = new Guna2Button();
            this.lblTitulo = new Label();
            this.lblSubtitulo = new Label();
            this.conteudo = new FlowLayoutPanel();
            this.btnVoltar = new Guna2Button();
            this.btnNext = new Guna2Button();
            this.painel.SuspendLayout();
            this.SuspendLayout();

            // NovaMarcacaoCliente
            this.Text = "+ Nova Marcação";
            this.Size = new Size(1020, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(18);

            // painel
            this.painel.Dock = DockStyle.Fill;
            this.painel.Radius = 28;
            this.painel.FillColor = Color.White;
            this.painel.ShadowColor = Color.Gray;
            this.painel.ShadowDepth = 18;
            this.painel.Name = "painel";

            // btnFechar
            this.btnFechar.Text = "×";
            this.btnFechar.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.btnFechar.FillColor = Color.WhiteSmoke;
            this.btnFechar.ForeColor = Color.FromArgb(55, 45, 55);
            this.btnFechar.BorderRadius = 16;
            this.btnFechar.Size = new Size(45, 40);
            this.btnFechar.Location = new Point(925, 20);
            this.btnFechar.Name = "btnFechar";

            // lblTitulo
            this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitulo.ForeColor = Color.FromArgb(255, 79, 135);
            this.lblTitulo.Location = new Point(38, 26);
            this.lblTitulo.Size = new Size(680, 42);
            this.lblTitulo.Name = "lblTitulo";

            // lblSubtitulo
            this.lblSubtitulo.Font = new Font("Segoe UI", 10F);
            this.lblSubtitulo.ForeColor = Color.FromArgb(125, 125, 125);
            this.lblSubtitulo.Location = new Point(40, 68);
            this.lblSubtitulo.Size = new Size(760, 26);
            this.lblSubtitulo.Name = "lblSubtitulo";

            // conteudo
            this.conteudo.Location = new Point(38, 110);
            this.conteudo.Size = new Size(910, 490);
            this.conteudo.AutoScroll = true;
            this.conteudo.WrapContents = true;
            this.conteudo.FlowDirection = FlowDirection.LeftToRight;
            this.conteudo.Name = "conteudo";

            // btnVoltar
            this.btnVoltar.Text = "Voltar";
            this.btnVoltar.FillColor = Color.WhiteSmoke;
            this.btnVoltar.ForeColor = Color.FromArgb(55, 45, 55);
            this.btnVoltar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnVoltar.BorderRadius = 18;
            this.btnVoltar.Size = new Size(130, 45);
            this.btnVoltar.Location = new Point(620, 620);
            this.btnVoltar.Name = "btnVoltar";

            // btnNext
            this.btnNext.Text = "Next";
            this.btnNext.FillColor = Color.FromArgb(255, 79, 135);
            this.btnNext.ForeColor = Color.White;
            this.btnNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnNext.BorderRadius = 18;
            this.btnNext.Size = new Size(170, 45);
            this.btnNext.Location = new Point(770, 620);
            this.btnNext.Name = "btnNext";

            this.painel.Controls.Add(this.btnFechar);
            this.painel.Controls.Add(this.lblTitulo);
            this.painel.Controls.Add(this.lblSubtitulo);
            this.painel.Controls.Add(this.conteudo);
            this.painel.Controls.Add(this.btnVoltar);
            this.painel.Controls.Add(this.btnNext);
            this.Controls.Add(this.painel);
            this.painel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
