using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class NovaMarcacaoAdmin
    {
        private Label lblTituloJanela;
        private Guna2Button btnFechar;

        private void InitializeComponent()
        {
            this.container = new Guna2ShadowPanel();
            this.lblTituloJanela = new Label();
            this.lblPasso = new Label();
            this.btnFechar = new Guna2Button();
            this.conteudo = new Panel();
            this.btnAnterior = new Guna2Button();
            this.btnProximo = new Guna2Button();
            this.container.SuspendLayout();
            this.SuspendLayout();

            // NovaMarcacaoAdmin
            this.Text = "Nova Marcação";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(960, 725);
            this.AutoScaleMode = AutoScaleMode.None;

            // container
            this.container.Dock = DockStyle.Fill;
            this.container.Radius = 26;
            this.container.FillColor = Color.White;
            this.container.ShadowColor = Color.Gray;
            this.container.ShadowDepth = 20;
            this.container.Name = "container";

            // lblTituloJanela
            this.lblTituloJanela.Text = "+ Nova Marcação";
            this.lblTituloJanela.Location = new Point(36, 25);
            this.lblTituloJanela.Size = new Size(390, 42);
            this.lblTituloJanela.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            this.lblTituloJanela.ForeColor = Color.FromArgb(55, 45, 55);
            this.lblTituloJanela.Name = "lblTituloJanela";

            // lblPasso
            this.lblPasso.Text = string.Empty;
            this.lblPasso.Location = new Point(38, 72);
            this.lblPasso.Size = new Size(760, 28);
            this.lblPasso.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.lblPasso.ForeColor = Color.FromArgb(255, 79, 135);
            this.lblPasso.Name = "lblPasso";

            // btnFechar
            this.btnFechar.Text = "×";
            this.btnFechar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            this.btnFechar.FillColor = Color.WhiteSmoke;
            this.btnFechar.ForeColor = Color.FromArgb(55, 45, 55);
            this.btnFechar.BorderRadius = 16;
            this.btnFechar.Size = new Size(45, 40);
            this.btnFechar.Location = new Point(870, 25);
            this.btnFechar.Name = "btnFechar";

            // conteudo
            this.conteudo.Location = new Point(36, 112);
            this.conteudo.Size = new Size(880, 515);
            this.conteudo.AutoScroll = true;
            this.conteudo.BackColor = Color.White;
            this.conteudo.Name = "conteudo";

            // btnAnterior
            this.btnAnterior.Text = "Anterior";
            this.btnAnterior.BorderRadius = 20;
            this.btnAnterior.FillColor = Color.WhiteSmoke;
            this.btnAnterior.ForeColor = Color.FromArgb(55, 45, 55);
            this.btnAnterior.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnAnterior.Size = new Size(140, 44);
            this.btnAnterior.Location = new Point(600, 650);
            this.btnAnterior.Name = "btnAnterior";

            // btnProximo
            this.btnProximo.Text = "Próximo";
            this.btnProximo.BorderRadius = 20;
            this.btnProximo.FillColor = Color.FromArgb(255, 79, 135);
            this.btnProximo.ForeColor = Color.White;
            this.btnProximo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnProximo.Size = new Size(165, 44);
            this.btnProximo.Location = new Point(752, 650);
            this.btnProximo.Name = "btnProximo";

            this.container.Controls.Add(this.lblTituloJanela);
            this.container.Controls.Add(this.lblPasso);
            this.container.Controls.Add(this.btnFechar);
            this.container.Controls.Add(this.conteudo);
            this.container.Controls.Add(this.btnAnterior);
            this.container.Controls.Add(this.btnProximo);
            this.Controls.Add(this.container);
            this.container.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
