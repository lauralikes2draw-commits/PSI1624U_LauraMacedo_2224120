using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ProjetoFinal
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void label6_Click(object sender, EventArgs e)
        {
            label6.ForeColor = Color.FromArgb(240, 98, 146);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
        
            AbrirLink("https://www.tiktok.com/_.laura.macedo");
        
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string email = txtUsuario.Text.Trim();
            string senha = txtSenha.Text.Trim();

            if (email == "" || senha == "")
            {
                MessageBox.Show("Preencha o e-mail e a senha.");
                return;
            }

            try
            {
                ProfissionalRepository.EnsureSchema();
                ClienteRepository.EnsureSchema();
                using (SqlConnection conn = Conexao.Conectar())
                {
                    conn.Open();

                    string sql = @"SELECT IdUsuario, Nome, Email, TipoUsuario, Foto
                           FROM Usuarios
                           WHERE Email = @Email AND (Senha = @SenhaHash OR Senha = @SenhaTexto) AND Ativo = 1";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@SenhaHash", GerarHashSenha(senha));
                        cmd.Parameters.AddWithValue("@SenhaTexto", senha);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UsuarioLogado.Id = Convert.ToInt32(reader["IdUsuario"]);
                                UsuarioLogado.Nome = reader["Nome"].ToString();
                                UsuarioLogado.Email = reader["Email"].ToString();
                                UsuarioLogado.Tipo = reader["TipoUsuario"].ToString();
                                UsuarioLogado.Foto = reader["Foto"] == DBNull.Value ? "" : reader["Foto"].ToString();

                                GuardarLembrar();
                                reader.Close();
                                AtualizarUltimaVisita(conn, UsuarioLogado.Id);

                                MessageBox.Show("Bem-vinda, " + UsuarioLogado.Nome + "!");

                                if (UsuarioLogado.Tipo == "Admin")
                                {
                                    FormAdminPrinc formadmprinc = new FormAdminPrinc();
                                    formadmprinc.Show();
                                    this.Hide();
                                }
                                else if (UsuarioLogado.Tipo == "Cliente")
                                {
                                    if (ClienteRepository.TemAvaliacaoPendente(UsuarioLogado.Id))
                                    {
                                        AvaliacoesCliente avaliacoes = new AvaliacoesCliente();
                                        avaliacoes.Show();
                                    }
                                    else
                                    {
                                        FormClientePrinc formClienteprinc = new FormClientePrinc();
                                        formClienteprinc.Show();
                                    }
                                    this.Hide();
                                }
                                else if (UsuarioLogado.Tipo == "Profissional")
                                {
                                    DashboardProfissional dashboard = new DashboardProfissional();
                                    dashboard.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                MessageBox.Show("E-mail ou senha incorretos.");
                            }
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro na conexão: " + erro.Message);
            }
        }

        private string GerarHashSenha(string senha)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void AtualizarUltimaVisita(SqlConnection conn, int idUsuario)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Usuarios SET UltimaVisita = GETDATE() WHERE IdUsuario = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        private void GuardarLembrar()
        {
            if (chkLembrar.Checked)
            {
                Properties.Settings.Default.EmailSalvo = txtUsuario.Text;
                Properties.Settings.Default.SenhaSalva = txtSenha.Text;
                Properties.Settings.Default.Lembrar = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.EmailSalvo = "";
                Properties.Settings.Default.SenhaSalva = "";
                Properties.Settings.Default.Lembrar = false;
                Properties.Settings.Default.Save();
            }
        }

        
            bool senhaVisivel = false;

            private void FormLogin_Load(object sender, EventArgs e)
            {
                txtSenha.PasswordChar = '●';

                if (Properties.Settings.Default.Lembrar == true)
                {
                    txtUsuario.Text = Properties.Settings.Default.EmailSalvo;
                    txtSenha.Text = Properties.Settings.Default.SenhaSalva;
                    chkLembrar.Checked = true;
                }
            }

            private void txtSenha_IconRightClick(object sender, EventArgs e)
            {
                if (senhaVisivel == false)
                {
                    txtSenha.PasswordChar = '\0';
                    senhaVisivel = true;
                }
                else
                {
                    txtSenha.PasswordChar = '●';
                    senhaVisivel = false;
                }
            }

        private void btnCriarConta_Click(object sender, EventArgs e)
        {
            FormCriarConta criarConta = new FormCriarConta();
            criarConta.Show();
            this.Hide();
        }

        private void AbrirLink(string link)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Não foi possível abrir o link.");
            }
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            AbrirLink("https://www.instagram.com/_.laura.macedo._");
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            AbrirLink("https://wa.me/351913885275?text=Olá,%20gostaria%20de%20marcar%20um%20serviço.");
        }

        private void lblEsqueceuSenha_Click(object sender, EventArgs e)
        {
            FormRecuperarSenha formRec = new FormRecuperarSenha();
            formRec.Show();
            this.Hide();

        }

        private void txtSenha_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
