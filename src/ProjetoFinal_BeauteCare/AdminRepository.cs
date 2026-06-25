using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoFinal
{
    internal class AdminInfo
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Foto { get; set; }
    }

    internal class AdminCliente
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; }
        public DateTime? UltimaVisita { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Foto { get; set; }
        public int TotalServicos { get; set; }
        public decimal TotalGasto { get; set; }
        public int MarcacoesMes { get; set; }
        public string UltimoServico { get; set; }
        public DateTime? UltimaMarcacaoData { get; set; }
        public TimeSpan? UltimaMarcacaoHora { get; set; }
    }

    internal class AdminProfissional
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Especialidade { get; set; }
        public bool Ativo { get; set; }
        public string Foto { get; set; }
        public decimal Avaliacao { get; set; }
        public decimal ComissaoPercentual { get; set; }
        public int Servicos { get; set; }
        public int AtendimentosMes { get; set; }
        public decimal FaturacaoMes { get; set; }
    }

    internal class AdminServico
    {
        public int IdServico { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; }
        public int Popularidade { get; set; }
        public string Descricao { get; set; }
        public string Foto { get; set; }
        public decimal Avaliacao { get; set; }
        public int TotalMarcacoes { get; set; }
        public decimal TotalFaturado { get; set; }
    }

    internal class AdminMarcacao
    {
        public int IdMarcacao { get; set; }
        public int? IdCliente { get; set; }
        public int? IdProfissional { get; set; }
        public int? IdServico { get; set; }
        public string Cliente { get; set; }
        public string ClienteFoto { get; set; }
        public string Profissional { get; set; }
        public string Servico { get; set; }
        public DateTime DataMarcacao { get; set; }
        public TimeSpan Hora { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal Valor { get; set; }
        public string Estado { get; set; }
        public string Observacoes { get; set; }
    }

    internal class AdminFatura
    {
        public int IdFatura { get; set; }
        public string NumeroFatura { get; set; }
        public int? IdMarcacao { get; set; }
        public int? IdCliente { get; set; }
        public string Cliente { get; set; }
        public DateTime DataFatura { get; set; }
        public string Servicos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }
        public string MetodoPagamento { get; set; }
        public string Estado { get; set; }
    }

    internal class AdminSearchItem
    {
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
    }

    internal class AdminOpcao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public override string ToString() { return string.IsNullOrWhiteSpace(Tipo) ? Nome : Nome + "  •  " + Tipo; }
    }

    internal static class AdminRepository
    {
        public static readonly CultureInfo Pt = new CultureInfo("pt-PT");

        public static void EnsureSchema()
        {
            ProfissionalRepository.EnsureSchema();
            ClienteRepository.EnsureSchema();
            string sql = @"
IF COL_LENGTH('dbo.Usuarios', 'Foto') IS NULL ALTER TABLE dbo.Usuarios ADD Foto NVARCHAR(400) NULL;
IF COL_LENGTH('dbo.Usuarios', 'Especialidade') IS NULL ALTER TABLE dbo.Usuarios ADD Especialidade NVARCHAR(120) NULL;
IF COL_LENGTH('dbo.Usuarios', 'Avaliacao') IS NULL ALTER TABLE dbo.Usuarios ADD Avaliacao DECIMAL(3,2) NOT NULL CONSTRAINT DF_Usuarios_Avaliacao_Admin DEFAULT(5);
IF COL_LENGTH('dbo.Usuarios', 'ComissaoPercentual') IS NULL ALTER TABLE dbo.Usuarios ADD ComissaoPercentual DECIMAL(5,2) NOT NULL CONSTRAINT DF_Usuarios_ComissaoPercentual_Admin DEFAULT(40);
IF COL_LENGTH('dbo.Servicos', 'Foto') IS NULL ALTER TABLE dbo.Servicos ADD Foto NVARCHAR(400) NULL;
IF COL_LENGTH('dbo.Servicos', 'Avaliacao') IS NULL ALTER TABLE dbo.Servicos ADD Avaliacao DECIMAL(3,2) NOT NULL CONSTRAINT DF_Servicos_Avaliacao_Admin DEFAULT(5);
IF COL_LENGTH('dbo.Servicos', 'Excluido') IS NULL ALTER TABLE dbo.Servicos ADD Excluido BIT NOT NULL CONSTRAINT DF_Servicos_Excluido_Admin DEFAULT(0);
IF OBJECT_ID('dbo.Espacos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Espacos
    (
        IdEspaco INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nome NVARCHAR(120) NOT NULL,
        Descricao NVARCHAR(300) NULL,
        Ativo BIT NOT NULL DEFAULT(1)
    );
END;
IF NOT EXISTS (SELECT 1 FROM dbo.Espacos WHERE IdEspaco=1)
BEGIN
    SET IDENTITY_INSERT dbo.Espacos ON;
    INSERT INTO dbo.Espacos (IdEspaco, Nome, Descricao, Ativo) VALUES (1, N'Sala 1', N'Sala padrão', 1);
    SET IDENTITY_INSERT dbo.Espacos OFF;
END;
IF COL_LENGTH('dbo.Marcacoes', 'DuracaoMinutos') IS NULL ALTER TABLE dbo.Marcacoes ADD DuracaoMinutos INT NOT NULL CONSTRAINT DF_Marcacoes_DuracaoMinutos_Admin DEFAULT(60);
IF COL_LENGTH('dbo.Marcacoes', 'IdEspaco') IS NULL ALTER TABLE dbo.Marcacoes ADD IdEspaco INT NOT NULL CONSTRAINT DF_Marcacoes_IdEspaco_Admin_Default DEFAULT(1);
IF COL_LENGTH('dbo.Marcacoes', 'Espaco') IS NULL ALTER TABLE dbo.Marcacoes ADD Espaco NVARCHAR(80) NOT NULL CONSTRAINT DF_Marcacoes_Espaco_Admin_Default DEFAULT(N'Sala 1');
IF COL_LENGTH('dbo.Marcacoes', 'IdEspaco') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Marcacoes') AND c.name='IdEspaco') ALTER TABLE dbo.Marcacoes ADD CONSTRAINT DF_Marcacoes_IdEspaco_Admin DEFAULT(1) FOR IdEspaco;
IF COL_LENGTH('dbo.Marcacoes', 'Espaco') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Marcacoes') AND c.name='Espaco') ALTER TABLE dbo.Marcacoes ADD CONSTRAINT DF_Marcacoes_Espaco_Admin DEFAULT(N'Sala 1') FOR Espaco;
IF COL_LENGTH('dbo.Marcacoes','IdEspaco') IS NOT NULL EXEC(N'UPDATE dbo.Marcacoes SET IdEspaco=1 WHERE IdEspaco IS NULL');
IF COL_LENGTH('dbo.Marcacoes','Espaco') IS NOT NULL EXEC(N'UPDATE dbo.Marcacoes SET Espaco=N''Sala 1'' WHERE Espaco IS NULL OR LTRIM(RTRIM(Espaco))=N''''');
IF COL_LENGTH('dbo.Notificacoes', 'IdUsuario') IS NULL ALTER TABLE dbo.Notificacoes ADD IdUsuario INT NULL;
IF COL_LENGTH('dbo.Faturas', 'IdProfissional') IS NULL ALTER TABLE dbo.Faturas ADD IdProfissional INT NULL;
IF COL_LENGTH('dbo.Faturas', 'Profissional') IS NULL ALTER TABLE dbo.Faturas ADD Profissional NVARCHAR(120) NULL;
IF COL_LENGTH('dbo.Faturas', 'ComissaoPercentual') IS NULL ALTER TABLE dbo.Faturas ADD ComissaoPercentual DECIMAL(5,2) NOT NULL CONSTRAINT DF_Faturas_ComissaoPercentual_Admin DEFAULT(40);
IF COL_LENGTH('dbo.Faturas', 'HoraFatura') IS NULL ALTER TABLE dbo.Faturas ADD HoraFatura TIME NULL;
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NULL ALTER TABLE dbo.Faturas ADD ValorTotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_ValorTotal_Admin DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Faturas') AND c.name='ValorTotal') ALTER TABLE dbo.Faturas ADD CONSTRAINT DF_Faturas_ValorTotal_Admin_Default DEFAULT(0) FOR ValorTotal;
IF COL_LENGTH('dbo.Faturas','ValorTotal') IS NOT NULL EXEC(N'UPDATE dbo.Faturas SET ValorTotal=COALESCE(NULLIF(ValorTotal,0), Total, Subtotal, 0) WHERE ValorTotal IS NULL OR ValorTotal=0');
IF OBJECT_ID('dbo.MensagensEnviadas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MensagensEnviadas
    (
        IdMensagem INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdRemetente INT NULL,
        IdDestinatario INT NULL,
        DestinatarioNome NVARCHAR(120) NULL,
        Canal NVARCHAR(30) NOT NULL DEFAULT('App'),
        Assunto NVARCHAR(160) NULL,
        Mensagem NVARCHAR(700) NOT NULL,
        DataEnvio DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;
IF COL_LENGTH('dbo.MensagensEnviadas', 'EstadoEnvio') IS NULL ALTER TABLE dbo.MensagensEnviadas ADD EstadoEnvio NVARCHAR(60) NULL;
IF COL_LENGTH('dbo.MensagensEnviadas', 'ErroEnvio') IS NULL ALTER TABLE dbo.MensagensEnviadas ADD ErroEnvio NVARCHAR(500) NULL;
IF OBJECT_ID('dbo.Importacoes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Importacoes
    (
        IdImportacao INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Tipo NVARCHAR(60) NOT NULL,
        Ficheiro NVARCHAR(400) NULL,
        LinhasImportadas INT NOT NULL DEFAULT(0),
        DataImportacao DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;
UPDATE dbo.Marcacoes
SET DuracaoMinutos = COALESCE(NULLIF(DuracaoMinutos,0), (SELECT TOP 1 DuracaoMinutos FROM dbo.Servicos s WHERE s.IdServico=dbo.Marcacoes.IdServico), 60)
WHERE DuracaoMinutos IS NULL OR DuracaoMinutos = 0;
UPDATE dbo.Marcacoes
SET Valor = COALESCE(NULLIF(Valor,0), (SELECT TOP 1 Preco FROM dbo.Servicos s WHERE s.IdServico=dbo.Marcacoes.IdServico), 0)
WHERE Valor IS NULL OR Valor = 0;
UPDATE u
SET UltimaVisita = x.Ultima
FROM dbo.Usuarios u
CROSS APPLY (SELECT MAX(CAST(m.DataMarcacao AS DATETIME)) AS Ultima FROM dbo.Marcacoes m WHERE m.IdCliente=u.IdUsuario AND m.Estado IN ('Concluído','Concluido','Paga','Pago','Confirmado')) x
WHERE u.TipoUsuario='Cliente' AND x.Ultima IS NOT NULL AND (u.UltimaVisita IS NULL OR u.UltimaVisita < x.Ultima);";
            ExecuteNonQuery(sql, null);
            GarantirNotificacoesAutomaticas();
        }

        public static int ResolverIdAdmin()
        {
            if (UsuarioLogado.Id > 0 && string.Equals(UsuarioLogado.Tipo, "Admin", StringComparison.OrdinalIgnoreCase)) return UsuarioLogado.Id;
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 IdUsuario, Nome, Email, Foto FROM dbo.Usuarios WHERE TipoUsuario='Admin' AND Ativo=1 ORDER BY IdUsuario", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        UsuarioLogado.Id = ReadInt(r, "IdUsuario");
                        UsuarioLogado.Nome = ReadString(r, "Nome");
                        UsuarioLogado.Email = ReadString(r, "Email");
                        UsuarioLogado.Tipo = "Admin";
                        UsuarioLogado.Foto = ReadString(r, "Foto");
                        return UsuarioLogado.Id;
                    }
                }
            }
            return 0;
        }

        public static AdminInfo GetAdmin(int id)
        {
            if (id <= 0) id = ResolverIdAdmin();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 IdUsuario, Nome, Email, Telefone, Foto FROM dbo.Usuarios WHERE IdUsuario=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new AdminInfo { IdUsuario = ReadInt(r, "IdUsuario"), Nome = ReadString(r, "Nome"), Email = ReadString(r, "Email"), Telefone = ReadString(r, "Telefone"), Foto = ReadString(r, "Foto") };
                    }
                }
            }
            return new AdminInfo { IdUsuario = id, Nome = "Administrador", Email = "" };
        }

        public static void AtualizarFotoAdmin(int idAdmin, string caminho)
        {
            ExecuteNonQuery("UPDATE dbo.Usuarios SET Foto=@Foto WHERE IdUsuario=@Id", new Dictionary<string, object> { { "@Foto", caminho }, { "@Id", idAdmin } });
            UsuarioLogado.Foto = caminho;
        }

        public static void GarantirNotificacoesAutomaticas()
        {
            string sql = @"
DECLARE @AdminId INT = (SELECT TOP 1 IdUsuario FROM dbo.Usuarios WHERE TipoUsuario='Admin' AND Ativo=1 ORDER BY IdUsuario);
IF @AdminId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Marcacoes WHERE DataMarcacao=CAST(GETDATE() AS DATE) AND Estado IN ('Pendente','Confirmado'))
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@AdminId AND Mensagem LIKE 'Agenda de hoje%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@AdminId, 'Agenda de hoje pronta para acompanhar.', GETDATE(), 0);
IF @AdminId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Marcacoes WHERE Estado='Pendente')
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@AdminId AND Mensagem LIKE 'Há marcações pendentes%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@AdminId, 'Há marcações pendentes para confirmar.', GETDATE(), 0);
IF @AdminId IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Usuarios WHERE TipoUsuario='Cliente' AND Ativo=1 AND (UltimaVisita IS NULL OR UltimaVisita < DATEADD(DAY,-30,GETDATE())))
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@AdminId AND Mensagem LIKE 'Clientes inativos%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@AdminId, 'Clientes inativos precisam de contacto.', GETDATE(), 0);";
            ExecuteNonQuery(sql, null);
        }

        public static List<NotificacaoInfo> GetNotificacoesAdmin()
        {
            List<NotificacaoInfo> lista = new List<NotificacaoInfo>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 30 Id, Mensagem, DataNotificacao, Lida FROM dbo.Notificacoes WHERE IdUsuario=@Admin ORDER BY Lida, DataNotificacao DESC", conn))
            {
                cmd.Parameters.AddWithValue("@Admin", ResolverIdAdmin());
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(new NotificacaoInfo { Id = ReadInt(r, "Id"), Mensagem = ReadString(r, "Mensagem"), DataNotificacao = ReadDateTime(r, "DataNotificacao", DateTime.Now), Lida = ReadBool(r, "Lida") });
                }
            }
            return lista;
        }

        public static int GetNotificacoesNaoLidasAdmin()
        {
            return ScalarInt("SELECT COUNT(*) FROM dbo.Notificacoes WHERE Lida=0 AND IdUsuario=@Admin", new Dictionary<string, object> { { "@Admin", ResolverIdAdmin() } });
        }

        public static void MarcarNotificacaoLida(int id)
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE Id=@Id", new Dictionary<string, object> { { "@Id", id } });
        }

        public static int CountClientes(bool? ativo)
        {
            string sql = "SELECT COUNT(*) FROM dbo.Usuarios WHERE TipoUsuario='Cliente'" + (ativo.HasValue ? " AND Ativo=@Ativo" : "");
            Dictionary<string, object> p = ativo.HasValue ? new Dictionary<string, object> { { "@Ativo", ativo.Value } } : null;
            return ScalarInt(sql, p);
        }

        public static int CountProfissionais(bool? ativo)
        {
            string sql = "SELECT COUNT(*) FROM dbo.Usuarios WHERE TipoUsuario='Profissional'" + (ativo.HasValue ? " AND Ativo=@Ativo" : "");
            Dictionary<string, object> p = ativo.HasValue ? new Dictionary<string, object> { { "@Ativo", ativo.Value } } : null;
            return ScalarInt(sql, p);
        }

        public static int CountServicos(bool? ativo)
        {
            string sql = "SELECT COUNT(*) FROM dbo.Servicos WHERE ISNULL(Excluido,0)=0" + (ativo.HasValue ? " AND Ativo=@Ativo" : "");
            Dictionary<string, object> p = ativo.HasValue ? new Dictionary<string, object> { { "@Ativo", ativo.Value } } : null;
            return ScalarInt(sql, p);
        }

        public static int CountMarcacoes(DateTime? inicio, DateTime? fim, string estado)
        {
            string sql = "SELECT COUNT(*) FROM dbo.Marcacoes WHERE 1=1";
            Dictionary<string, object> p = new Dictionary<string, object>();
            AddDateAndEstado(ref sql, p, "DataMarcacao", inicio, fim, estado);
            return ScalarInt(sql, p);
        }

        public static decimal SumFaturas(DateTime? inicio, DateTime? fim, string estado)
        {
            string sql = "SELECT ISNULL(SUM(Total),0) FROM dbo.Faturas WHERE 1=1";
            Dictionary<string, object> p = new Dictionary<string, object>();
            AddDateAndEstado(ref sql, p, "DataFatura", inicio, fim, estado);
            return ScalarDecimal(sql, p);
        }

        public static decimal SumMarcacoes(DateTime? inicio, DateTime? fim, string estado)
        {
            string sql = "SELECT ISNULL(SUM(Valor),0) FROM dbo.Marcacoes WHERE 1=1";
            Dictionary<string, object> p = new Dictionary<string, object>();
            AddDateAndEstado(ref sql, p, "DataMarcacao", inicio, fim, estado);
            return ScalarDecimal(sql, p);
        }

        public static int CountNewClientesMes()
        {
            return ScalarInt("SELECT COUNT(*) FROM dbo.Usuarios WHERE TipoUsuario='Cliente' AND Ativo=1 AND DataCriacao >= DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)", null);
        }

        public static List<AdminCliente> GetClientes(string termo, string estado, int limite)
        {
            List<AdminCliente> lista = new List<AdminCliente>();
            string sql = @"
SELECT TOP (@Limite) u.IdUsuario, u.Nome, u.Email, u.Telefone, u.Ativo, u.UltimaVisita, u.DataCriacao, u.Foto,
       ISNULL(x.TotalServicos,0) AS TotalServicos, ISNULL(x.TotalGasto,0) AS TotalGasto, ISNULL(x.MarcacoesMes,0) AS MarcacoesMes,
       y.Servico AS UltimoServico, y.DataMarcacao AS UltimaMarcacaoData, y.Hora AS UltimaMarcacaoHora
FROM dbo.Usuarios u
OUTER APPLY (
    SELECT COUNT(*) TotalServicos,
           SUM(CASE WHEN f.Estado IN ('Paga','Pago','Concluído','Concluido') THEN f.Total ELSE 0 END) TotalGasto,
           SUM(CASE WHEN m.DataMarcacao >= DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) THEN 1 ELSE 0 END) MarcacoesMes
    FROM dbo.Marcacoes m LEFT JOIN dbo.Faturas f ON f.IdMarcacao=m.IdMarcacao OR f.IdCliente=u.IdUsuario
    WHERE m.IdCliente=u.IdUsuario
) x
OUTER APPLY (
    SELECT TOP 1 Servico, DataMarcacao, Hora FROM dbo.Marcacoes m WHERE m.IdCliente=u.IdUsuario ORDER BY DataMarcacao DESC, Hora DESC
) y
WHERE u.TipoUsuario='Cliente'
  AND (@Termo='' OR u.Nome LIKE @Like OR u.Email LIKE @Like OR u.Telefone LIKE @Like)
  AND (@Estado='Todos' OR (@Estado='Ativo' AND u.Ativo=1) OR (@Estado='Inativo' AND u.Ativo=0))
ORDER BY u.Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 1000 : limite);
                cmd.Parameters.AddWithValue("@Termo", termo ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (termo ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(ReadCliente(r));
                }
            }
            return lista;
        }

        public static List<AdminCliente> GetClientesInativos(int limite)
        {
            List<AdminCliente> lista = new List<AdminCliente>();
            string sql = @"
SELECT TOP (@Limite) u.IdUsuario, u.Nome, u.Email, u.Telefone, u.Ativo, u.UltimaVisita, u.DataCriacao, u.Foto,
       ISNULL(x.TotalServicos,0) AS TotalServicos, ISNULL(x.TotalGasto,0) AS TotalGasto, 0 AS MarcacoesMes,
       y.Servico AS UltimoServico, y.DataMarcacao AS UltimaMarcacaoData, y.Hora AS UltimaMarcacaoHora
FROM dbo.Usuarios u
OUTER APPLY (SELECT COUNT(*) TotalServicos, SUM(CASE WHEN f.Estado IN ('Paga','Pago') THEN f.Total ELSE 0 END) TotalGasto FROM dbo.Faturas f WHERE f.IdCliente=u.IdUsuario) x
OUTER APPLY (SELECT TOP 1 Servico, DataMarcacao, Hora FROM dbo.Marcacoes m WHERE m.IdCliente=u.IdUsuario ORDER BY DataMarcacao DESC, Hora DESC) y
WHERE u.TipoUsuario='Cliente' AND u.Ativo=1 AND (u.UltimaVisita IS NULL OR u.UltimaVisita < DATEADD(DAY,-30,GETDATE()))
ORDER BY CASE WHEN u.UltimaVisita IS NULL THEN 0 ELSE 1 END, u.UltimaVisita";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 3 : limite);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(ReadCliente(r));
                }
            }
            return lista;
        }

        public static AdminCliente GetCliente(int id)
        {
            List<AdminCliente> all = GetClientes("", "Todos", 5000);
            foreach (AdminCliente c in all) if (c.IdUsuario == id) return c;
            return null;
        }

        public static List<AdminProfissional> GetProfissionais(string termo, string estado, int limite)
        {
            List<AdminProfissional> lista = new List<AdminProfissional>();
            string sql = @"
SELECT TOP (@Limite) u.IdUsuario, u.Nome, u.Email, u.Telefone, u.Especialidade, u.Ativo, u.Foto, u.Avaliacao, u.ComissaoPercentual,
       ISNULL(x.Servicos,0) AS Servicos, ISNULL(x.AtendimentosMes,0) AS AtendimentosMes, ISNULL(x.FaturacaoMes,0) AS FaturacaoMes
FROM dbo.Usuarios u
OUTER APPLY (
    SELECT COUNT(*) Servicos,
           SUM(CASE WHEN m.DataMarcacao >= DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) THEN 1 ELSE 0 END) AtendimentosMes,
           SUM(CASE WHEN m.DataMarcacao >= DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) THEN m.Valor ELSE 0 END) FaturacaoMes
    FROM dbo.Marcacoes m WHERE m.IdProfissional=u.IdUsuario
) x
WHERE u.TipoUsuario='Profissional'
  AND (@Termo='' OR u.Nome LIKE @Like OR u.Email LIKE @Like OR u.Telefone LIKE @Like OR u.Especialidade LIKE @Like)
  AND (@Estado='Todos' OR (@Estado='Ativo' AND u.Ativo=1) OR (@Estado='Inativo' AND u.Ativo=0))
ORDER BY u.Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 1000 : limite);
                cmd.Parameters.AddWithValue("@Termo", termo ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (termo ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AdminProfissional
                        {
                            IdUsuario = ReadInt(r, "IdUsuario"), Nome = ReadString(r, "Nome"), Email = ReadString(r, "Email"), Telefone = ReadString(r, "Telefone"), Especialidade = ReadString(r, "Especialidade"), Ativo = ReadBool(r, "Ativo"), Foto = ReadString(r, "Foto"), Avaliacao = ReadDecimal(r, "Avaliacao", 5), ComissaoPercentual = ReadDecimal(r, "ComissaoPercentual", 40), Servicos = ReadInt(r, "Servicos"), AtendimentosMes = ReadInt(r, "AtendimentosMes"), FaturacaoMes = ReadDecimal(r, "FaturacaoMes", 0)
                        });
                    }
                }
            }
            return lista;
        }

        public static List<AdminProfissional> GetTopProfissionais(int limite)
        {
            List<AdminProfissional> lista = GetProfissionais("", "Ativo", 5000);
            lista.Sort(delegate (AdminProfissional a, AdminProfissional b)
            {
                int cmp = b.Servicos.CompareTo(a.Servicos);
                if (cmp != 0) return cmp;
                cmp = b.Avaliacao.CompareTo(a.Avaliacao);
                if (cmp != 0) return cmp;
                return b.AtendimentosMes.CompareTo(a.AtendimentosMes);
            });
            if (limite > 0 && lista.Count > limite) lista = lista.GetRange(0, limite);
            return lista;
        }

        public static List<AdminServico> GetServicos(string termo, string estado, int limite)
        {
            List<AdminServico> lista = new List<AdminServico>();
            string sql = @"
SELECT TOP (@Limite) s.IdServico, s.Nome, s.Categoria, s.DuracaoMinutos, s.Preco, s.Ativo, s.Popularidade, s.Descricao, s.Foto,
       CAST(ISNULL(av.MediaAvaliacao, s.Avaliacao) AS DECIMAL(5,2)) AS Avaliacao,
       ISNULL(x.TotalMarcacoes,0) AS TotalMarcacoes, ISNULL(x.TotalFaturado,0) AS TotalFaturado
FROM dbo.Servicos s
OUTER APPLY (
    SELECT COUNT(*) TotalMarcacoes, ISNULL(SUM(m.Valor),0) TotalFaturado
    FROM dbo.Marcacoes m
    WHERE (m.IdServico=s.IdServico OR m.Servico=s.Nome)
      AND ISNULL(m.Estado,'') NOT IN ('Cancelado','Cancelada','Cacelado')
) x
OUTER APPLY (
    SELECT CAST(AVG(CAST(COALESCE(a.NotaServico, a.Classificacao) AS DECIMAL(5,2))) AS DECIMAL(5,2)) MediaAvaliacao
    FROM dbo.Avaliacoes a
    WHERE a.IdServico=s.IdServico
      AND COALESCE(a.NotaServico, a.Classificacao) IS NOT NULL
) av
WHERE ISNULL(s.Excluido,0)=0
  AND (@Termo='' OR s.Nome LIKE @Like OR s.Categoria LIKE @Like OR s.Descricao LIKE @Like)
  AND (@Estado='Todos' OR (@Estado='Ativo' AND s.Ativo=1) OR (@Estado='Inativo' AND s.Ativo=0))
ORDER BY s.Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 1000 : limite);
                cmd.Parameters.AddWithValue("@Termo", termo ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (termo ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AdminServico
                        {
                            IdServico = ReadInt(r, "IdServico"), Nome = ReadString(r, "Nome"), Categoria = ReadString(r, "Categoria"), DuracaoMinutos = ReadInt(r, "DuracaoMinutos"), Preco = ReadDecimal(r, "Preco", 0), Ativo = ReadBool(r, "Ativo"), Popularidade = ReadInt(r, "Popularidade"), Descricao = ReadString(r, "Descricao"), Foto = ReadString(r, "Foto"), Avaliacao = ReadDecimal(r, "Avaliacao", 5), TotalMarcacoes = ReadInt(r, "TotalMarcacoes"), TotalFaturado = ReadDecimal(r, "TotalFaturado", 0)
                        });
                    }
                }
            }
            return lista;
        }

        public static List<AdminServico> GetTopServicos(int limite)
        {
            List<AdminServico> lista = GetServicos("", "Ativo", 5000);
            lista.Sort(delegate (AdminServico a, AdminServico b)
            {
                decimal scoreA = (a.TotalMarcacoes * 10m) + (a.Avaliacao * 2m) + (a.Popularidade / 100m);
                decimal scoreB = (b.TotalMarcacoes * 10m) + (b.Avaliacao * 2m) + (b.Popularidade / 100m);
                int cmp = scoreB.CompareTo(scoreA);
                if (cmp != 0) return cmp;
                cmp = b.TotalMarcacoes.CompareTo(a.TotalMarcacoes);
                if (cmp != 0) return cmp;
                return b.Avaliacao.CompareTo(a.Avaliacao);
            });
            if (limite > 0 && lista.Count > limite) lista = lista.GetRange(0, limite);
            return lista;
        }

        public static List<AdminMarcacao> GetMarcacoes(string termo, string estado, DateTime? inicio, DateTime? fim, int limite)
        {
            List<AdminMarcacao> lista = new List<AdminMarcacao>();
            string sql = @"
SELECT TOP (@Limite) m.IdMarcacao, m.IdCliente, m.IdProfissional, m.IdServico, m.Cliente, c.Foto AS ClienteFoto, m.Profissional, m.Servico, m.DataMarcacao, m.Hora, m.DuracaoMinutos, m.Valor, m.Estado, m.Observacoes
FROM dbo.Marcacoes m
LEFT JOIN dbo.Usuarios c ON c.IdUsuario=m.IdCliente
WHERE (@Termo='' OR m.Cliente LIKE @Like OR m.Profissional LIKE @Like OR m.Servico LIKE @Like OR m.Estado LIKE @Like)
  AND (@Estado='Todos' OR m.Estado=@Estado)
  AND (@Inicio IS NULL OR m.DataMarcacao >= @Inicio)
  AND (@Fim IS NULL OR m.DataMarcacao < @Fim)
ORDER BY m.DataMarcacao DESC, m.Hora DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 1000 : limite);
                cmd.Parameters.AddWithValue("@Termo", termo ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (termo ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
                cmd.Parameters.AddWithValue("@Inicio", inicio.HasValue ? (object)inicio.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("@Fim", fim.HasValue ? (object)fim.Value.Date.AddDays(1) : DBNull.Value);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AdminMarcacao
                        {
                            IdMarcacao = ReadInt(r, "IdMarcacao"), IdCliente = ReadNullableInt(r, "IdCliente"), IdProfissional = ReadNullableInt(r, "IdProfissional"), IdServico = ReadNullableInt(r, "IdServico"), Cliente = ReadString(r, "Cliente"), ClienteFoto = ReadString(r, "ClienteFoto"), Profissional = ReadString(r, "Profissional"), Servico = ReadString(r, "Servico"), DataMarcacao = ReadDateTime(r, "DataMarcacao", DateTime.Today), Hora = ReadTime(r, "Hora", TimeSpan.Zero), DuracaoMinutos = ReadInt(r, "DuracaoMinutos"), Valor = ReadDecimal(r, "Valor", 0), Estado = ReadString(r, "Estado"), Observacoes = ReadString(r, "Observacoes")
                        });
                    }
                }
            }
            return lista;
        }

        public static List<AdminFatura> GetFaturas(string termo, string estado, DateTime? inicio, DateTime? fim, int limite)
        {
            List<AdminFatura> lista = new List<AdminFatura>();
            string sql = @"
SELECT TOP (@Limite) IdFatura, NumeroFatura, IdMarcacao, IdCliente, Cliente, DataFatura, Servicos, Subtotal, Desconto, Total, MetodoPagamento, Estado
FROM dbo.Faturas
WHERE (@Termo='' OR Cliente LIKE @Like OR NumeroFatura LIKE @Like OR Servicos LIKE @Like)
  AND (@Estado='Todos' OR Estado=@Estado)
  AND (@Inicio IS NULL OR DataFatura >= @Inicio)
  AND (@Fim IS NULL OR DataFatura < @Fim)
ORDER BY DataFatura DESC, IdFatura DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Limite", limite <= 0 ? 1000 : limite);
                cmd.Parameters.AddWithValue("@Termo", termo ?? "");
                cmd.Parameters.AddWithValue("@Like", "%" + (termo ?? "") + "%");
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Todos" : estado);
                cmd.Parameters.AddWithValue("@Inicio", inicio.HasValue ? (object)inicio.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("@Fim", fim.HasValue ? (object)fim.Value.Date.AddDays(1) : DBNull.Value);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AdminFatura { IdFatura = ReadInt(r, "IdFatura"), NumeroFatura = ReadString(r, "NumeroFatura"), IdMarcacao = ReadNullableInt(r, "IdMarcacao"), IdCliente = ReadNullableInt(r, "IdCliente"), Cliente = ReadString(r, "Cliente"), DataFatura = ReadDateTime(r, "DataFatura", DateTime.Today), Servicos = ReadString(r, "Servicos"), Subtotal = ReadDecimal(r, "Subtotal", 0), Desconto = ReadDecimal(r, "Desconto", 0), Total = ReadDecimal(r, "Total", 0), MetodoPagamento = ReadString(r, "MetodoPagamento"), Estado = ReadString(r, "Estado") });
                    }
                }
            }
            return lista;
        }

        public static AdminFatura GetFaturaById(int id)
        {
            List<AdminFatura> list = GetFaturas("", "Todos", null, null, 5000);
            foreach (AdminFatura f in list) if (f.IdFatura == id) return f;
            return null;
        }

        public static void SetEstadoFatura(int id, string estado)
        {
            ExecuteNonQuery("UPDATE dbo.Faturas SET Estado=@Estado WHERE IdFatura=@Id", new Dictionary<string, object> { { "@Estado", NormalizarEstadoFaturaBanco(estado) }, { "@Id", id } });
        }

        public static void SetEstadoMarcacao(int id, string estado)
        {
            string estadoBanco = NormalizarEstadoMarcacaoBanco(estado);
            ExecuteNonQuery(@"UPDATE dbo.Marcacoes SET Estado=@Estado WHERE IdMarcacao=@Id;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT IdCliente,
       CASE WHEN @Estado LIKE N'Confirmad%' THEN N'A sua marcação foi confirmada: ' ELSE N'A sua marcação foi atualizada: ' END
       + ISNULL(Servico,N'') + N' em ' + CONVERT(NVARCHAR(10), DataMarcacao, 103) + N' às ' + CONVERT(NVARCHAR(5), Hora, 108) + N'.',
       GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@Id AND IdCliente IS NOT NULL;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT IdProfissional,
       CASE WHEN @Estado LIKE N'Confirmad%' THEN N'Marcação confirmada: ' ELSE N'Marcação atualizada: ' END
       + ISNULL(Cliente,N'') + N' - ' + ISNULL(Servico,N'') + N'.',
       GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@Id AND IdProfissional IS NOT NULL;",
                new Dictionary<string, object> { { "@Estado", estadoBanco }, { "@Id", id } });
        }

        public static void DesativarUsuario(int id, bool ativo)
        {
            ExecuteNonQuery("UPDATE dbo.Usuarios SET Ativo=@Ativo WHERE IdUsuario=@Id", new Dictionary<string, object> { { "@Ativo", ativo }, { "@Id", id } });
        }

        public static void DesativarServico(int id, bool ativo)
        {
            ExecuteNonQuery("UPDATE dbo.Servicos SET Ativo=@Ativo WHERE IdServico=@Id", new Dictionary<string, object> { { "@Id", id }, { "@Ativo", ativo } });
        }

        public static void EliminarServico(int id)
        {
            ExecuteNonQuery(@"UPDATE dbo.Servicos SET Excluido=1, Ativo=0 WHERE IdServico=@Id;
IF OBJECT_ID('dbo.MarcacaoServicos','U') IS NOT NULL
BEGIN
    UPDATE m
       SET Estado=N'Cancelada',
           Observacoes=LEFT(ISNULL(m.Observacoes,N'') + CASE WHEN ISNULL(m.Observacoes,N'') = N'' THEN N'' ELSE N' | ' END + N'Serviço eliminado do catálogo.', 500)
      FROM dbo.Marcacoes m
      INNER JOIN dbo.MarcacaoServicos ms ON ms.IdMarcacao=m.IdMarcacao
     WHERE ms.IdServico=@Id
       AND CAST(m.DataMarcacao AS DATE) >= CAST(GETDATE() AS DATE)
       AND ISNULL(m.Estado,N'') NOT IN (N'Cancelada', N'Concluída', N'Concluida');
END;
UPDATE dbo.Marcacoes
   SET Estado=N'Cancelada',
       Observacoes=LEFT(ISNULL(Observacoes,N'') + CASE WHEN ISNULL(Observacoes,N'') = N'' THEN N'' ELSE N' | ' END + N'Serviço eliminado do catálogo.', 500)
 WHERE IdServico=@Id
   AND CAST(DataMarcacao AS DATE) >= CAST(GETDATE() AS DATE)
   AND ISNULL(Estado,N'') NOT IN (N'Cancelada', N'Concluída', N'Concluida');", new Dictionary<string, object> { { "@Id", id } });
        }

        public static string CriarCliente(string nome, string email, string telefone)
        {
            string senha = GerarSenhaTemporaria();
            ExecuteNonQuery(@"INSERT INTO dbo.Usuarios (Nome, Email, Telefone, Senha, TipoUsuario, Ativo, DataCriacao, UltimaVisita) VALUES (@Nome,@Email,@Telefone,@Senha,'Cliente',1,GETDATE(),NULL)",
                new Dictionary<string, object> { { "@Nome", nome }, { "@Email", email }, { "@Telefone", telefone }, { "@Senha", GerarHashSenha(senha) } });
            return senha;
        }

        public static string CriarProfissional(string nome, string email, string telefone, string especialidade, decimal comissao, decimal avaliacao)
        {
            string senha = GerarSenhaTemporaria();
            ExecuteNonQuery(@"INSERT INTO dbo.Usuarios (Nome, Email, Telefone, Senha, TipoUsuario, Ativo, DataCriacao, Especialidade, ComissaoPercentual, Avaliacao) VALUES (@Nome,@Email,@Telefone,@Senha,'Profissional',1,GETDATE(),@Especialidade,@Comissao,@Avaliacao)",
                new Dictionary<string, object> { { "@Nome", nome }, { "@Email", email }, { "@Telefone", telefone }, { "@Senha", GerarHashSenha(senha) }, { "@Especialidade", especialidade }, { "@Comissao", comissao }, { "@Avaliacao", avaliacao } });
            return senha;
        }

        public static int CriarServico(string nome, string categoria, int duracao, decimal preco, string descricao, string foto)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Servicos (Nome, Categoria, DuracaoMinutos, Preco, Ativo, Popularidade, Descricao, Foto, Avaliacao) OUTPUT INSERTED.IdServico VALUES (@Nome,@Categoria,@Duracao,@Preco,1,0,@Descricao,@Foto,5)", conn))
            {
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Categoria", categoria);
                cmd.Parameters.AddWithValue("@Duracao", duracao);
                cmd.Parameters.AddWithValue("@Preco", preco);
                cmd.Parameters.AddWithValue("@Descricao", descricao ?? "");
                cmd.Parameters.AddWithValue("@Foto", string.IsNullOrWhiteSpace(foto) ? (object)DBNull.Value : foto);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void AtualizarProfissional(int id, string nome, string email, string telefone, string especialidade, decimal comissao, decimal avaliacao, string foto)
        {
            ExecuteNonQuery(@"UPDATE dbo.Usuarios
SET Nome=@Nome, Email=@Email, Telefone=@Telefone, Especialidade=@Especialidade, ComissaoPercentual=@Comissao, Avaliacao=@Avaliacao, Foto=@Foto
WHERE IdUsuario=@Id AND TipoUsuario='Profissional'",
                new Dictionary<string, object> { { "@Id", id }, { "@Nome", nome }, { "@Email", email }, { "@Telefone", telefone }, { "@Especialidade", especialidade }, { "@Comissao", comissao }, { "@Avaliacao", avaliacao }, { "@Foto", string.IsNullOrWhiteSpace(foto) ? (object)DBNull.Value : foto } });
        }

        public static void AtualizarServico(int id, string nome, string categoria, int duracao, decimal preco, string descricao, string foto)
        {
            ExecuteNonQuery(@"UPDATE dbo.Servicos
SET Nome=@Nome, Categoria=@Categoria, DuracaoMinutos=@Duracao, Preco=@Preco, Descricao=@Descricao, Foto=@Foto
WHERE IdServico=@Id",
                new Dictionary<string, object> { { "@Id", id }, { "@Nome", nome }, { "@Categoria", categoria }, { "@Duracao", duracao }, { "@Preco", preco }, { "@Descricao", descricao ?? "" }, { "@Foto", string.IsNullOrWhiteSpace(foto) ? (object)DBNull.Value : foto } });
        }

        public static int CriarMarcacao(int? idCliente, int? idProfissional, int? idServico, string cliente, string profissional, string servico, DateTime data, TimeSpan hora, int duracao, decimal valor, string estado, string obs)
        {
            if (!idCliente.HasValue) idCliente = ResolverIdAdmin();
            estado = NormalizarEstadoMarcacaoBanco(estado);
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Marcacoes (IdCliente, IdProfissional, IdServico, Cliente, Profissional, Servico, DataMarcacao, Hora, DuracaoMinutos, Valor, Estado, Observacoes, IdEspaco, Espaco, DataCriacao) OUTPUT INSERTED.IdMarcacao VALUES (@IdCliente,@IdProfissional,@IdServico,@Cliente,@Profissional,@Servico,@Data,@Hora,@Duracao,@Valor,@Estado,@Obs,1,N'Sala 1',GETDATE())", conn))
            {
                cmd.Parameters.AddWithValue("@IdCliente", idCliente.HasValue ? (object)idCliente.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfissional", idProfissional.HasValue ? (object)idProfissional.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IdServico", idServico.HasValue ? (object)idServico.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Cliente", cliente ?? "");
                cmd.Parameters.AddWithValue("@Profissional", profissional ?? "");
                cmd.Parameters.AddWithValue("@Servico", servico ?? "");
                cmd.Parameters.AddWithValue("@Data", data.Date);
                cmd.Parameters.AddWithValue("@Hora", hora);
                cmd.Parameters.AddWithValue("@Duracao", duracao);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Pendente" : estado);
                cmd.Parameters.AddWithValue("@Obs", obs ?? "");
                conn.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                if (idCliente.HasValue && data.Date <= DateTime.Today)
                {
                    ExecuteNonQuery("UPDATE dbo.Usuarios SET UltimaVisita=@Data WHERE IdUsuario=@Id AND (UltimaVisita IS NULL OR UltimaVisita<@Data)", new Dictionary<string, object> { { "@Data", data.Date }, { "@Id", idCliente.Value } });
                }
                return id;
            }
        }


        public static bool ProfissionalDisponivel(int idProfissional, DateTime data, TimeSpan hora, int duracaoMinutos)
        {
            if (idProfissional <= 0) return false;
            if (duracaoMinutos <= 0) duracaoMinutos = 60;
            int inicio = (int)hora.TotalMinutes;
            string sql = @"SELECT COUNT(*)
FROM dbo.Marcacoes
WHERE IdProfissional=@IdProfissional
  AND CAST(DataMarcacao AS DATE)=@Data
  AND ISNULL(Estado,'') NOT IN ('Cancelado','Cancelada','Concluido','Concluida','Concluído','Concluída')
  AND @InicioMin < (DATEPART(HOUR, Hora) * 60 + DATEPART(MINUTE, Hora) + ISNULL(NULLIF(DuracaoMinutos,0),60))
  AND (@InicioMin + @Duracao) > (DATEPART(HOUR, Hora) * 60 + DATEPART(MINUTE, Hora));";
            int ocupadas = ScalarInt(sql, new Dictionary<string, object>
            {
                { "@IdProfissional", idProfissional },
                { "@Data", data.Date },
                { "@InicioMin", inicio },
                { "@Duracao", duracaoMinutos }
            });
            return ocupadas == 0;
        }

        public static List<AdminProfissional> GetProfissionaisDisponiveis(DateTime data, TimeSpan hora, int duracaoMinutos)
        {
            List<AdminProfissional> todos = GetProfissionais("", "Ativo", 5000);
            List<AdminProfissional> livres = new List<AdminProfissional>();
            foreach (AdminProfissional p in todos)
                if (ProfissionalDisponivel(p.IdUsuario, data, hora, duracaoMinutos)) livres.Add(p);
            return livres;
        }

        public static string EnviarMensagem(List<int> ids, string canal, string assunto, string mensagem)
        {
            if (ids == null || ids.Count == 0) return "Nenhum destinatário selecionado.";
            string canalFinal = string.IsNullOrWhiteSpace(canal) ? "App" : canal.Trim();
            if (!canalFinal.Equals("App", StringComparison.OrdinalIgnoreCase) && !canalFinal.Equals("Email", StringComparison.OrdinalIgnoreCase)) canalFinal = "Email";
            int enviados = 0;
            int falhados = 0;
            List<string> erros = new List<string>();
            foreach (int id in ids)
            {
                Dictionary<string, string> dados = GetContactoUsuario(id);
                string nome = dados.ContainsKey("Nome") ? dados["Nome"] : "";
                string email = dados.ContainsKey("Email") ? dados["Email"] : "";
                string telefone = dados.ContainsKey("Telefone") ? dados["Telefone"] : "";
                string estadoEnvio = "Notificação do app";
                string erroEnvio = "";

                if (canalFinal.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(email)) { estadoEnvio = "Não enviado"; erroEnvio = "Destinatário sem email."; }
                    else
                    {
                        try { EnviarEmailReal(email, assunto, mensagem); estadoEnvio = "Email enviado"; }
                        catch (Exception ex) { estadoEnvio = "Email não enviado"; erroEnvio = ex.Message; }
                    }
                }


                ExecuteNonQuery(@"INSERT INTO dbo.MensagensEnviadas (IdRemetente, IdDestinatario, DestinatarioNome, Canal, Assunto, Mensagem, EstadoEnvio, ErroEnvio)
VALUES (@Rem,@Dest,@Nome,@Canal,@Assunto,@Mensagem,@EstadoEnvio,@ErroEnvio);
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida) VALUES (@Dest, @Mensagem, GETDATE(), 0);",
                    new Dictionary<string, object> { { "@Rem", ResolverIdAdmin() }, { "@Dest", id }, { "@Nome", nome }, { "@Canal", canalFinal }, { "@Assunto", assunto ?? "" }, { "@Mensagem", mensagem ?? "" }, { "@EstadoEnvio", estadoEnvio }, { "@ErroEnvio", erroEnvio } });
                if (estadoEnvio.IndexOf("enviado", StringComparison.OrdinalIgnoreCase) >= 0 && estadoEnvio.IndexOf("não", StringComparison.OrdinalIgnoreCase) < 0 && estadoEnvio.IndexOf("Não", StringComparison.OrdinalIgnoreCase) < 0) enviados++;
                else if (canalFinal.Equals("App", StringComparison.OrdinalIgnoreCase)) enviados++;
                else { falhados++; if (!string.IsNullOrWhiteSpace(erroEnvio)) erros.Add(nome + ": " + erroEnvio); }
            }
            string resumo = canalFinal.Equals("App", StringComparison.OrdinalIgnoreCase)
                ? "Mensagem publicada nas notificações do app para " + enviados + " destinatário(s)."
                : canalFinal + ": " + enviados + " enviado(s), " + falhados + " falhado(s).";
            if (erros.Count > 0) resumo += Environment.NewLine + string.Join(Environment.NewLine, erros.Take(3).ToArray());
            return resumo;
        }

        private static Dictionary<string, string> GetContactoUsuario(int id)
        {
            Dictionary<string, string> dados = new Dictionary<string, string>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 Nome, Email, Telefone FROM dbo.Usuarios WHERE IdUsuario=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        dados["Nome"] = ReadString(r, "Nome");
                        dados["Email"] = ReadString(r, "Email");
                        dados["Telefone"] = ReadString(r, "Telefone");
                    }
                }
            }
            return dados;
        }

        private static void EnviarEmailReal(string destino, string assunto, string mensagem)
        {
            string host = AppSetting("SmtpHost");
            string user = AppSetting("SmtpUser");
            string pass = AppSetting("SmtpPass");
            if (!string.IsNullOrWhiteSpace(pass)) pass = pass.Replace(" ", "");
            string from = AppSetting("SmtpFrom");
            if (string.IsNullOrWhiteSpace(from)) from = user;
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
                throw new InvalidOperationException("Configure SmtpHost, SmtpUser, SmtpPass e SmtpFrom no App.config para envio real de email.");

            int port = 587;
            int.TryParse(AppSetting("SmtpPort"), out port);
            bool ssl = !AppSetting("SmtpEnableSsl").Equals("false", StringComparison.OrdinalIgnoreCase);
            using (SmtpClient smtp = new SmtpClient(host, port <= 0 ? 587 : port))
            {
                smtp.EnableSsl = ssl;
                if (!string.IsNullOrWhiteSpace(user)) smtp.Credentials = new NetworkCredential(user, pass);
                using (MailMessage mail = new MailMessage(from, destino))
                {
                    mail.Subject = string.IsNullOrWhiteSpace(assunto) ? "Mensagem BeauteCare" : assunto;
                    mail.IsBodyHtml = true;
                    mail.Body = MontarCorpoEmail(mensagem);
                    smtp.Send(mail);
                }
            }
        }

        private static string MontarCorpoEmail(string mensagem)
        {
            string texto = System.Net.WebUtility.HtmlEncode(mensagem ?? "");
            texto = texto.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>");
            return @"<div style='font-family:Segoe UI,Arial,sans-serif;background:#fff6fa;padding:24px'>" +
                   @"<div style='max-width:560px;margin:auto;background:#ffffff;border-radius:18px;padding:24px;border:1px solid #ffd6e5'>" +
                   @"<h2 style='margin:0 0 12px;color:#ff4f87'>BeautéCare</h2>" +
                   @"<p style='font-size:15px;color:#3b2f3b;line-height:1.6'>" + texto + @"</p>" +
                   @"<p style='font-size:12px;color:#888;margin-top:24px'>Esta mensagem foi enviada pelo sistema BeautéCare.</p>" +
                   @"</div></div>";
        }

        private static string AppSetting(string key)
        {
            try { return ConfigurationManager.AppSettings[key] ?? ""; }
            catch { return ""; }
        }


        public static string GetTipoUsuario(int idUsuario)
        {
            return ScalarString("SELECT TOP 1 TipoUsuario FROM dbo.Usuarios WHERE IdUsuario=@Id", new Dictionary<string, object> { { "@Id", idUsuario } });
        }

        public static List<AdminOpcao> GetUsuariosMensagemOpcoes(string tipo)
        {
            List<AdminOpcao> lista = new List<AdminOpcao>();
            string filtro = string.IsNullOrWhiteSpace(tipo) || tipo == "Todos" ? "" : " AND TipoUsuario=@Tipo";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdUsuario, Nome, TipoUsuario FROM dbo.Usuarios WHERE Ativo=1 AND TipoUsuario IN ('Cliente','Profissional')" + filtro + " ORDER BY TipoUsuario, Nome", conn))
            {
                if (!string.IsNullOrWhiteSpace(filtro)) cmd.Parameters.AddWithValue("@Tipo", tipo);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AdminOpcao { Id = ReadInt(r, "IdUsuario"), Nome = ReadString(r, "Nome"), Tipo = ReadString(r, "TipoUsuario") });
                    }
                }
            }
            return lista;
        }

        public static void MarcarTodasNotificacoesAdminLidas()
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE Lida=0 AND IdUsuario=@Admin", new Dictionary<string, object> { { "@Admin", ResolverIdAdmin() } });
        }

        public static int CriarFaturaMarcacao(int idMarcacao, int? idCliente, int? idProfissional, string cliente, string profissional, string servicos, decimal subtotal, decimal desconto, decimal total, string metodo, string estado, DateTime data, TimeSpan hora)
        {
            string numero = "FAT" + DateTime.Now.ToString("yyyy") + "/" + (ScalarInt("SELECT COUNT(*)+1 FROM dbo.Faturas WHERE YEAR(DataFatura)=YEAR(GETDATE())", null)).ToString("0000");
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Faturas (NumeroFatura, IdMarcacao, IdCliente, IdProfissional, Cliente, Profissional, DataFatura, HoraFatura, Servicos, Subtotal, Desconto, Total, ValorTotal, MetodoPagamento, Estado, ComissaoPercentual)
OUTPUT INSERTED.IdFatura
VALUES (@Numero,@IdMarcacao,@IdCliente,@IdProfissional,@Cliente,@Profissional,@Data,@Hora,@Servicos,@Subtotal,@Desconto,@Total,@Total,@Metodo,@Estado,ISNULL((SELECT TOP 1 ComissaoPercentual FROM dbo.Usuarios WHERE IdUsuario=@IdProfissional),40))", conn))
            {
                cmd.Parameters.AddWithValue("@Numero", numero);
                cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente.HasValue ? (object)idCliente.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IdProfissional", idProfissional.HasValue ? (object)idProfissional.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Cliente", cliente ?? "");
                cmd.Parameters.AddWithValue("@Profissional", profissional ?? "");
                cmd.Parameters.AddWithValue("@Data", data.Date);
                cmd.Parameters.AddWithValue("@Hora", hora);
                cmd.Parameters.AddWithValue("@Servicos", servicos ?? "");
                cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                cmd.Parameters.AddWithValue("@Desconto", desconto);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Metodo", string.IsNullOrWhiteSpace(metodo) ? "Multibanco" : metodo);
                cmd.Parameters.AddWithValue("@Estado", NormalizarEstadoFaturaBanco(estado));
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<AdminOpcao> GetClientesOpcoes()
        {
            return GetUsuarioOpcoes("Cliente");
        }

        public static List<AdminOpcao> GetProfissionaisOpcoes()
        {
            return GetUsuarioOpcoes("Profissional");
        }

        private static List<AdminOpcao> GetUsuarioOpcoes(string tipo)
        {
            List<AdminOpcao> lista = new List<AdminOpcao>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdUsuario, Nome FROM dbo.Usuarios WHERE TipoUsuario=@Tipo AND Ativo=1 ORDER BY Nome", conn))
            {
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader()) while (r.Read()) lista.Add(new AdminOpcao { Id = ReadInt(r, "IdUsuario"), Nome = ReadString(r, "Nome") });
            }
            return lista;
        }

        public static List<AdminOpcao> GetServicosOpcoes()
        {
            List<AdminOpcao> lista = new List<AdminOpcao>();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT IdServico, Nome FROM dbo.Servicos WHERE Ativo=1 ORDER BY Nome", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader()) while (r.Read()) lista.Add(new AdminOpcao { Id = ReadInt(r, "IdServico"), Nome = ReadString(r, "Nome") });
            }
            return lista;
        }

        public static AdminServico GetServico(int id)
        {
            foreach (AdminServico s in GetServicos("", "Todos", 5000)) if (s.IdServico == id) return s;
            return null;
        }

        public static List<AdminSearchItem> PesquisarTudo(string termo)
        {
            List<AdminSearchItem> lista = new List<AdminSearchItem>();
            if (string.IsNullOrWhiteSpace(termo)) return lista;
            foreach (AdminCliente c in GetClientes(termo, "Todos", 5)) lista.Add(new AdminSearchItem { Tipo = "Cliente", Titulo = c.Nome, Subtitulo = c.Email + " • " + c.Telefone });
            foreach (AdminProfissional p in GetProfissionais(termo, "Todos", 5)) lista.Add(new AdminSearchItem { Tipo = "Profissional", Titulo = p.Nome, Subtitulo = p.Especialidade + " • " + p.Email });
            foreach (AdminServico s in GetServicos(termo, "Todos", 5)) lista.Add(new AdminSearchItem { Tipo = "Serviço", Titulo = s.Nome, Subtitulo = s.Categoria + " • " + Money(s.Preco) });
            foreach (AdminMarcacao m in GetMarcacoes(termo, "Todos", null, null, 5)) lista.Add(new AdminSearchItem { Tipo = "Marcação", Titulo = m.Cliente + " - " + m.Servico, Subtitulo = m.DataMarcacao.ToString("dd/MM/yyyy") + " " + m.Hora.ToString(@"hh\:mm") + " • " + m.Estado });
            foreach (AdminFatura f in GetFaturas(termo, "Todos", null, null, 5)) lista.Add(new AdminSearchItem { Tipo = "Fatura", Titulo = f.NumeroFatura + " - " + f.Cliente, Subtitulo = f.Servicos + " • " + Money(f.Total) + " • " + f.Estado });
            if (lista.Count > 12) lista = lista.GetRange(0, 12);
            return lista;
        }

        public static string Money(decimal value) { return value.ToString("C2", Pt); }

        public static string PrimeiroNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "Admin";
            string[] parts = nome.Trim().Split(' ');
            return parts.Length == 0 ? nome : parts[0];
        }

        public static int ImportarClientesCsv(string path)
        {
            int count = 0;
            if (!File.Exists(path)) return 0;
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (i == 0 && line.ToLowerInvariant().Contains("nome") && line.ToLowerInvariant().Contains("email")) continue;
                string[] p = line.Split(';', ',');
                if (p.Length < 2) continue;
                string nome = p[0].Trim(); string email = p[1].Trim(); string tel = p.Length > 2 ? p[2].Trim() : "";
                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email)) continue;
                if (ScalarInt("SELECT COUNT(*) FROM dbo.Usuarios WHERE Email=@Email", new Dictionary<string, object> { { "@Email", email } }) == 0)
                {
                    CriarCliente(nome, email, tel); count++;
                }
            }
            RegistarImportacao("Clientes", path, count);
            return count;
        }

        public static int ImportarProfissionaisCsv(string path)
        {
            int count = 0;
            if (!File.Exists(path)) return 0;
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (i == 0 && line.ToLowerInvariant().Contains("nome") && line.ToLowerInvariant().Contains("email")) continue;
                string[] p = line.Split(';', ',');
                if (p.Length < 2) continue;
                string nome = p[0].Trim(); string email = p[1].Trim(); string tel = p.Length > 2 ? p[2].Trim() : ""; string esp = p.Length > 3 ? p[3].Trim() : "Estética";
                if (ScalarInt("SELECT COUNT(*) FROM dbo.Usuarios WHERE Email=@Email", new Dictionary<string, object> { { "@Email", email } }) == 0)
                {
                    CriarProfissional(nome, email, tel, esp, 40, 5); count++;
                }
            }
            RegistarImportacao("Profissionais", path, count);
            return count;
        }

        public static int ImportarFaturasCsv(string path)
        {
            int count = 0;
            if (!File.Exists(path)) return 0;
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (i == 0 && line.ToLowerInvariant().Contains("fatura")) continue;
                string[] p = line.Split(';');
                if (p.Length < 5) continue;
                string numero = p[0].Trim(); string cliente = p[1].Trim(); string servicos = p[2].Trim(); decimal total = ToDecimal(p[3]); string estado = p[4].Trim();
                if (ScalarInt("SELECT COUNT(*) FROM dbo.Faturas WHERE NumeroFatura=@N", new Dictionary<string, object> { { "@N", numero } }) == 0)
                {
                    ExecuteNonQuery(@"INSERT INTO dbo.Faturas (NumeroFatura, Cliente, DataFatura, Servicos, Subtotal, Desconto, Total, ValorTotal, MetodoPagamento, Estado) VALUES (@N,@C,CAST(GETDATE() AS DATE),@S,@T,0,@T,@T,'Importado',@E)", new Dictionary<string, object> { { "@N", numero }, { "@C", cliente }, { "@S", servicos }, { "@T", total }, { "@E", string.IsNullOrWhiteSpace(estado) ? "Pendente" : estado } });
                    count++;
                }
            }
            RegistarImportacao("Faturas", path, count);
            return count;
        }

        public static void RegistarImportacao(string tipo, string path, int count)
        {
            ExecuteNonQuery("INSERT INTO dbo.Importacoes (Tipo, Ficheiro, LinhasImportadas) VALUES (@Tipo,@Ficheiro,@Linhas)", new Dictionary<string, object> { { "@Tipo", tipo }, { "@Ficheiro", path }, { "@Linhas", count } });
        }

        private static AdminCliente ReadCliente(SqlDataReader r)
        {
            return new AdminCliente
            {
                IdUsuario = ReadInt(r, "IdUsuario"), Nome = ReadString(r, "Nome"), Email = ReadString(r, "Email"), Telefone = ReadString(r, "Telefone"), Ativo = ReadBool(r, "Ativo"), UltimaVisita = ReadNullableDateTime(r, "UltimaVisita"), DataCriacao = ReadDateTime(r, "DataCriacao", DateTime.Today), Foto = ReadString(r, "Foto"), TotalServicos = ReadInt(r, "TotalServicos"), TotalGasto = ReadDecimal(r, "TotalGasto", 0), MarcacoesMes = ReadInt(r, "MarcacoesMes"), UltimoServico = ReadString(r, "UltimoServico"), UltimaMarcacaoData = ReadNullableDateTime(r, "UltimaMarcacaoData"), UltimaMarcacaoHora = ReadNullableTime(r, "UltimaMarcacaoHora")
            };
        }

        private static void AddDateAndEstado(ref string sql, Dictionary<string, object> p, string campo, DateTime? inicio, DateTime? fim, string estado)
        {
            if (inicio.HasValue) { sql += " AND " + campo + ">=@Inicio"; p["@Inicio"] = inicio.Value.Date; }
            if (fim.HasValue) { sql += " AND " + campo + "<@Fim"; p["@Fim"] = fim.Value.Date.AddDays(1); }
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            {
                string e = NormalizarEstadoMarcacaoBanco(estado);
                if (e == "Confirmada") sql += " AND Estado IN ('Confirmada','Confirmado')";
                else if (e == "Cancelada") sql += " AND Estado IN ('Cancelada','Cancelado')";
                else if (e == "Concluida") sql += " AND Estado IN ('Concluida','Concluído','Concluída','Concluido')";
                else { sql += " AND Estado=@Estado"; p["@Estado"] = e; }
            }
        }

        private static string NormalizarEstadoMarcacaoBanco(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "Pendente";
            string e = estado.Trim().ToLowerInvariant();
            if (e.StartsWith("confirm")) return "Confirmada";
            if (e.StartsWith("cancel")) return "Cancelada";
            if (e.StartsWith("concl")) return "Concluida";
            if (e.StartsWith("pend")) return "Pendente";
            return estado.Trim();
        }

        private static string NormalizarEstadoFaturaBanco(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "Pendente";
            string e = estado.Trim().ToLowerInvariant();
            if (e.StartsWith("pag")) return "Paga";
            if (e.StartsWith("cancel")) return "Cancelada";
            if (e.StartsWith("não") || e.StartsWith("nao")) return "Pendente";
            if (e.StartsWith("pend")) return "Pendente";
            return estado.Trim();
        }

        public static int ScalarInt(string sql, Dictionary<string, object> p)
        {
            object o = ExecuteScalar(sql, p);
            if (o == null || o == DBNull.Value) return 0;
            return Convert.ToInt32(o);
        }

        public static decimal ScalarDecimal(string sql, Dictionary<string, object> p)
        {
            object o = ExecuteScalar(sql, p);
            if (o == null || o == DBNull.Value) return 0m;
            return Convert.ToDecimal(o);
        }

        public static string ScalarString(string sql, Dictionary<string, object> p)
        {
            object o = ExecuteScalar(sql, p);
            return o == null || o == DBNull.Value ? "" : o.ToString();
        }

        public static object ExecuteScalar(string sql, Dictionary<string, object> p)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddParams(cmd, p);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static void ExecuteNonQuery(string sql, Dictionary<string, object> p)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AddParams(cmd, p);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void AddParams(SqlCommand cmd, Dictionary<string, object> p)
        {
            if (p == null) return;
            foreach (KeyValuePair<string, object> kv in p) cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
        }

        public static string GerarSenhaTemporaria()
        {
            return "Bc" + DateTime.Now.ToString("HHmmss") + new Random().Next(10, 99).ToString();
        }

        public static string GerarHashSenha(string senha)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static decimal ToDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            text = text.Replace("€", "").Trim();
            decimal d;
            if (decimal.TryParse(text, NumberStyles.Any, Pt, out d)) return d;
            if (decimal.TryParse(text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return 0m;
        }

        public static int ReadInt(SqlDataReader r, string name)
        {
            object o = r[name]; if (o == DBNull.Value) return 0; return Convert.ToInt32(o);
        }
        public static int? ReadNullableInt(SqlDataReader r, string name)
        {
            object o = r[name]; if (o == DBNull.Value) return null; return Convert.ToInt32(o);
        }
        public static string ReadString(SqlDataReader r, string name)
        {
            object o = r[name]; return o == DBNull.Value ? "" : o.ToString();
        }
        public static bool ReadBool(SqlDataReader r, string name)
        {
            object o = r[name]; if (o == DBNull.Value) return false; return Convert.ToBoolean(o);
        }
        public static decimal ReadDecimal(SqlDataReader r, string name, decimal def)
        {
            object o = r[name]; if (o == DBNull.Value) return def; return Convert.ToDecimal(o);
        }
        public static DateTime ReadDateTime(SqlDataReader r, string name, DateTime def)
        {
            object o = r[name]; if (o == DBNull.Value) return def; return Convert.ToDateTime(o);
        }
        public static DateTime? ReadNullableDateTime(SqlDataReader r, string name)
        {
            object o = r[name]; if (o == DBNull.Value) return null; return Convert.ToDateTime(o);
        }
        public static TimeSpan ReadTime(SqlDataReader r, string name, TimeSpan def)
        {
            object o = r[name]; if (o == DBNull.Value) return def; if (o is TimeSpan) return (TimeSpan)o; return TimeSpan.Parse(o.ToString());
        }
        public static TimeSpan? ReadNullableTime(SqlDataReader r, string name)
        {
            object o = r[name]; if (o == DBNull.Value) return null; if (o is TimeSpan) return (TimeSpan)o; return TimeSpan.Parse(o.ToString());
        }
    }
}
