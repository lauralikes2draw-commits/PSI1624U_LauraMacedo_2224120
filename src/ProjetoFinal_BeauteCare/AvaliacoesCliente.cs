using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ProjetoFinal
{
    public partial class AvaliacoesCliente : Form
    {
        private int idCliente;
        private ClienteMarcacaoInfo pendente;

        public AvaliacoesCliente()
        {
            InitializeComponent();
            this.Load += AvaliacoesCliente_Load;
        }

        private void AvaliacoesCliente_Load(object sender, EventArgs e)
        {
            try
            {
                ClienteSharedUi.PrepararPagina(this, "avaliacoes");
                idCliente = ClienteRepository.ResolverIdCliente();
                ConfigurarPagina();
                CarregarAvaliacaoPendente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar avaliações: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarPagina()
        {
            label6.Text = "Avaliação";
            label7.Text = "Avalie a sua última experiência conosco";
            label16.Text = "Avalie a Profissional";
            label14.Text = "Como foi o atendimento\r\ne profissionalismo?";
            label13.Text = "Avalie o Espaço";
            label12.Text = "Como foi a limpeza, organização\r\ne conforto do espaço?";
            label17.Text = "Avalie o Serviço";
            label15.Text = "Como foi a qualidade do serviço\r\nrealizado?";
            label18.Text = "Selecione uma avaliação";
            label19.Text = "Selecione uma avaliação";
            label20.Text = "Selecione uma avaliação";
            label21.Text = "A sua avaliação é anónima e ajuda outras clientes a escolherem o melhor para elas.";
            guna2Button1.Text = "Enviar Avaliação";
            guna2Button1.Click -= EnviarAvaliacao_Click;
            guna2Button1.Click += EnviarAvaliacao_Click;

            guna2RatingStar1.ValueChanged -= Rating_ValueChanged;
            guna2RatingStar2.ValueChanged -= Rating_ValueChanged;
            guna2RatingStar3.ValueChanged -= Rating_ValueChanged;
            guna2RatingStar1.ValueChanged += Rating_ValueChanged;
            guna2RatingStar2.ValueChanged += Rating_ValueChanged;
            guna2RatingStar3.ValueChanged += Rating_ValueChanged;
        }

        private void CarregarAvaliacaoPendente()
        {
            pendente = ClienteRepository.GetAvaliacaoPendente(idCliente);
            if (pendente == null)
            {
                label9.Text = "Sem avaliações pendentes";
                lblClientes.Text = "Quando concluir uma marcação, ela aparecerá aqui.";
                label8.Text = "Obrigada por usar a BeauteCare";
                label10.Text = "";
                label11.Text = "";
                guna2Button1.Enabled = false;
                guna2Button1.FillColor = Color.LightGray;
                return;
            }

            label9.Text = pendente.Servico;
            lblClientes.Text = pendente.DataMarcacao.ToString("dd MMM, yyyy", ClienteRepository.Pt) + " às " + pendente.Hora.ToString(@"hh\:mm");
            label8.Text = "com " + pendente.Profissional;
            label10.Text = pendente.Profissional;
            label11.Text = pendente.Servico;

            ServicoInfo fake = new ServicoInfo { Nome = pendente.Servico, Categoria = pendente.Servico, Foto = pendente.ServicoFoto };
            guna2CirclePictureBox2.Image = ProfissionalSharedUi.CarregarImagemServico(fake);
            guna2CirclePictureBox3.Image = ProfissionalSharedUi.CarregarImagemServico(fake);
            guna2CirclePictureBox4.Image = ProfissionalSharedUi.CarregarImagemPerfil(pendente.ProfissionalFoto);
            guna2CirclePictureBox5.Image = ProfissionalSharedUi.CarregarImagemPerfil(pendente.ProfissionalFoto);
            guna2CirclePictureBox6.Image = Properties.Resources.icons8_building_60;
            guna2CirclePictureBox7.Image = ProfissionalSharedUi.CarregarImagemServico(fake);
        }

        private void Rating_ValueChanged(object sender, EventArgs e)
        {
            AtualizarTextoRating(guna2RatingStar1, label18);
            AtualizarTextoRating(guna2RatingStar2, label19);
            AtualizarTextoRating(guna2RatingStar3, label20);
        }

        private void AtualizarTextoRating(Guna2RatingStar rating, Label label)
        {
            if (rating.Value <= 0) label.Text = "Selecione uma avaliação";
            else label.Text = NormalizarNota(rating.Value).ToString("0.0") + "/5 selecionado";
        }

        private decimal NormalizarNota(float valor)
        {
            decimal nota = Math.Round((decimal)valor, 1, MidpointRounding.AwayFromZero);
            if (nota < 0) nota = 0;
            if (nota > 5) nota = 5;
            return nota;
        }

        private void EnviarAvaliacao_Click(object sender, EventArgs e)
        {
            if (pendente == null) return;
            try
            {
                decimal n1 = NormalizarNota(guna2RatingStar1.Value);
                decimal n2 = NormalizarNota(guna2RatingStar2.Value);
                decimal n3 = NormalizarNota(guna2RatingStar3.Value);
                if (n1 <= 0 || n2 <= 0 || n3 <= 0)
                {
                    MessageBox.Show("Avalie todos os itens antes de enviar.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ClienteRepository.EnviarAvaliacao(idCliente, pendente.IdMarcacao, n1, n2, n3, "");
                MessageBox.Show("Avaliação enviada com sucesso. Obrigada!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormClientePrinc dash = new FormClientePrinc();
                dash.Show();
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível enviar a avaliação: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void guna2CirclePictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
