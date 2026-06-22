using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Configuration;


namespace ProjetoFinal
{
    public partial class FormRecuperarSenha : Form
    {
        string emailUsuario = "";
        int codigoGerado;
        DateTime tempoExpiracao;
        string conexao =
        @"Server=(localdb)\MSSQLLocalDB;Database=BeauteCareDB;Trusted_Connection=True;";
        int tempo = 45;


        public FormRecuperarSenha()
        {
            InitializeComponent();
        }



        private void FormRecuperarSenha_Load(object sender, EventArgs e)
        {
            AdminSharedUi.AjustarJanelaEQualidade(this);
            AplicarEstiloRecuperacao();
            PrepararCamposCodigo();
            panelEtapa1.Visible = true;
            panelEtapa2.Visible = false;
            panelEtapa3.Visible = false;
            panelEtapa1.BringToFront();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string codigoDigitado =
            txt1.Text + txt2.Text + txt3.Text +
            txt4.Text + txt5.Text + txt6.Text;

            if (DateTime.Now > tempoExpiracao)
            {
                MessageBox.Show("Código expirado!");
                return;
            }

            if (codigoDigitado == codigoGerado.ToString())
            {
                panelEtapa2.Visible = false;
                panelEtapa1.Visible = false;
                panelEtapa3.Visible = true;
            }
            else
            {
                MessageBox.Show("Código inválido!");
            }
        }

        private void btnCriarConta_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        public void EnviarCodigoEmail(string destino, int codigo)
        {
            string host = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
            string portaTexto = ConfigurationManager.AppSettings["SmtpPort"] ?? "587";
            string sslTexto = ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true";
            string user = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
            string pass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";
            if (!string.IsNullOrWhiteSpace(pass)) pass = pass.Replace(" ", "");
            string from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(from))
                throw new InvalidOperationException("Configure SmtpUser, SmtpPass e SmtpFrom no App.config para enviar o código de recuperação.");

            int porta = 587;
            int.TryParse(portaTexto, out porta);
            bool usarSsl = !sslTexto.Equals("false", StringComparison.OrdinalIgnoreCase);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(from, "BeautéCare");
                mail.To.Add(destino);
                mail.Subject = "Código de recuperação - BeautéCare";
                mail.IsBodyHtml = true;
                mail.Body = MontarEmailCodigo(codigo);

                using (SmtpClient smtp = new SmtpClient(string.IsNullOrWhiteSpace(host) ? "smtp.gmail.com" : host, porta <= 0 ? 587 : porta))
                {
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = usarSsl;
                    smtp.Send(mail);
                }
            }
        }

        private string MontarEmailCodigo(int codigo)
        {
            return @"<div style='font-family:Segoe UI,Arial,sans-serif;background:#fff6fa;padding:26px'>" +
                   @"<div style='max-width:560px;margin:auto;background:#ffffff;border-radius:20px;padding:26px;border:1px solid #ffd6e5'>" +
                   @"<h2 style='margin:0;color:#ff4f87'>BeautéCare</h2>" +
                   @"<p style='color:#5a4a5a;font-size:15px'>Use o código abaixo para restaurar a sua senha:</p>" +
                   @"<div style='font-size:34px;font-weight:700;letter-spacing:8px;color:#ff4f87;background:#fff0f6;border-radius:14px;padding:14px;text-align:center'>" + codigo.ToString() + @"</div>" +
                   @"<p style='color:#888;font-size:13px;margin-top:18px'>Este código expira em 5 minutos. Se não pediu esta recuperação, ignore este email.</p>" +
                   @"</div></div>";
        }

        private void AplicarEstiloRecuperacao()
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.FromArgb(255, 241, 247);
                this.Paint -= FormRecuperarSenha_Paint;
                this.Paint += FormRecuperarSenha_Paint;

                foreach (Control p in new Control[] { panelEtapa1, panelEtapa2, panelEtapa3 })
                {
                    if (p == null) continue;
                    p.BackColor = Color.Transparent;
                    try
                    {
                        Guna.UI2.WinForms.Guna2ShadowPanel sp = p as Guna.UI2.WinForms.Guna2ShadowPanel;
                        if (sp != null)
                        {
                            sp.FillColor = Color.White;
                            sp.Radius = 24;
                            sp.ShadowColor = Color.FromArgb(255, 190, 210);
                            sp.ShadowDepth = 18;
                        }
                    }
                    catch { }
                }

                EstilizarTextBox(txtEmail, false);
                EstilizarTextBox(txtSenha, true);
                EstilizarTextBox(txtConfirmar, true);
                label7.Text = "Enviamos um código para";
                label13.Text = "Digite o código de segurança";
                label14.Text = "Verifique o seu email e introduza os 6 números.";
                label16.Text = "Não recebeu?";
                lblReenviar.ForeColor = Color.FromArgb(255, 79, 135);
            }
            catch { }
        }

        private void FormRecuperarSenha_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(255, 247, 251), Color.FromArgb(255, 224, 238), 45F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
            catch { }
        }

        private void EstilizarTextBox(Guna.UI2.WinForms.Guna2TextBox txt, bool senha)
        {
            if (txt == null) return;
            txt.BorderRadius = 14;
            txt.BorderColor = Color.FromArgb(255, 190, 210);
            txt.FocusedState.BorderColor = Color.FromArgb(255, 79, 135);
            txt.FillColor = Color.White;
            txt.Font = new Font("Segoe UI", 10F);
            txt.ForeColor = Color.FromArgb(55, 45, 55);
            txt.PasswordChar = senha ? '●' : '\0';
        }

        private void PrepararCamposCodigo()
        {
            Guna.UI2.WinForms.Guna2TextBox[] campos = new[] { txt1, txt2, txt3, txt4, txt5, txt6 };
            for (int i = 0; i < campos.Length; i++)
            {
                Guna.UI2.WinForms.Guna2TextBox campo = campos[i];
                if (campo == null) continue;
                campo.MaxLength = 1;
                campo.TextAlign = HorizontalAlignment.Center;
                campo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                campo.ForeColor = Color.FromArgb(255, 79, 135);
                campo.BorderRadius = 18;
                campo.BorderColor = Color.FromArgb(255, 190, 210);
                campo.FocusedState.BorderColor = Color.FromArgb(255, 79, 135);
                campo.TextChanged -= CampoCodigo_TextChanged;
                campo.TextChanged += CampoCodigo_TextChanged;
                campo.KeyPress -= CampoCodigo_KeyPress;
                campo.KeyPress += CampoCodigo_KeyPress;
            }
        }

        private void CampoCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void CampoCodigo_TextChanged(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2TextBox atual = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (atual == null || atual.Text.Length == 0) return;
            if (atual == txt1) txt2.Focus();
            else if (atual == txt2) txt3.Focus();
            else if (atual == txt3) txt4.Focus();
            else if (atual == txt4) txt5.Focus();
            else if (atual == txt5) txt6.Focus();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Digite um email!");
                return;
            }

            emailUsuario = txtEmail.Text;

            // validar se existe no banco
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Usuarios WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", emailUsuario);

                int existe = (int)cmd.ExecuteScalar();

                if (existe == 0)
                {
                    MessageBox.Show("Email não encontrado!");
                    return;
                }
            }

            Random r = new Random();
            codigoGerado = r.Next(100000, 999999);

            tempoExpiracao = DateTime.Now.AddMinutes(5);

            try
            {
                EnviarCodigoEmail(emailUsuario, codigoGerado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível enviar o código de recuperação: " + ex.Message, "BeautéCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string nome = emailUsuario.Split('@')[0];
            string dominio = emailUsuario.Split('@')[1];

            string oculto = nome.Length > 3
                ? nome.Substring(0, 3) + "***@" + dominio
                : nome + "***@" + dominio;

            lblEmailEtapa2.Text = oculto;
            

            panelEtapa1.Visible = false;
            panelEtapa2.Visible = true;
            panelEtapa3.Visible = false;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void lblReenviar_Click(object sender, EventArgs e)
        {
            tempo = 45;
            lblReenviar.Enabled = false;
            timer1.Start();

            Random r = new Random();
            codigoGerado = r.Next(100000, 999999);

            tempoExpiracao = DateTime.Now.AddMinutes(5);

            try
            {
                EnviarCodigoEmail(emailUsuario, codigoGerado);
                MessageBox.Show("Novo código enviado para o email.", "BeautéCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível reenviar o código: " + ex.Message, "BeautéCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tempo--;

            lblReenviar.Text = $"Reenviar ({tempo}s)";

            if (tempo <= 0)
            {
                timer1.Stop();
                lblReenviar.Enabled = true;
                lblReenviar.Text = "Reenviar";
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (txtSenha.Text != txtConfirmar.Text)
            {
                MessageBox.Show("As senhas não coincidem!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                string query = "UPDATE Usuarios SET Senha=@Senha WHERE Email=@Email";

                SqlCommand cmd = new SqlCommand(query, conn);
                string senhaHash = HashSenha(txtSenha.Text);
                cmd.Parameters.AddWithValue("@Senha", senhaHash);
                cmd.Parameters.AddWithValue("@Email", emailUsuario);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Senha redefinida com sucesso!");

            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }

        public string HashSenha(string senha)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();

        }
    }
    
}
