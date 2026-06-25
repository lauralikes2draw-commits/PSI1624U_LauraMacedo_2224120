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

namespace ProjetoFinal
{
    public partial class FormCriarConta : Form
    {
        string conexao =
        @"Server=(localdb)\MSSQLLocalDB;Database=BeauteCareDB;Trusted_Connection=True;";
        private bool senhaVisivel = false;
        private bool confirmarSenhaVisivel = false;

        public FormCriarConta()
        {
            InitializeComponent();
            txtSenha.IconRightClick += txtSenha_IconRightClick;
            txtConfirmarSenha.IconRightClick += txtConfirmarSenha_IconRightClick;
        }

        public string HashSenha(string senha)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void FormCriarConta_Load(object sender, EventArgs e)
        {

        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefone = txtTelemovel.Text.Trim();
            string senha = txtSenha.Text;
            string confirmar = txtConfirmarSenha.Text;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(telefone) ||
                string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Preencha todos os campos!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email inválido!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (senha.Length < 6)
            {
                MessageBox.Show("A senha deve ter pelo menos 6 caracteres!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (senha != confirmar)
            {
                MessageBox.Show("As senhas não coincidem!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!chkTermos.Checked)
            {
                MessageBox.Show("Você precisa aceitar os termos!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Garante que a estrutura atual da base existe mesmo quando a conta é criada antes do primeiro login.
                try
                {
                    ProfissionalRepository.EnsureSchema();
                    ClienteRepository.EnsureSchema();
                    AdminRepository.EnsureSchema();
                }
                catch { }

                using (SqlConnection conn = Conexao.Conectar())
                {
                    conn.Open();

                    using (SqlCommand cmdVerificar = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios WHERE Email=@Email", conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Email", email);
                        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                        if (existe > 0)
                        {
                            MessageBox.Show("Este email já está cadastrado!", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    int idCriado;
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Usuarios
(Nome, Email, Telefone, Senha, TipoUsuario, Ativo, DataCriacao, UltimaVisita)
OUTPUT INSERTED.IdUsuario
VALUES (@Nome, @Email, @Telefone, @Senha, 'Cliente', 1, GETDATE(), NULL)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", nome);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Telefone", telefone);
                        cmd.Parameters.AddWithValue("@Senha", HashSenha(senha));
                        idCriado = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    using (SqlCommand cmdNotif = new SqlCommand(@"INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdUsuario, @Mensagem, GETDATE(), 0)", conn))
                    {
                        cmdNotif.Parameters.AddWithValue("@IdUsuario", idCriado);
                        cmdNotif.Parameters.AddWithValue("@Mensagem", "Bem-vinda à BeautéCare! A sua conta foi criada com sucesso.");
                        cmdNotif.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Conta criada com sucesso! Já pode iniciar sessão.", "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtNome.Clear();
                txtEmail.Clear();
                txtTelemovel.Clear();
                txtSenha.Clear();
                txtConfirmarSenha.Clear();
                chkTermos.Checked = false;

                FormLogin login = new FormLogin();
                login.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível criar a conta: " + ex.Message, "BeauteCare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void txtSenha_IconRightClick(object sender, EventArgs e)
        {
            senhaVisivel = !senhaVisivel;
            txtSenha.PasswordChar = senhaVisivel ? '\0' : '●';
        }

        private void txtConfirmarSenha_IconRightClick(object sender, EventArgs e)
        {
            confirmarSenhaVisivel = !confirmarSenhaVisivel;
            txtConfirmarSenha.PasswordChar = confirmarSenhaVisivel ? '\0' : '●';
        }

        private void btnCriarConta_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }
    }
}
