using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace ProjetoFinal
{
    internal class ClienteInfo
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public DateTime DataCriacao { get; set; }
        public int Pontos { get; set; }
    }

    internal class ClienteDashboardResumo
    {
        public ClienteMarcacaoInfo ProximaMarcacao { get; set; }
        public string ServicoFavorito { get; set; }
        public int ServicoFavoritoQtd { get; set; }
        public int Pontos { get; set; }
        public int MarcacoesMes { get; set; }
        public string HorasCuidadoMes { get; set; }
        public decimal TotalGastoMes { get; set; }
        public decimal VariacaoMes { get; set; }
        public int PromocoesDisponiveis { get; set; }
        public decimal TotalGastoAno { get; set; }
        public decimal MediaMensal { get; set; }
        public int FaturasPagasAno { get; set; }
        public decimal FaturasPendentes { get; set; }
    }

    internal class ClienteMarcacaoInfo
    {
        public int IdMarcacao { get; set; }
        public int? IdServico { get; set; }
        public int? IdProfissional { get; set; }
        public string Servico { get; set; }
        public string Profissional { get; set; }
        public string Estado { get; set; }
        public DateTime DataMarcacao { get; set; }
        public TimeSpan Hora { get; set; }
        public decimal Valor { get; set; }
        public int DuracaoMinutos { get; set; }
        public string ServicoFoto { get; set; }
        public string ProfissionalFoto { get; set; }
        public decimal AvaliacaoProfissional { get; set; }
        public decimal AvaliacaoServico { get; set; }
    }

    internal class CupaoInfo
    {
        public int IdCupao { get; set; }
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public int? IdServico { get; set; }
        public string NomeServico { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime DataValidade { get; set; }
    }

    internal static class ClienteRepository
    {
        public static readonly CultureInfo Pt = new CultureInfo("pt-PT");

        public static void EnsureSchema()
        {
            ProfissionalRepository.EnsureSchema();
            string sql = @"
IF COL_LENGTH('dbo.Usuarios', 'PontosCliente') IS NULL ALTER TABLE dbo.Usuarios ADD PontosCliente INT NOT NULL CONSTRAINT DF_Usuarios_PontosCliente DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'IdCupao') IS NULL ALTER TABLE dbo.Faturas ADD IdCupao INT NULL;
IF COL_LENGTH('dbo.Faturas', 'CodigoCupao') IS NULL ALTER TABLE dbo.Faturas ADD CodigoCupao NVARCHAR(40) NULL;
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NULL ALTER TABLE dbo.Faturas ADD ValorTotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_ValorTotal_Cliente DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Faturas') AND c.name='ValorTotal') ALTER TABLE dbo.Faturas ADD CONSTRAINT DF_Faturas_ValorTotal_Cliente_Default DEFAULT(0) FOR ValorTotal;
IF COL_LENGTH('dbo.Faturas','ValorTotal') IS NOT NULL EXEC(N'UPDATE dbo.Faturas SET ValorTotal=COALESCE(NULLIF(ValorTotal,0), Total, Subtotal, 0) WHERE ValorTotal IS NULL OR ValorTotal=0');
IF COL_LENGTH('dbo.Marcacoes', 'Avaliada') IS NULL ALTER TABLE dbo.Marcacoes ADD Avaliada BIT NOT NULL CONSTRAINT DF_Marcacoes_Avaliada DEFAULT(0);
IF OBJECT_ID('dbo.Avaliacoes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Avaliacoes
    (
        IdAvaliacao INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCliente INT NULL,
        IdProfissional INT NULL,
        IdServico INT NULL,
        IdMarcacao INT NULL,
        Classificacao DECIMAL(3,2) NULL,
        NotaProfissional DECIMAL(3,2) NULL,
        NotaEspaco DECIMAL(3,2) NULL,
        NotaServico DECIMAL(3,2) NULL,
        Comentario NVARCHAR(500) NULL,
        DataAvaliacao DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;
IF COL_LENGTH('dbo.Avaliacoes', 'IdMarcacao') IS NULL ALTER TABLE dbo.Avaliacoes ADD IdMarcacao INT NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'Classificacao') IS NULL ALTER TABLE dbo.Avaliacoes ADD Classificacao DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaProfissional') IS NULL ALTER TABLE dbo.Avaliacoes ADD NotaProfissional DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaEspaco') IS NULL ALTER TABLE dbo.Avaliacoes ADD NotaEspaco DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaServico') IS NULL ALTER TABLE dbo.Avaliacoes ADD NotaServico DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'Classificacao') IS NOT NULL ALTER TABLE dbo.Avaliacoes ALTER COLUMN Classificacao DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaProfissional') IS NOT NULL ALTER TABLE dbo.Avaliacoes ALTER COLUMN NotaProfissional DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaEspaco') IS NOT NULL ALTER TABLE dbo.Avaliacoes ALTER COLUMN NotaEspaco DECIMAL(3,2) NULL;
IF COL_LENGTH('dbo.Avaliacoes', 'NotaServico') IS NOT NULL ALTER TABLE dbo.Avaliacoes ALTER COLUMN NotaServico DECIMAL(3,2) NULL;
IF OBJECT_ID('dbo.Cupoes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cupoes
    (
        IdCupao INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Codigo NVARCHAR(40) NOT NULL UNIQUE,
        IdCliente INT NOT NULL,
        Tipo NVARCHAR(30) NOT NULL DEFAULT('Promocao'),
        IdServico INT NULL,
        NomeServico NVARCHAR(120) NULL,
        PercentualDesconto DECIMAL(5,2) NOT NULL DEFAULT(0),
        ValorDesconto DECIMAL(10,2) NOT NULL DEFAULT(0),
        DataCriacao DATETIME NOT NULL DEFAULT(GETDATE()),
        DataValidade DATETIME NOT NULL DEFAULT(DATEADD(DAY,30,GETDATE())),
        Usado BIT NOT NULL DEFAULT(0),
        DataUso DATETIME NULL
    );
END;
UPDATE f
SET f.IdProfissional = COALESCE(f.IdProfissional, m.IdProfissional),
    f.Profissional = COALESCE(NULLIF(f.Profissional,''), m.Profissional),
    f.HoraFatura = COALESCE(f.HoraFatura, m.Hora)
FROM dbo.Faturas f
LEFT JOIN dbo.Marcacoes m ON f.IdMarcacao = m.IdMarcacao
WHERE f.IdMarcacao IS NOT NULL AND (f.IdProfissional IS NULL OR f.Profissional IS NULL OR f.HoraFatura IS NULL);";
            ExecuteNonQuery(sql, null);
        }

        public static int ResolverIdCliente()
        {
            if (UsuarioLogado.Id > 0 && string.Equals(UsuarioLogado.Tipo, "Cliente", StringComparison.OrdinalIgnoreCase))
                return UsuarioLogado.Id;

            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 IdUsuario, Nome, Email, Foto FROM dbo.Usuarios WHERE TipoUsuario='Cliente' AND Ativo=1 ORDER BY IdUsuario", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        UsuarioLogado.Id = Convert.ToInt32(r["IdUsuario"]);
                        UsuarioLogado.Nome = ProfissionalRepository.ReadString(r, "Nome");
                        UsuarioLogado.Email = ProfissionalRepository.ReadString(r, "Email");
                        UsuarioLogado.Tipo = "Cliente";
                        UsuarioLogado.Foto = ProfissionalRepository.ReadString(r, "Foto");
                        return UsuarioLogado.Id;
                    }
                }
            }
            return 0;
        }

        public static ClienteInfo GetCliente(int idCliente)
        {
            if (idCliente <= 0) idCliente = ResolverIdCliente();
            GarantirPontosIniciais(idCliente);
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"SELECT IdUsuario, Nome, Email, Foto, DataCriacao, PontosCliente FROM dbo.Usuarios WHERE IdUsuario=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", idCliente);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new ClienteInfo
                        {
                            IdUsuario = Convert.ToInt32(r["IdUsuario"]),
                            Nome = ProfissionalRepository.ReadString(r, "Nome"),
                            Email = ProfissionalRepository.ReadString(r, "Email"),
                            Foto = ProfissionalRepository.ReadString(r, "Foto"),
                            DataCriacao = r["DataCriacao"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["DataCriacao"]),
                            Pontos = r["PontosCliente"] == DBNull.Value ? 0 : Convert.ToInt32(r["PontosCliente"])
                        };
                    }
                }
            }
            return new ClienteInfo { IdUsuario = idCliente, Nome = "Cliente", DataCriacao = DateTime.Today };
        }

        public static void GarantirPontosIniciais(int idCliente)
        {
            if (idCliente <= 0) return;
            string sql = @"
DECLARE @pontosAtuais INT = (SELECT ISNULL(PontosCliente,0) FROM dbo.Usuarios WHERE IdUsuario=@IdCliente);
DECLARE @pontosCalculados INT = (SELECT ISNULL(CAST(FLOOR(SUM(CASE WHEN Estado IN ('Paga','Pago','Concluído','Concluido') THEN Total ELSE 0 END) / 1.50) AS INT),0) FROM dbo.Faturas WHERE IdCliente=@IdCliente);
IF @pontosAtuais = 0 AND @pontosCalculados > 0
    UPDATE dbo.Usuarios SET PontosCliente=@pontosCalculados WHERE IdUsuario=@IdCliente;";
            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@IdCliente", idCliente } });
        }

        public static void GarantirNotificacoesIniciais(int idCliente)
        {
            if (idCliente <= 0) return;
            string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@IdCliente AND Mensagem LIKE 'Bem-vinda%cliente%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdCliente, 'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', GETDATE(), 0);
IF EXISTS (SELECT 1 FROM dbo.Marcacoes WHERE IdCliente=@IdCliente AND Estado IN ('Pendente','Confirmado'))
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@IdCliente AND Mensagem LIKE 'Tem uma marcação%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdCliente, 'Tem uma marcação agendada. Confira o horário para não se atrasar.', GETDATE(), 0);
IF EXISTS (SELECT 1 FROM dbo.Marcacoes WHERE IdCliente=@IdCliente AND Estado IN ('Concluído','Concluido') AND ISNULL(Avaliada,0)=0)
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@IdCliente AND Mensagem LIKE 'Avalie o seu último%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdCliente, 'Avalie o seu último atendimento e ajude outras clientes.', GETDATE(), 0);";
            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@IdCliente", idCliente } });
        }

        public static ClienteDashboardResumo GetDashboardResumo(int idCliente)
        {
            if (idCliente <= 0) idCliente = ResolverIdCliente();
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            DateTime inicioAno = new DateTime(hoje.Year, 1, 1);

            ClienteDashboardResumo res = new ClienteDashboardResumo();
            res.ProximaMarcacao = GetProximasMarcacoes(idCliente, 1).FirstOrDefault();
            ClienteInfo c = GetCliente(idCliente);
            res.Pontos = c.Pontos;
            res.PromocoesDisponiveis = 2;

            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                res.MarcacoesMes = ScalarInt(conn, "SELECT COUNT(*) FROM dbo.Marcacoes WHERE IdCliente=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim", idCliente, inicioMes, fimMes);
                int minutos = ScalarInt(conn, "SELECT ISNULL(SUM(DuracaoMinutos),0) FROM dbo.Marcacoes WHERE IdCliente=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim AND Estado NOT IN ('Cancelado','Cancelada')", idCliente, inicioMes, fimMes);
                res.HorasCuidadoMes = (minutos / 60).ToString("0") + "h" + (minutos % 60).ToString("00") + "min";
                res.TotalGastoMes = ScalarDecimal(conn, "SELECT ISNULL(SUM(Total),0) FROM dbo.Faturas WHERE IdCliente=@Id AND DataFatura>=@Ini AND DataFatura<@Fim AND Estado IN ('Paga','Pago')", idCliente, inicioMes, fimMes);
                DateTime mesAnt = inicioMes.AddMonths(-1);
                decimal anterior = ScalarDecimal(conn, "SELECT ISNULL(SUM(Total),0) FROM dbo.Faturas WHERE IdCliente=@Id AND DataFatura>=@Ini AND DataFatura<@Fim AND Estado IN ('Paga','Pago')", idCliente, mesAnt, inicioMes);
                res.VariacaoMes = res.TotalGastoMes - anterior;
                res.TotalGastoAno = ScalarDecimal(conn, "SELECT ISNULL(SUM(Total),0) FROM dbo.Faturas WHERE IdCliente=@Id AND DataFatura>=@Ini AND Estado IN ('Paga','Pago')", idCliente, inicioAno, DateTime.Today.AddDays(1));
                int meses = Math.Max(1, hoje.Month);
                res.MediaMensal = res.TotalGastoAno / meses;
                res.FaturasPagasAno = ScalarInt(conn, "SELECT COUNT(*) FROM dbo.Faturas WHERE IdCliente=@Id AND DataFatura>=@Ini AND Estado IN ('Paga','Pago')", idCliente, inicioAno, DateTime.Today.AddDays(1));
                res.FaturasPendentes = ScalarDecimal(conn, "SELECT ISNULL(SUM(Total),0) FROM dbo.Faturas WHERE IdCliente=@Id AND Estado IN ('Pendente','Não paga','Nao paga')", idCliente);

                using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 Servico, COUNT(*) AS Qtd FROM dbo.Marcacoes WHERE IdCliente=@Id GROUP BY Servico ORDER BY COUNT(*) DESC, Servico", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idCliente);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            res.ServicoFavorito = ProfissionalRepository.ReadString(r, "Servico");
                            res.ServicoFavoritoQtd = Convert.ToInt32(r["Qtd"]);
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(res.ServicoFavorito)) res.ServicoFavorito = "Ainda sem histórico";
            return res;
        }

        public static List<ClienteMarcacaoInfo> GetProximasMarcacoes(int idCliente, int limite)
        {
            string top = limite > 0 ? "TOP " + limite.ToString() : "";
            string sql = @"SELECT " + top + @" m.IdMarcacao, m.IdServico, m.IdProfissional, m.Servico, m.Profissional, m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.DuracaoMinutos,
                       s.Foto AS ServicoFoto, s.Avaliacao AS AvaliacaoServico, u.Foto AS ProfissionalFoto, u.Avaliacao AS AvaliacaoProfissional
                FROM dbo.Marcacoes m
                LEFT JOIN dbo.Servicos s ON m.IdServico=s.IdServico
                LEFT JOIN dbo.Usuarios u ON m.IdProfissional=u.IdUsuario
                WHERE m.IdCliente=@IdCliente AND m.Estado NOT IN ('Cancelado','Cancelada') AND (m.DataMarcacao > CAST(GETDATE() AS DATE) OR (m.DataMarcacao=CAST(GETDATE() AS DATE) AND m.Hora>=CAST(GETDATE() AS TIME)))
                ORDER BY m.DataMarcacao, m.Hora";
            return GetMarcacoesBySql(sql, new Dictionary<string, object> { { "@IdCliente", idCliente } });
        }

        public static List<ClienteMarcacaoInfo> GetMarcacoes(int idCliente, string pesquisa, string estado)
        {
            string sql = @"SELECT m.IdMarcacao, m.IdServico, m.IdProfissional, m.Servico, m.Profissional, m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.DuracaoMinutos,
                       s.Foto AS ServicoFoto, s.Avaliacao AS AvaliacaoServico, u.Foto AS ProfissionalFoto, u.Avaliacao AS AvaliacaoProfissional
                FROM dbo.Marcacoes m
                LEFT JOIN dbo.Servicos s ON m.IdServico=s.IdServico
                LEFT JOIN dbo.Usuarios u ON m.IdProfissional=u.IdUsuario
                WHERE m.IdCliente=@IdCliente
                  AND (@Pesquisa='' OR m.Servico LIKE @Like OR m.Profissional LIKE @Like OR m.Estado LIKE @Like OR CONVERT(NVARCHAR(10),m.DataMarcacao,103) LIKE @Like)
                  AND (@Estado='' OR @Estado='Todos' OR m.Estado=@Estado)
                ORDER BY m.DataMarcacao DESC, m.Hora DESC";
            return GetMarcacoesBySql(sql, new Dictionary<string, object> { { "@IdCliente", idCliente }, { "@Pesquisa", pesquisa ?? "" }, { "@Like", "%" + (pesquisa ?? "") + "%" }, { "@Estado", estado ?? "" } });
        }

        private static List<ClienteMarcacaoInfo> GetMarcacoesBySql(string sql, Dictionary<string, object> parametros)
        {
            List<ClienteMarcacaoInfo> lista = new List<ClienteMarcacaoInfo>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddParams(cmd, parametros);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new ClienteMarcacaoInfo
                        {
                            IdMarcacao = Convert.ToInt32(r["IdMarcacao"]),
                            IdServico = r["IdServico"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdServico"]),
                            IdProfissional = r["IdProfissional"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdProfissional"]),
                            Servico = ProfissionalRepository.ReadString(r, "Servico"),
                            Profissional = ProfissionalRepository.ReadString(r, "Profissional"),
                            DataMarcacao = Convert.ToDateTime(r["DataMarcacao"]),
                            Hora = (TimeSpan)r["Hora"],
                            Valor = ProfissionalRepository.ReadDecimal(r, "Valor", 0m),
                            Estado = ProfissionalRepository.ReadString(r, "Estado"),
                            DuracaoMinutos = ProfissionalRepository.ReadInt(r, "DuracaoMinutos", 60),
                            ServicoFoto = ProfissionalRepository.ReadString(r, "ServicoFoto"),
                            ProfissionalFoto = ProfissionalRepository.ReadString(r, "ProfissionalFoto"),
                            AvaliacaoProfissional = ProfissionalRepository.ReadDecimal(r, "AvaliacaoProfissional", 5m),
                            AvaliacaoServico = ProfissionalRepository.ReadDecimal(r, "AvaliacaoServico", 5m)
                        });
                    }
                }
            }
            return lista;
        }

        public static List<FaturaInfo> GetFaturas(int idCliente, string pesquisa, string estado)
        {
            List<FaturaInfo> lista = new List<FaturaInfo>();
            string sql = @"SELECT IdFatura, NumeroFatura, IdMarcacao, IdCliente, Cliente, IdProfissional, Profissional, DataFatura, HoraFatura, Servicos, Subtotal, Desconto, Total, ComissaoPercentual, MetodoPagamento, Estado
                           FROM dbo.Faturas
                           WHERE IdCliente=@IdCliente
                             AND (@Pesquisa='' OR NumeroFatura LIKE @Like OR Servicos LIKE @Like OR Estado LIKE @Like OR MetodoPagamento LIKE @Like)
                             AND (@Estado='' OR @Estado='Todos' OR Estado=@Estado)
                           ORDER BY DataFatura DESC, IdFatura DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@Pesquisa", pesquisa ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (pesquisa ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", estado ?? "");
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new FaturaInfo
                        {
                            IdFatura = Convert.ToInt32(r["IdFatura"]),
                            NumeroFatura = ProfissionalRepository.ReadString(r, "NumeroFatura"),
                            IdMarcacao = r["IdMarcacao"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdMarcacao"]),
                            IdCliente = r["IdCliente"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdCliente"]),
                            Cliente = ProfissionalRepository.ReadString(r, "Cliente"),
                            Profissional = ProfissionalRepository.ReadString(r, "Profissional"),
                            DataFatura = Convert.ToDateTime(r["DataFatura"]),
                            Hora = r["HoraFatura"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["HoraFatura"],
                            Servicos = ProfissionalRepository.ReadString(r, "Servicos"),
                            Subtotal = ProfissionalRepository.ReadDecimal(r, "Subtotal", 0m),
                            Desconto = ProfissionalRepository.ReadDecimal(r, "Desconto", 0m),
                            Total = ProfissionalRepository.ReadDecimal(r, "Total", 0m),
                            ComissaoPercentual = ProfissionalRepository.ReadDecimal(r, "ComissaoPercentual", 40m),
                            MetodoPagamento = ProfissionalRepository.ReadString(r, "MetodoPagamento"),
                            Estado = ProfissionalRepository.ReadString(r, "Estado")
                        });
                    }
                }
            }
            return lista;
        }

        public static List<ServicoInfo> GetServicosRecomendados(int idCliente, int limite)
        {
            List<ServicoInfo> lista = new List<ServicoInfo>();
            string top = limite > 0 ? "TOP " + limite.ToString() : "";
            string sql = @"SELECT " + top + @" IdServico, Nome, Categoria, DuracaoMinutos, Preco, Avaliacao, Foto
                           FROM dbo.Servicos
                           WHERE Ativo=1
                           ORDER BY CASE WHEN IdServico NOT IN (SELECT ISNULL(IdServico,0) FROM dbo.Marcacoes WHERE IdCliente=@IdCliente) THEN 0 ELSE 1 END, Popularidade DESC, Avaliacao DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new ServicoInfo
                        {
                            IdServico = Convert.ToInt32(r["IdServico"]),
                            Nome = ProfissionalRepository.ReadString(r, "Nome"),
                            Categoria = ProfissionalRepository.ReadString(r, "Categoria"),
                            DuracaoMinutos = ProfissionalRepository.ReadInt(r, "DuracaoMinutos", 60),
                            Preco = ProfissionalRepository.ReadDecimal(r, "Preco", 0m),
                            Avaliacao = ProfissionalRepository.ReadDecimal(r, "Avaliacao", 5m),
                            Foto = ProfissionalRepository.ReadString(r, "Foto")
                        });
                    }
                }
            }
            return lista;
        }

        public static List<ProfissionalInfo> GetProfissionais()
        {
            List<ProfissionalInfo> lista = new List<ProfissionalInfo>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdUsuario, Nome, Email, Foto, Especialidade, Avaliacao, ComissaoPercentual FROM dbo.Usuarios WHERE TipoUsuario='Profissional' AND Ativo=1 ORDER BY Avaliacao DESC, Nome", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new ProfissionalInfo
                        {
                            IdUsuario = Convert.ToInt32(r["IdUsuario"]),
                            Nome = ProfissionalRepository.ReadString(r, "Nome"),
                            Email = ProfissionalRepository.ReadString(r, "Email"),
                            Foto = ProfissionalRepository.ReadString(r, "Foto"),
                            Especialidade = ProfissionalRepository.ReadString(r, "Especialidade"),
                            Avaliacao = ProfissionalRepository.ReadDecimal(r, "Avaliacao", 5m),
                            ComissaoPercentual = ProfissionalRepository.ReadDecimal(r, "ComissaoPercentual", 40m)
                        });
                    }
                }
            }
            return lista;
        }

        public static CupaoInfo GetPromocaoMicroPigmentacao(int idCliente)
        {
            EnsureSchema();
            string sql = @"SELECT TOP 1 IdServico, Nome
                           FROM dbo.Servicos
                           WHERE Ativo=1
                             AND (
                                  Nome COLLATE Latin1_General_CI_AI LIKE N'%micro%pigment%'
                                  OR Nome COLLATE Latin1_General_CI_AI LIKE N'%micropigment%'
                                  OR Categoria COLLATE Latin1_General_CI_AI LIKE N'%micro%pigment%'
                                  OR Categoria COLLATE Latin1_General_CI_AI LIKE N'%micropigment%'
                             )
                           ORDER BY Popularidade DESC, Avaliacao DESC, Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new CupaoInfo
                        {
                            Codigo = "PROMO",
                            Tipo = "Promocao",
                            IdServico = Convert.ToInt32(r["IdServico"]),
                            NomeServico = ProfissionalRepository.ReadString(r, "Nome"),
                            PercentualDesconto = 35m,
                            DataValidade = DateTime.Today.AddDays(30)
                        };
                    }
                }
            }
            return null;
        }

        public static CupaoInfo GetPromocaoDoMes(int idCliente, int indice)
        {
            List<ServicoInfo> servicos = GetServicosRecomendados(idCliente, 0);
            if (servicos.Count == 0) return null;
            int pos = (DateTime.Today.Month + indice) % servicos.Count;
            ServicoInfo s = servicos[pos];
            decimal desconto = indice == 0 ? 20m : 15m;
            return new CupaoInfo { Codigo = "PROMO", Tipo = "Promocao", IdServico = s.IdServico, NomeServico = s.Nome, PercentualDesconto = desconto, DataValidade = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1) };
        }

        public static CupaoInfo GerarCupaoPromocao(int idCliente, int idServico, decimal percentual)
        {
            if (idCliente <= 0) throw new InvalidOperationException("Cliente inválido para gerar cupão.");
            if (idServico <= 0) throw new InvalidOperationException("Serviço inválido para esta promoção.");
            EnsureSchema();
            string codigo = "PROMO" + DateTime.Now.ToString("MMyy") + "-" + idCliente + "-" + idServico;
            ServicoInfo servico = ProfissionalRepository.GetServicos().FirstOrDefault(s => s.IdServico == idServico);
            string nome = servico == null ? "Serviço" : servico.Nome;
            string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Cupoes WHERE Codigo=@Codigo)
BEGIN
    INSERT INTO dbo.Cupoes (Codigo, IdCliente, Tipo, IdServico, NomeServico, PercentualDesconto, ValorDesconto, DataValidade, Usado)
    VALUES (@Codigo, @IdCliente, 'Promocao', @IdServico, @NomeServico, @Percentual, 0, DATEADD(DAY,30,GETDATE()), 0);
END;";
            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@Codigo", codigo }, { "@IdCliente", idCliente }, { "@IdServico", idServico }, { "@NomeServico", nome }, { "@Percentual", percentual } });
            CupaoInfo cupao = GetCupao(idCliente, codigo);
            if (cupao == null)
            {
                cupao = new CupaoInfo { Codigo = codigo, IdServico = idServico, NomeServico = nome, PercentualDesconto = percentual, Tipo = "Promocao", DataValidade = DateTime.Today.AddDays(30) };
            }
            return cupao;
        }

        public static CupaoInfo GerarCupaoPontos(int idCliente)
        {
            ClienteInfo c = GetCliente(idCliente);
            if (c.Pontos < 100) throw new InvalidOperationException("Precisa de pelo menos 100 pontos para resgatar um serviço gratuito.");
            string codigo = "PONTOS" + DateTime.Now.ToString("MMddHHmm") + idCliente;
            string sql = @"INSERT INTO dbo.Cupoes (Codigo, IdCliente, Tipo, PercentualDesconto, ValorDesconto, DataValidade, Usado)
                           VALUES (@Codigo, @IdCliente, 'Pontos', 100, 0, DATEADD(DAY,45,GETDATE()), 0);
                           UPDATE dbo.Usuarios SET PontosCliente=0 WHERE IdUsuario=@IdCliente;";
            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@Codigo", codigo }, { "@IdCliente", idCliente } });
            return GetCupao(idCliente, codigo);
        }

        public static CupaoInfo GetCupao(int idCliente, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 IdCupao, Codigo, Tipo, IdServico, NomeServico, PercentualDesconto, ValorDesconto, DataValidade
                                                    FROM dbo.Cupoes WHERE IdCliente=@IdCliente AND Codigo=@Codigo AND Usado=0 AND DataValidade>=GETDATE()", conn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@Codigo", codigo.Trim());
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new CupaoInfo
                        {
                            IdCupao = Convert.ToInt32(r["IdCupao"]),
                            Codigo = ProfissionalRepository.ReadString(r, "Codigo"),
                            Tipo = ProfissionalRepository.ReadString(r, "Tipo"),
                            IdServico = r["IdServico"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdServico"]),
                            NomeServico = ProfissionalRepository.ReadString(r, "NomeServico"),
                            PercentualDesconto = ProfissionalRepository.ReadDecimal(r, "PercentualDesconto", 0m),
                            ValorDesconto = ProfissionalRepository.ReadDecimal(r, "ValorDesconto", 0m),
                            DataValidade = Convert.ToDateTime(r["DataValidade"])
                        };
                    }
                }
            }
            return null;
        }

        public static decimal CalcularDescontoCupao(CupaoInfo cupao, List<ServicoInfo> servicos)
        {
            if (cupao == null || servicos == null || servicos.Count == 0) return 0m;
            decimal baseDesconto = 0m;
            if (cupao.IdServico.HasValue)
            {
                ServicoInfo escolhido = servicos.FirstOrDefault(s => s.IdServico == cupao.IdServico.Value);
                if (escolhido == null) throw new InvalidOperationException("Este cupão só pode ser usado no serviço: " + cupao.NomeServico + ".");
                baseDesconto = escolhido.Preco;
            }
            else
            {
                baseDesconto = servicos.Max(s => s.Preco);
            }
            decimal descontoPercentual = Math.Round(baseDesconto * cupao.PercentualDesconto / 100m, 2);
            return Math.Max(descontoPercentual, cupao.ValorDesconto);
        }

        public static int CriarMarcacaoCliente(int idCliente, int idProfissional, DateTime data, TimeSpan hora, List<ServicoInfo> servicos, string metodoPagamento, CupaoInfo cupao)
        {
            if (servicos == null || servicos.Count == 0) throw new InvalidOperationException("Escolha pelo menos um serviço.");
            ClienteInfo cliente = GetCliente(idCliente);
            ProfissionalInfo prof = ProfissionalRepository.GetProfissional(idProfissional);
            string nomesServicos = string.Join(", ", servicos.Select(s => s.Nome).ToArray());
            int duracao = servicos.Sum(s => s.DuracaoMinutos);
            decimal subtotal = servicos.Sum(s => s.Preco);
            decimal desconto = CalcularDescontoCupao(cupao, servicos);
            if (desconto > subtotal) desconto = subtotal;
            decimal total = subtotal - desconto;
            string estadoFatura = metodoPagamento == "Dinheiro" ? "Pendente" : "Paga";
            string estadoMarcacao = "Pendente";
            string numero = "FAT" + DateTime.Now.ToString("yyyyMMddHHmmss");
            int idMarcacao = 0;
            int idFatura = 0;
            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Marcacoes (IdCliente, IdProfissional, IdServico, Cliente, Profissional, Servico, DataMarcacao, Hora, Valor, Estado, Observacoes, DuracaoMinutos, IdEspaco, Espaco)
                                                            OUTPUT INSERTED.IdMarcacao
                                                            VALUES (@IdCliente, @IdProfissional, @IdServico, @Cliente, @Profissional, @Servico, @Data, @Hora, @Valor, @Estado, @Obs, @Duracao, 1, N'Sala 1')", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                        cmd.Parameters.AddWithValue("@IdServico", servicos[0].IdServico);
                        cmd.Parameters.AddWithValue("@Cliente", cliente.Nome);
                        cmd.Parameters.AddWithValue("@Profissional", prof.Nome);
                        cmd.Parameters.AddWithValue("@Servico", nomesServicos);
                        cmd.Parameters.AddWithValue("@Data", data.Date);
                        cmd.Parameters.AddWithValue("@Hora", hora);
                        cmd.Parameters.AddWithValue("@Valor", total);
                        cmd.Parameters.AddWithValue("@Estado", estadoMarcacao);
                        cmd.Parameters.AddWithValue("@Obs", cupao == null ? "" : "Cupão: " + cupao.Codigo);
                        cmd.Parameters.AddWithValue("@Duracao", duracao);
                        idMarcacao = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (ServicoInfo s in servicos)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.MarcacaoServicos (IdMarcacao, IdServico, NomeServico, DuracaoMinutos, Preco)
                                                                VALUES (@IdMarcacao, @IdServico, @Nome, @Duracao, @Preco)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                            cmd.Parameters.AddWithValue("@IdServico", s.IdServico);
                            cmd.Parameters.AddWithValue("@Nome", s.Nome);
                            cmd.Parameters.AddWithValue("@Duracao", s.DuracaoMinutos);
                            cmd.Parameters.AddWithValue("@Preco", s.Preco);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Faturas (NumeroFatura, IdMarcacao, IdCliente, Cliente, IdProfissional, Profissional, DataFatura, HoraFatura, Servicos, Subtotal, Desconto, Total, ValorTotal, MetodoPagamento, Estado, IdCupao, CodigoCupao)
                                                            OUTPUT INSERTED.IdFatura
                                                            VALUES (@Numero, @IdMarcacao, @IdCliente, @Cliente, @IdProfissional, @Profissional, @Data, @Hora, @Servicos, @Subtotal, @Desconto, @Total, @Total, @Metodo, @Estado, @IdCupao, @CodigoCupao)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Numero", numero);
                        cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@Cliente", cliente.Nome);
                        cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                        cmd.Parameters.AddWithValue("@Profissional", prof.Nome);
                        cmd.Parameters.AddWithValue("@Data", data.Date);
                        cmd.Parameters.AddWithValue("@Hora", hora);
                        cmd.Parameters.AddWithValue("@Servicos", nomesServicos);
                        cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@Desconto", desconto);
                        cmd.Parameters.AddWithValue("@Total", total);
                        cmd.Parameters.AddWithValue("@Metodo", metodoPagamento);
                        cmd.Parameters.AddWithValue("@Estado", estadoFatura);
                        cmd.Parameters.AddWithValue("@IdCupao", cupao == null ? (object)DBNull.Value : cupao.IdCupao);
                        cmd.Parameters.AddWithValue("@CodigoCupao", cupao == null ? (object)DBNull.Value : cupao.Codigo);
                        idFatura = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (cupao != null)
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Cupoes SET Usado=1, DataUso=GETDATE() WHERE IdCupao=@IdCupao", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdCupao", cupao.IdCupao);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (cupao != null && string.Equals(cupao.Tipo, "Pontos", StringComparison.OrdinalIgnoreCase))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Usuarios SET PontosCliente=0 WHERE IdUsuario=@IdCliente", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (estadoFatura == "Paga")
                    {
                        int pontos = (int)Math.Floor(total / 1.50m);
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Usuarios SET PontosCliente=PontosCliente+@Pontos WHERE IdUsuario=@IdCliente", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Pontos", pontos);
                            cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@IdCliente, @MsgCliente, GETDATE(), 0);
                                                            INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@IdProfissional, @MsgProf, GETDATE(), 0);", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                        cmd.Parameters.AddWithValue("@MsgCliente", "Marcação criada para " + data.ToString("dd/MM/yyyy") + " às " + hora.ToString(@"hh\:mm") + ".");
                        cmd.Parameters.AddWithValue("@MsgProf", "Nova marcação de " + cliente.Nome + " para " + data.ToString("dd/MM/yyyy") + " às " + hora.ToString(@"hh\:mm") + ".");
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            return idMarcacao;
        }

        public static void CancelarMarcacao(int idMarcacao, int idCliente)
        {
            ExecuteNonQuery("UPDATE dbo.Marcacoes SET Estado='Cancelado' WHERE IdMarcacao=@IdMarcacao AND IdCliente=@IdCliente", new Dictionary<string, object> { { "@IdMarcacao", idMarcacao }, { "@IdCliente", idCliente } });
        }

        public static ClienteMarcacaoInfo GetMarcacao(int idMarcacao)
        {
            string sql = @"SELECT TOP 1 m.IdMarcacao, m.IdServico, m.IdProfissional, m.Servico, m.Profissional, m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.DuracaoMinutos,
                       s.Foto AS ServicoFoto, s.Avaliacao AS AvaliacaoServico, u.Foto AS ProfissionalFoto, u.Avaliacao AS AvaliacaoProfissional
                FROM dbo.Marcacoes m
                LEFT JOIN dbo.Servicos s ON m.IdServico=s.IdServico
                LEFT JOIN dbo.Usuarios u ON m.IdProfissional=u.IdUsuario
                WHERE m.IdMarcacao=@IdMarcacao";
            return GetMarcacoesBySql(sql, new Dictionary<string, object> { { "@IdMarcacao", idMarcacao } }).FirstOrDefault();
        }

        public static void AtualizarMarcacoesFinalizadasParaAvaliacao(int idCliente)
        {
            if (idCliente <= 0) return;

            string sql = @"
UPDATE dbo.Marcacoes
SET Estado=N'Concluído'
WHERE IdCliente=@IdCliente
  AND ISNULL(Avaliada,0)=0
  AND Estado NOT IN (N'Cancelado', N'Cancelada', N'Concluído', N'Concluido')
  AND DATEADD(MINUTE,
        ISNULL(NULLIF(DuracaoMinutos,0),60),
        DATEADD(MINUTE,
            DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), ISNULL(Hora, CAST('00:00:00' AS TIME))),
            CAST(DataMarcacao AS DATETIME))) <= GETDATE();";

            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@IdCliente", idCliente } });
        }

        public static ClienteMarcacaoInfo GetAvaliacaoPendente(int idCliente)
        {
            AtualizarMarcacoesFinalizadasParaAvaliacao(idCliente);

            string sql = @"SELECT TOP 1 m.IdMarcacao, m.IdServico, m.IdProfissional, m.Servico, m.Profissional, m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.DuracaoMinutos,
                       s.Foto AS ServicoFoto, s.Avaliacao AS AvaliacaoServico, u.Foto AS ProfissionalFoto, u.Avaliacao AS AvaliacaoProfissional
                FROM dbo.Marcacoes m
                LEFT JOIN dbo.Servicos s ON m.IdServico=s.IdServico
                LEFT JOIN dbo.Usuarios u ON m.IdProfissional=u.IdUsuario
                WHERE m.IdCliente=@IdCliente
                  AND m.Estado IN (N'Concluído', N'Concluido')
                  AND ISNULL(m.Avaliada,0)=0
                  AND DATEADD(MINUTE,
                        ISNULL(NULLIF(m.DuracaoMinutos,0),60),
                        DATEADD(MINUTE,
                            DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), ISNULL(m.Hora, CAST('00:00:00' AS TIME))),
                            CAST(m.DataMarcacao AS DATETIME))) <= GETDATE()
                ORDER BY m.DataMarcacao DESC, m.Hora DESC";
            return GetMarcacoesBySql(sql, new Dictionary<string, object> { { "@IdCliente", idCliente } }).FirstOrDefault();
        }

        public static bool TemAvaliacaoPendente(int idCliente)
        {
            return GetAvaliacaoPendente(idCliente) != null;
        }

        public static void EnviarAvaliacao(int idCliente, int idMarcacao, decimal notaProfissional, decimal notaEspaco, decimal notaServico, string comentario)
        {
            if (notaProfissional <= 0 || notaEspaco <= 0 || notaServico <= 0)
                throw new InvalidOperationException("Avalie todos os itens antes de enviar.");
            ClienteMarcacaoInfo m = GetMarcacao(idMarcacao);
            if (m == null) throw new InvalidOperationException("Marcação não encontrada.");
            decimal media = Math.Round((notaProfissional + notaEspaco + notaServico) / 3m, 2);
            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Avaliacoes (IdCliente, IdProfissional, IdServico, IdMarcacao, Classificacao, NotaProfissional, NotaEspaco, NotaServico, Comentario, DataAvaliacao)
                                                            VALUES (@IdCliente, @IdProfissional, @IdServico, @IdMarcacao, @Classificacao, @NotaProf, @NotaEspaco, @NotaServico, @Comentario, GETDATE());
                                                            UPDATE dbo.Marcacoes SET Avaliada=1 WHERE IdMarcacao=@IdMarcacao AND IdCliente=@IdCliente;", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@IdProfissional", m.IdProfissional.HasValue ? (object)m.IdProfissional.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdServico", m.IdServico.HasValue ? (object)m.IdServico.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                        cmd.Parameters.AddWithValue("@Classificacao", media);
                        cmd.Parameters.AddWithValue("@NotaProf", notaProfissional);
                        cmd.Parameters.AddWithValue("@NotaEspaco", notaEspaco);
                        cmd.Parameters.AddWithValue("@NotaServico", notaServico);
                        cmd.Parameters.AddWithValue("@Comentario", string.IsNullOrWhiteSpace(comentario) ? (object)DBNull.Value : comentario);
                        cmd.ExecuteNonQuery();
                    }
                    if (m.IdProfissional.HasValue)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"UPDATE dbo.Usuarios SET Avaliacao=(SELECT CAST(AVG(CAST(NotaProfissional AS DECIMAL(5,2))) AS DECIMAL(3,2)) FROM dbo.Avaliacoes WHERE IdProfissional=@IdProfissional AND NotaProfissional IS NOT NULL) WHERE IdUsuario=@IdProfissional", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdProfissional", m.IdProfissional.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (m.IdServico.HasValue)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"UPDATE dbo.Servicos SET Avaliacao=(SELECT CAST(AVG(CAST(NotaServico AS DECIMAL(5,2))) AS DECIMAL(3,2)) FROM dbo.Avaliacoes WHERE IdServico=@IdServico AND NotaServico IS NOT NULL) WHERE IdServico=@IdServico", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdServico", m.IdServico.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public static List<SearchItem> PesquisarTudo(int idCliente, string termo)
        {
            List<SearchItem> lista = new List<SearchItem>();
            if (string.IsNullOrWhiteSpace(termo)) return lista;
            termo = termo.Trim();
            string like = "%" + termo + "%";
            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 5 Servico AS Titulo, CONVERT(NVARCHAR(10),DataMarcacao,103)+' · '+Estado AS Subtitulo, 'Marcação' AS Tipo FROM dbo.Marcacoes WHERE IdCliente=@Id AND (Servico LIKE @Like OR Profissional LIKE @Like OR Estado LIKE @Like)
                                                        UNION ALL
                                                        SELECT TOP 5 NumeroFatura, Servicos+' · '+Estado, 'Fatura' FROM dbo.Faturas WHERE IdCliente=@Id AND (NumeroFatura LIKE @Like OR Servicos LIKE @Like OR Estado LIKE @Like)
                                                        UNION ALL
                                                        SELECT TOP 5 Mensagem, CONVERT(NVARCHAR(16),DataNotificacao,120), 'Notificação' FROM dbo.Notificacoes WHERE IdUsuario=@Id AND Mensagem LIKE @Like
                                                        UNION ALL
                                                        SELECT TOP 5 Nome, Categoria+' · '+CAST(Preco AS NVARCHAR(30))+' €', 'Serviço' FROM dbo.Servicos WHERE Ativo=1 AND (Nome LIKE @Like OR Categoria LIKE @Like)", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idCliente);
                    cmd.Parameters.AddWithValue("@Like", like);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            lista.Add(new SearchItem { Titulo = ProfissionalRepository.ReadString(r, "Titulo"), Subtitulo = ProfissionalRepository.ReadString(r, "Subtitulo"), Tipo = ProfissionalRepository.ReadString(r, "Tipo") });
                        }
                    }
                }
            }
            return lista;
        }

        public static List<NotificacaoInfo> GetNotificacoes(int idCliente)
        {
            List<NotificacaoInfo> lista = new List<NotificacaoInfo>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 20 Id, Mensagem, DataNotificacao, Lida FROM dbo.Notificacoes WHERE IdUsuario=@IdUsuario ORDER BY Lida, DataNotificacao DESC", conn))
            {
                cmd.Parameters.AddWithValue("@IdUsuario", idCliente);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new NotificacaoInfo
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Mensagem = ProfissionalRepository.ReadString(r, "Mensagem"),
                            DataNotificacao = Convert.ToDateTime(r["DataNotificacao"]),
                            Lida = r["Lida"] != DBNull.Value && Convert.ToBoolean(r["Lida"])
                        });
                    }
                }
            }
            return lista;
        }

        public static int GetNotificacoesNaoLidas(int idCliente)
        {
            return ExecuteScalarInt("SELECT COUNT(*) FROM dbo.Notificacoes WHERE IdUsuario=@IdUsuario AND Lida=0", new Dictionary<string, object> { { "@IdUsuario", idCliente } });
        }

        public static void MarcarNotificacaoComoLida(int idNotificacao)
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE Id=@Id", new Dictionary<string, object> { { "@Id", idNotificacao } });
        }

        public static void MarcarTodasNotificacoesComoLidas(int idCliente)
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE IdUsuario=@IdUsuario", new Dictionary<string, object> { { "@IdUsuario", idCliente } });
        }

        public static void AtualizarFotoCliente(int idCliente, string caminhoFoto)
        {
            ExecuteNonQuery("UPDATE dbo.Usuarios SET Foto=@Foto WHERE IdUsuario=@Id", new Dictionary<string, object> { { "@Foto", caminhoFoto }, { "@Id", idCliente } });
            UsuarioLogado.Foto = caminhoFoto;
        }

        public static string FormatarMoeda(decimal valor)
        {
            return valor.ToString("N2", Pt) + " €";
        }

        private static int ExecuteScalarInt(string sql, Dictionary<string, object> parametros)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddParams(cmd, parametros);
                conn.Open();
                object obj = cmd.ExecuteScalar();
                return obj == DBNull.Value || obj == null ? 0 : Convert.ToInt32(obj);
            }
        }

        private static int ScalarInt(SqlConnection conn, string sql, int id, DateTime ini, DateTime fim)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Ini", ini);
                cmd.Parameters.AddWithValue("@Fim", fim);
                object obj = cmd.ExecuteScalar();
                return obj == DBNull.Value || obj == null ? 0 : Convert.ToInt32(obj);
            }
        }


        private static decimal ScalarDecimal(SqlConnection conn, string sql, int id)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                object obj = cmd.ExecuteScalar();
                return obj == DBNull.Value || obj == null ? 0m : Convert.ToDecimal(obj);
            }
        }

        private static decimal ScalarDecimal(SqlConnection conn, string sql, int id, DateTime ini, DateTime fim)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Ini", ini);
                cmd.Parameters.AddWithValue("@Fim", fim);
                object obj = cmd.ExecuteScalar();
                return obj == DBNull.Value || obj == null ? 0m : Convert.ToDecimal(obj);
            }
        }

        private static void ExecuteNonQuery(string sql, Dictionary<string, object> parametros)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddParams(cmd, parametros);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void AddParams(SqlCommand cmd, Dictionary<string, object> parametros)
        {
            if (parametros == null) return;
            foreach (KeyValuePair<string, object> p in parametros)
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }
    }
}
