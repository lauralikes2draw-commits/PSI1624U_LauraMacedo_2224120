using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ProjetoFinal
{
    internal class ProfissionalInfo
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public string Especialidade { get; set; }
        public decimal Avaliacao { get; set; }
        public decimal ComissaoPercentual { get; set; }
    }

    internal class UsuarioInfo
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
    }

    internal class ServicoInfo
    {
        public int IdServico { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal Preco { get; set; }
        public decimal Avaliacao { get; set; }
        public string Foto { get; set; }
    }

    internal class MarcacaoInfo
    {
        public int IdMarcacao { get; set; }
        public int? IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Servico { get; set; }
        public DateTime DataMarcacao { get; set; }
        public TimeSpan Hora { get; set; }
        public int DuracaoMinutos { get; set; }
        public decimal Valor { get; set; }
        public string Estado { get; set; }
        public string Observacoes { get; set; }
        public string ClienteFoto { get; set; }
    }

    internal class FaturaInfo
    {
        public int IdFatura { get; set; }
        public string NumeroFatura { get; set; }
        public int? IdMarcacao { get; set; }
        public int? IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Profissional { get; set; }
        public DateTime DataFatura { get; set; }
        public TimeSpan? Hora { get; set; }
        public string Servicos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }
        public decimal ComissaoPercentual { get; set; }
        public string MetodoPagamento { get; set; }
        public string Estado { get; set; }
    }

    internal class DashboardResumo
    {
        public int MarcacoesHoje { get; set; }
        public int ServicosSemana { get; set; }
        public int ServicosMes { get; set; }
        public decimal TotalGeradoMes { get; set; }
        public decimal ComissaoMes { get; set; }
        public decimal JaPago { get; set; }
        public decimal PendenteReceber { get; set; }
        public decimal ComissaoPercentual { get; set; }
        public DateTime ProximoPagamento { get; set; }
        public decimal AvaliacaoMedia { get; set; }
    }

    internal class SearchItem
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Tipo { get; set; }
    }

    internal class NotificacaoInfo
    {
        public int Id { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataNotificacao { get; set; }
        public bool Lida { get; set; }
    }

    internal static class ProfissionalRepository
    {
        public static readonly CultureInfo Pt = new CultureInfo("pt-PT");

        public static void EnsureSchema()
        {
            string sql = @"
IF OBJECT_ID('dbo.Faturas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Faturas
    (
        IdFatura INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NumeroFatura NVARCHAR(40) NULL,
        IdMarcacao INT NULL,
        IdCliente INT NULL,
        IdProfissional INT NULL,
        Cliente NVARCHAR(120) NOT NULL CONSTRAINT DF_Faturas_Cliente_Code DEFAULT(''),
        Profissional NVARCHAR(120) NULL,
        DataFatura DATE NOT NULL CONSTRAINT DF_Faturas_Data_Code DEFAULT(CAST(GETDATE() AS DATE)),
        HoraFatura TIME NULL,
        Servicos NVARCHAR(300) NOT NULL CONSTRAINT DF_Faturas_Servicos_Code DEFAULT(''),
        Subtotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Subtotal_Code DEFAULT(0),
        Desconto DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Desconto_Code DEFAULT(0),
        Total DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Total_Code DEFAULT(0),
        ValorTotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_ValorTotal_Code DEFAULT(0),
        MetodoPagamento NVARCHAR(60) NOT NULL CONSTRAINT DF_Faturas_Metodo_Code DEFAULT('Multibanco'),
        Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_Faturas_Estado_Code DEFAULT('Pendente'),
        ComissaoPercentual DECIMAL(5,2) NOT NULL CONSTRAINT DF_Faturas_Comissao_Code DEFAULT(40),
        IdCupao INT NULL,
        CodigoCupao NVARCHAR(40) NULL
    );
END;
IF COL_LENGTH('dbo.Faturas', 'NumeroFatura') IS NULL ALTER TABLE dbo.Faturas ADD NumeroFatura NVARCHAR(40) NULL;
IF COL_LENGTH('dbo.Faturas', 'IdMarcacao') IS NULL ALTER TABLE dbo.Faturas ADD IdMarcacao INT NULL;
IF COL_LENGTH('dbo.Faturas', 'IdCliente') IS NULL ALTER TABLE dbo.Faturas ADD IdCliente INT NULL;
IF COL_LENGTH('dbo.Faturas', 'Cliente') IS NULL ALTER TABLE dbo.Faturas ADD Cliente NVARCHAR(120) NOT NULL CONSTRAINT DF_Faturas_Cliente_Code2 DEFAULT('');
IF COL_LENGTH('dbo.Faturas', 'DataFatura') IS NULL ALTER TABLE dbo.Faturas ADD DataFatura DATE NOT NULL CONSTRAINT DF_Faturas_Data_Code2 DEFAULT(CAST(GETDATE() AS DATE));
IF COL_LENGTH('dbo.Faturas', 'Servicos') IS NULL ALTER TABLE dbo.Faturas ADD Servicos NVARCHAR(300) NOT NULL CONSTRAINT DF_Faturas_Servicos_Code2 DEFAULT('');
IF COL_LENGTH('dbo.Faturas', 'Subtotal') IS NULL ALTER TABLE dbo.Faturas ADD Subtotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Subtotal_Code2 DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'Desconto') IS NULL ALTER TABLE dbo.Faturas ADD Desconto DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Desconto_Code2 DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'Total') IS NULL ALTER TABLE dbo.Faturas ADD Total DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_Total_Code2 DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'MetodoPagamento') IS NULL ALTER TABLE dbo.Faturas ADD MetodoPagamento NVARCHAR(60) NOT NULL CONSTRAINT DF_Faturas_Metodo_Code2 DEFAULT('Multibanco');
IF COL_LENGTH('dbo.Faturas', 'Estado') IS NULL ALTER TABLE dbo.Faturas ADD Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_Faturas_Estado_Code2 DEFAULT('Pendente');
IF OBJECT_ID('dbo.Notificacoes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notificacoes
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Mensagem NVARCHAR(300) NOT NULL,
        Tipo NVARCHAR(50) NULL,
        IdUsuario INT NULL,
        DataNotificacao DATETIME NOT NULL CONSTRAINT DF_Notificacoes_Data_Code DEFAULT(GETDATE()),
        Lida BIT NOT NULL CONSTRAINT DF_Notificacoes_Lida_Code DEFAULT(0)
    );
END;
IF COL_LENGTH('dbo.Notificacoes', 'IdUsuario') IS NULL ALTER TABLE dbo.Notificacoes ADD IdUsuario INT NULL;
IF COL_LENGTH('dbo.Faturas', 'IdProfissional') IS NULL ALTER TABLE dbo.Faturas ADD IdProfissional INT NULL;
IF COL_LENGTH('dbo.Faturas', 'Profissional') IS NULL ALTER TABLE dbo.Faturas ADD Profissional NVARCHAR(120) NULL;
IF COL_LENGTH('dbo.Faturas', 'ComissaoPercentual') IS NULL ALTER TABLE dbo.Faturas ADD ComissaoPercentual DECIMAL(5,2) NOT NULL CONSTRAINT DF_Faturas_ComissaoPercentual DEFAULT(40);
IF COL_LENGTH('dbo.Faturas', 'HoraFatura') IS NULL ALTER TABLE dbo.Faturas ADD HoraFatura TIME NULL;
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NULL ALTER TABLE dbo.Faturas ADD ValorTotal DECIMAL(10,2) NOT NULL CONSTRAINT DF_Faturas_ValorTotal DEFAULT(0);
IF COL_LENGTH('dbo.Faturas', 'ValorTotal') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Faturas') AND c.name='ValorTotal') ALTER TABLE dbo.Faturas ADD CONSTRAINT DF_Faturas_ValorTotal_Default DEFAULT(0) FOR ValorTotal;
IF COL_LENGTH('dbo.Faturas','ValorTotal') IS NOT NULL EXEC(N'UPDATE dbo.Faturas SET ValorTotal=COALESCE(NULLIF(ValorTotal,0), Total, Subtotal, 0) WHERE ValorTotal IS NULL OR ValorTotal=0');
IF COL_LENGTH('dbo.Servicos', 'Foto') IS NULL ALTER TABLE dbo.Servicos ADD Foto NVARCHAR(400) NULL;
IF COL_LENGTH('dbo.Servicos', 'Avaliacao') IS NULL ALTER TABLE dbo.Servicos ADD Avaliacao DECIMAL(3,2) NOT NULL CONSTRAINT DF_Servicos_Avaliacao DEFAULT(5);
IF COL_LENGTH('dbo.Servicos', 'Excluido') IS NULL ALTER TABLE dbo.Servicos ADD Excluido BIT NOT NULL CONSTRAINT DF_Servicos_Excluido DEFAULT(0);
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
IF COL_LENGTH('dbo.Marcacoes', 'DuracaoMinutos') IS NULL ALTER TABLE dbo.Marcacoes ADD DuracaoMinutos INT NOT NULL CONSTRAINT DF_Marcacoes_DuracaoMinutos DEFAULT(60);
IF COL_LENGTH('dbo.Marcacoes', 'IdEspaco') IS NULL ALTER TABLE dbo.Marcacoes ADD IdEspaco INT NOT NULL CONSTRAINT DF_Marcacoes_IdEspaco_Default DEFAULT(1);
IF COL_LENGTH('dbo.Marcacoes', 'Espaco') IS NULL ALTER TABLE dbo.Marcacoes ADD Espaco NVARCHAR(80) NOT NULL CONSTRAINT DF_Marcacoes_Espaco_Default DEFAULT(N'Sala 1');
IF COL_LENGTH('dbo.Marcacoes', 'IdEspaco') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Marcacoes') AND c.name='IdEspaco') ALTER TABLE dbo.Marcacoes ADD CONSTRAINT DF_Marcacoes_IdEspaco DEFAULT(1) FOR IdEspaco;
IF COL_LENGTH('dbo.Marcacoes', 'Espaco') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc INNER JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id WHERE dc.parent_object_id=OBJECT_ID('dbo.Marcacoes') AND c.name='Espaco') ALTER TABLE dbo.Marcacoes ADD CONSTRAINT DF_Marcacoes_Espaco DEFAULT(N'Sala 1') FOR Espaco;
IF COL_LENGTH('dbo.Marcacoes','IdEspaco') IS NOT NULL EXEC(N'UPDATE dbo.Marcacoes SET IdEspaco=1 WHERE IdEspaco IS NULL');
IF COL_LENGTH('dbo.Marcacoes','Espaco') IS NOT NULL EXEC(N'UPDATE dbo.Marcacoes SET Espaco=N''Sala 1'' WHERE Espaco IS NULL OR LTRIM(RTRIM(Espaco))=N''''');
IF OBJECT_ID('dbo.MarcacaoServicos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarcacaoServicos
    (
        IdMarcacaoServico INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdMarcacao INT NOT NULL,
        IdServico INT NOT NULL,
        NomeServico NVARCHAR(120) NOT NULL,
        DuracaoMinutos INT NOT NULL DEFAULT(60),
        Preco DECIMAL(10,2) NOT NULL DEFAULT(0)
    );
END;
UPDATE f
SET f.IdProfissional = COALESCE(f.IdProfissional, m.IdProfissional, (SELECT TOP 1 IdUsuario FROM dbo.Usuarios WHERE TipoUsuario='Profissional' ORDER BY IdUsuario)),
    f.Profissional = COALESCE(NULLIF(f.Profissional,''), m.Profissional, (SELECT TOP 1 Nome FROM dbo.Usuarios WHERE TipoUsuario='Profissional' ORDER BY IdUsuario)),
    f.ComissaoPercentual = COALESCE(NULLIF(f.ComissaoPercentual,0), u.ComissaoPercentual, 40),
    f.HoraFatura = COALESCE(f.HoraFatura, m.Hora)
FROM dbo.Faturas f
LEFT JOIN dbo.Marcacoes m ON f.IdMarcacao = m.IdMarcacao
LEFT JOIN dbo.Usuarios u ON COALESCE(f.IdProfissional, m.IdProfissional) = u.IdUsuario
WHERE f.IdProfissional IS NULL OR f.Profissional IS NULL OR f.HoraFatura IS NULL;
UPDATE m
SET m.DuracaoMinutos = COALESCE(NULLIF(m.DuracaoMinutos,0), s.DuracaoMinutos, 60)
FROM dbo.Marcacoes m
LEFT JOIN dbo.Servicos s ON m.IdServico = s.IdServico
WHERE m.DuracaoMinutos IS NULL OR m.DuracaoMinutos = 0;";
            ExecuteNonQuery(sql, null);
        }

        public static void GarantirNotificacoesIniciais(int idProfissional)
        {
            if (idProfissional <= 0) return;
            string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@IdUsuario AND Mensagem LIKE 'Bem-vinda%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdUsuario, 'Bem-vinda ao seu painel profissional. Confira as suas marcações de hoje.', GETDATE(), 0);
IF EXISTS (SELECT 1 FROM dbo.Marcacoes WHERE IdProfissional=@IdUsuario AND Estado='Pendente')
AND NOT EXISTS (SELECT 1 FROM dbo.Notificacoes WHERE IdUsuario=@IdUsuario AND Mensagem LIKE 'Há marcações pendentes%')
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdUsuario, 'Há marcações pendentes para confirmar ou acompanhar.', GETDATE(), 0);";
            ExecuteNonQuery(sql, new Dictionary<string, object> { { "@IdUsuario", idProfissional } });
        }

        public static int ResolverIdProfissional()
        {
            if (UsuarioLogado.Id > 0 && string.Equals(UsuarioLogado.Tipo, "Profissional", StringComparison.OrdinalIgnoreCase))
                return UsuarioLogado.Id;

            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 IdUsuario, Nome, Email, Foto FROM dbo.Usuarios WHERE TipoUsuario='Profissional' AND Ativo=1 ORDER BY IdUsuario", conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        UsuarioLogado.Id = Convert.ToInt32(r["IdUsuario"]);
                        UsuarioLogado.Nome = ReadString(r, "Nome");
                        UsuarioLogado.Email = ReadString(r, "Email");
                        UsuarioLogado.Tipo = "Profissional";
                        UsuarioLogado.Foto = ReadString(r, "Foto");
                        return UsuarioLogado.Id;
                    }
                }
            }
            return 0;
        }

        public static ProfissionalInfo GetProfissional(int idProfissional)
        {
            if (idProfissional <= 0) idProfissional = ResolverIdProfissional();
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(@"SELECT IdUsuario, Nome, Email, Foto, Especialidade, Avaliacao, ComissaoPercentual
                                                    FROM dbo.Usuarios WHERE IdUsuario=@IdUsuario", conn))
            {
                cmd.Parameters.AddWithValue("@IdUsuario", idProfissional);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new ProfissionalInfo
                        {
                            IdUsuario = Convert.ToInt32(r["IdUsuario"]),
                            Nome = ReadString(r, "Nome"),
                            Email = ReadString(r, "Email"),
                            Foto = ReadString(r, "Foto"),
                            Especialidade = ReadString(r, "Especialidade"),
                            Avaliacao = ReadDecimal(r, "Avaliacao", 5m),
                            ComissaoPercentual = ReadDecimal(r, "ComissaoPercentual", 40m)
                        };
                    }
                }
            }
            return new ProfissionalInfo { IdUsuario = idProfissional, Nome = "Profissional", Avaliacao = 5m, ComissaoPercentual = 40m };
        }

        public static DashboardResumo GetDashboardResumo(int idProfissional)
        {
            ProfissionalInfo p = GetProfissional(idProfissional);
            DateTime hoje = DateTime.Today;
            DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1);
            DateTime inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek + 1);
            if (hoje.DayOfWeek == DayOfWeek.Sunday) inicioSemana = hoje.AddDays(-6);
            DateTime fimSemana = inicioSemana.AddDays(7);

            int marcHoje = ExecuteScalarInt(@"SELECT COUNT(*) FROM dbo.Marcacoes WHERE IdProfissional=@Id AND DataMarcacao=@D AND Estado NOT IN ('Cancelado','Cancelada')", Param("@Id", idProfissional, "@D", hoje));
            int servSemana = ExecuteScalarInt(@"SELECT COUNT(*) FROM dbo.Marcacoes WHERE IdProfissional=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim AND Estado NOT IN ('Cancelado','Cancelada')", Param("@Id", idProfissional, "@Ini", inicioSemana, "@Fim", fimSemana));
            int servMes = ExecuteScalarInt(@"SELECT COUNT(*) FROM dbo.Marcacoes WHERE IdProfissional=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim AND Estado IN ('Concluído','Concluido')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));
            if (servMes == 0)
                servMes = ExecuteScalarInt(@"SELECT COUNT(*) FROM dbo.Marcacoes WHERE IdProfissional=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim AND Estado NOT IN ('Cancelado','Cancelada')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));

            decimal totalGerado = ExecuteScalarDecimal(@"SELECT COALESCE(SUM(Total),0) FROM dbo.Faturas WHERE IdProfissional=@Id AND DataFatura>=@Ini AND DataFatura<@Fim AND Estado NOT IN ('Cancelado','Cancelada')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));
            if (totalGerado == 0)
                totalGerado = ExecuteScalarDecimal(@"SELECT COALESCE(SUM(Valor),0) FROM dbo.Marcacoes WHERE IdProfissional=@Id AND DataMarcacao>=@Ini AND DataMarcacao<@Fim AND Estado NOT IN ('Cancelado','Cancelada')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));

            decimal pago = ExecuteScalarDecimal(@"SELECT COALESCE(SUM(Total * ComissaoPercentual / 100.0),0) FROM dbo.Faturas WHERE IdProfissional=@Id AND DataFatura>=@Ini AND DataFatura<@Fim AND Estado IN ('Paga','Pago','Liquidada')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));
            decimal pendente = ExecuteScalarDecimal(@"SELECT COALESCE(SUM(Total * ComissaoPercentual / 100.0),0) FROM dbo.Faturas WHERE IdProfissional=@Id AND DataFatura>=@Ini AND DataFatura<@Fim AND Estado NOT IN ('Paga','Pago','Liquidada','Cancelado','Cancelada')", Param("@Id", idProfissional, "@Ini", inicioMes, "@Fim", fimMes));
            decimal avaliacao = ExecuteScalarDecimal(@"SELECT COALESCE(AVG(CAST(Classificacao AS DECIMAL(10,2))), @Fallback) FROM dbo.Avaliacoes WHERE IdProfissional=@Id", Param("@Id", idProfissional, "@Fallback", p.Avaliacao));

            DateTime proxPagamento = hoje.Day <= 15 ? new DateTime(hoje.Year, hoje.Month, 15) : new DateTime(hoje.Year, hoje.Month, 15).AddMonths(1);
            decimal comissaoMes = totalGerado * p.ComissaoPercentual / 100m;
            return new DashboardResumo
            {
                MarcacoesHoje = marcHoje,
                ServicosSemana = servSemana,
                ServicosMes = servMes,
                TotalGeradoMes = totalGerado,
                ComissaoMes = comissaoMes,
                JaPago = pago,
                PendenteReceber = pendente,
                ComissaoPercentual = p.ComissaoPercentual,
                ProximoPagamento = proxPagamento,
                AvaliacaoMedia = avaliacao
            };
        }

        public static List<MarcacaoInfo> GetProximasMarcacoes(int idProfissional, int limite)
        {
            var lista = new List<MarcacaoInfo>();
            string sql = @"SELECT TOP (@Limite) m.IdMarcacao, m.IdCliente, m.Cliente,
                                  COALESCE(NULLIF(serv.Servicos, N''), m.Servico) AS Servico,
                                  m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.Observacoes, m.DuracaoMinutos, u.Foto AS ClienteFoto
                           FROM dbo.Marcacoes m
                           LEFT JOIN dbo.Usuarios u ON m.IdCliente = u.IdUsuario
                           OUTER APPLY
                           (
                               SELECT STUFF((
                                   SELECT N', ' + COALESCE(NULLIF(ms.NomeServico,N''), s.Nome, N'Serviço')
                                   FROM dbo.MarcacaoServicos ms
                                   LEFT JOIN dbo.Servicos s ON s.IdServico = ms.IdServico
                                   WHERE ms.IdMarcacao = m.IdMarcacao
                                   ORDER BY ms.IdMarcacaoServico
                                   FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, N'') AS Servicos
                           ) serv
                           WHERE m.IdProfissional=@Id AND m.Estado NOT IN ('Cancelado','Cancelada')
                             AND (m.DataMarcacao > CAST(GETDATE() AS DATE) OR (m.DataMarcacao = CAST(GETDATE() AS DATE) AND m.Hora >= CAST(GETDATE() AS TIME)))
                           ORDER BY m.DataMarcacao, m.Hora";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProfissional);
                cmd.Parameters.AddWithValue("@Limite", limite);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(ReadMarcacao(r));
            }
            return lista;
        }

        public static List<MarcacaoInfo> GetMarcacoesPorDia(int idProfissional, DateTime data)
        {
            var lista = new List<MarcacaoInfo>();
            string sql = @"SELECT m.IdMarcacao, m.IdCliente, m.Cliente,
                                  COALESCE(NULLIF(serv.Servicos, N''), m.Servico) AS Servico,
                                  m.DataMarcacao, m.Hora, m.Valor, m.Estado, m.Observacoes, m.DuracaoMinutos, u.Foto AS ClienteFoto
                           FROM dbo.Marcacoes m
                           LEFT JOIN dbo.Usuarios u ON m.IdCliente = u.IdUsuario
                           OUTER APPLY
                           (
                               SELECT STUFF((
                                   SELECT N', ' + COALESCE(NULLIF(ms.NomeServico,N''), s.Nome, N'Serviço')
                                   FROM dbo.MarcacaoServicos ms
                                   LEFT JOIN dbo.Servicos s ON s.IdServico = ms.IdServico
                                   WHERE ms.IdMarcacao = m.IdMarcacao
                                   ORDER BY ms.IdMarcacaoServico
                                   FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, N'') AS Servicos
                           ) serv
                           WHERE m.IdProfissional=@Id AND m.DataMarcacao=@Data
                           ORDER BY m.Hora";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProfissional);
                cmd.Parameters.AddWithValue("@Data", data.Date);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(ReadMarcacao(r));
            }
            return lista;
        }

        public static void CancelarMarcacao(int idMarcacao, int idProfissional)
        {
            string sql = @"
UPDATE dbo.Marcacoes SET Estado='Cancelado' WHERE IdMarcacao=@IdMarcacao AND IdProfissional=@IdProfissional;
UPDATE dbo.Faturas SET Estado='Cancelada' WHERE IdMarcacao=@IdMarcacao;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT @IdProfissional, 'Marcação cancelada: ' + ISNULL(Cliente,'') + ' - ' + ISNULL(Servico,'') + '.', GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@IdMarcacao;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT IdCliente, 'A sua marcação foi cancelada: ' + ISNULL(Servico,'') + ' em ' + CONVERT(NVARCHAR(10), DataMarcacao, 103) + ' às ' + CONVERT(NVARCHAR(5), Hora, 108) + '.', GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@IdMarcacao AND IdCliente IS NOT NULL;";
            ExecuteNonQuery(sql, Param("@IdMarcacao", idMarcacao, "@IdProfissional", idProfissional));
        }

        public static void ConfirmarMarcacao(int idMarcacao, int idProfissional)
        {
            string sql = @"
UPDATE dbo.Marcacoes
   SET Estado='Confirmada'
 WHERE IdMarcacao=@IdMarcacao
   AND IdProfissional=@IdProfissional
   AND ISNULL(Estado,'') NOT IN ('Concluido','Concluida','Concluído','Concluída');
UPDATE dbo.Faturas
   SET Estado=CASE WHEN Estado IS NULL OR Estado='' OR Estado='Cancelada' OR Estado='Cancelado' THEN 'Pendente' ELSE Estado END
 WHERE IdMarcacao=@IdMarcacao;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT @IdProfissional, 'Marcação confirmada: ' + ISNULL(Cliente,'') + ' - ' + ISNULL(Servico,'') + '.', GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@IdMarcacao;
INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
SELECT IdCliente, 'A sua marcação foi confirmada: ' + ISNULL(Servico,'') + ' em ' + CONVERT(NVARCHAR(10), DataMarcacao, 103) + ' às ' + CONVERT(NVARCHAR(5), Hora, 108) + '.', GETDATE(), 0
FROM dbo.Marcacoes WHERE IdMarcacao=@IdMarcacao AND IdCliente IS NOT NULL;";
            ExecuteNonQuery(sql, Param("@IdMarcacao", idMarcacao, "@IdProfissional", idProfissional));
        }

        public static List<ServicoInfo> GetServicos()
        {
            var lista = new List<ServicoInfo>();
            string sql = @"SELECT s.IdServico, s.Nome, s.Categoria, s.DuracaoMinutos, s.Preco, s.Foto,
                                  COALESCE((SELECT AVG(CAST(a.Classificacao AS DECIMAL(10,2))) FROM dbo.Avaliacoes a WHERE a.IdServico=s.IdServico), s.Avaliacao, 5) AS Avaliacao
                           FROM dbo.Servicos s
                           WHERE s.Ativo=1 AND ISNULL(s.Excluido,0)=0
                           ORDER BY s.Categoria, s.Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        lista.Add(new ServicoInfo
                        {
                            IdServico = Convert.ToInt32(r["IdServico"]),
                            Nome = ReadString(r, "Nome"),
                            Categoria = ReadString(r, "Categoria"),
                            DuracaoMinutos = ReadInt(r, "DuracaoMinutos", 60),
                            Preco = ReadDecimal(r, "Preco", 0m),
                            Avaliacao = ReadDecimal(r, "Avaliacao", 5m),
                            Foto = ReadString(r, "Foto")
                        });
                    }
            }
            return lista;
        }

        public static UsuarioInfo ProcurarCliente(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo)) return null;
            string like = "%" + termo.Trim() + "%";
            string sql = @"SELECT TOP 1 IdUsuario, Nome, Email, Foto FROM dbo.Usuarios
                           WHERE TipoUsuario='Cliente' AND Ativo=1
                           AND (Email=@Termo OR Nome LIKE @Like OR Telefone LIKE @Like)
                           ORDER BY CASE WHEN Email=@Termo THEN 0 ELSE 1 END, Nome";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Termo", termo.Trim());
                cmd.Parameters.AddWithValue("@Like", like);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new UsuarioInfo
                        {
                            IdUsuario = Convert.ToInt32(r["IdUsuario"]),
                            Nome = ReadString(r, "Nome"),
                            Email = ReadString(r, "Email"),
                            Foto = ReadString(r, "Foto")
                        };
                    }
                }
            }
            return null;
        }

        public static int CriarMarcacaoComFatura(int idProfissional, UsuarioInfo cliente, bool paraPropriaProfissional, DateTime data, TimeSpan hora, List<ServicoInfo> servicos, string metodoPagamento)
        {
            if (servicos == null || servicos.Count == 0) throw new InvalidOperationException("Escolha pelo menos um procedimento.");
            ProfissionalInfo prof = GetProfissional(idProfissional);
            string nomeCliente = paraPropriaProfissional ? prof.Nome + " (pessoal)" : cliente.Nome;
            object idClienteObj = paraPropriaProfissional || cliente == null ? (object)idProfissional : cliente.IdUsuario;
            string nomesServicos = string.Join(", ", servicos.Select(s => s.Nome).ToArray());
            int duracao = servicos.Sum(s => s.DuracaoMinutos);
            decimal total = servicos.Sum(s => s.Preco);
            int primeiroServico = servicos[0].IdServico;
            string numero = "FAT" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string estadoFatura = "Paga";
            if (!AdminRepository.ProfissionalDisponivel(idProfissional, data.Date, hora, duracao))
                throw new InvalidOperationException("Esta profissional já tem uma marcação neste horário. Escolha outra hora.");

            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        int idMarcacao;
                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Marcacoes
(IdCliente, IdProfissional, IdServico, Cliente, Profissional, Servico, DataMarcacao, Hora, Valor, Estado, Observacoes, DuracaoMinutos, IdEspaco, Espaco)
VALUES (@IdCliente, @IdProfissional, @IdServico, @Cliente, @Profissional, @Servico, @Data, @Hora, @Valor, 'Confirmada', @Obs, @Duracao, 1, N'Sala 1');
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdCliente", idClienteObj);
                            cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                            cmd.Parameters.AddWithValue("@IdServico", primeiroServico);
                            cmd.Parameters.AddWithValue("@Cliente", nomeCliente);
                            cmd.Parameters.AddWithValue("@Profissional", prof.Nome);
                            cmd.Parameters.AddWithValue("@Servico", nomesServicos);
                            cmd.Parameters.AddWithValue("@Data", data.Date);
                            cmd.Parameters.AddWithValue("@Hora", hora);
                            cmd.Parameters.AddWithValue("@Valor", total);
                            cmd.Parameters.AddWithValue("@Obs", paraPropriaProfissional ? "Agendamento pessoal criado pela profissional." : "Agendamento criado pela profissional para a cliente.");
                            cmd.Parameters.AddWithValue("@Duracao", duracao);
                            idMarcacao = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (ServicoInfo serv in servicos)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.MarcacaoServicos (IdMarcacao, IdServico, NomeServico, DuracaoMinutos, Preco)
VALUES (@IdMarcacao, @IdServico, @Nome, @Duracao, @Preco);", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                                cmd.Parameters.AddWithValue("@IdServico", serv.IdServico);
                                cmd.Parameters.AddWithValue("@Nome", serv.Nome);
                                cmd.Parameters.AddWithValue("@Duracao", serv.DuracaoMinutos);
                                cmd.Parameters.AddWithValue("@Preco", serv.Preco);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Faturas
(NumeroFatura, IdMarcacao, IdCliente, Cliente, DataFatura, HoraFatura, Servicos, Subtotal, Desconto, Total, ValorTotal, MetodoPagamento, Estado, IdProfissional, Profissional, ComissaoPercentual)
VALUES (@Numero, @IdMarcacao, @IdCliente, @Cliente, @Data, @Hora, @Servicos, @Subtotal, 0, @Total, @Total, @Metodo, @Estado, @IdProfissional, @Profissional, @Comissao);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@Numero", numero);
                            cmd.Parameters.AddWithValue("@IdMarcacao", idMarcacao);
                            cmd.Parameters.AddWithValue("@IdCliente", idClienteObj);
                            cmd.Parameters.AddWithValue("@Cliente", nomeCliente);
                            cmd.Parameters.AddWithValue("@Data", data.Date);
                            cmd.Parameters.AddWithValue("@Hora", hora);
                            cmd.Parameters.AddWithValue("@Servicos", nomesServicos);
                            cmd.Parameters.AddWithValue("@Subtotal", total);
                            cmd.Parameters.AddWithValue("@Total", total);
                            cmd.Parameters.AddWithValue("@Metodo", metodoPagamento);
                            cmd.Parameters.AddWithValue("@Estado", estadoFatura);
                            cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                            cmd.Parameters.AddWithValue("@Profissional", prof.Nome);
                            cmd.Parameters.AddWithValue("@Comissao", prof.ComissaoPercentual);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO dbo.Notificacoes (IdUsuario, Mensagem, DataNotificacao, Lida)
VALUES (@IdProfissional, @Mensagem, GETDATE(), 0);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@IdProfissional", idProfissional);
                            cmd.Parameters.AddWithValue("@Mensagem", "Nova marcação criada para " + nomeCliente + " em " + data.ToString("dd/MM/yyyy") + " às " + hora.ToString(@"hh\:mm") + ".");
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return idMarcacao;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public static List<FaturaInfo> GetFaturas(int idProfissional, string pesquisa, string estado)
        {
            var lista = new List<FaturaInfo>();
            string filtroEstado = string.IsNullOrWhiteSpace(estado) || estado == "Todos" ? null : estado;
            string filtro = string.IsNullOrWhiteSpace(pesquisa) ? null : "%" + pesquisa.Trim() + "%";
            string sql = @"SELECT f.IdFatura, f.NumeroFatura, f.IdMarcacao, f.IdCliente, f.Cliente, COALESCE(f.Profissional, m.Profissional) AS Profissional,
                                  f.DataFatura, COALESCE(f.HoraFatura, m.Hora) AS HoraFatura, f.Servicos, f.Subtotal, f.Desconto, f.Total,
                                  f.ComissaoPercentual, f.MetodoPagamento, f.Estado
                           FROM dbo.Faturas f
                           LEFT JOIN dbo.Marcacoes m ON f.IdMarcacao = m.IdMarcacao
                           WHERE (f.IdProfissional=@Id OR m.IdProfissional=@Id)
                             AND (@Filtro IS NULL OR f.NumeroFatura LIKE @Filtro OR f.Cliente LIKE @Filtro OR f.Servicos LIKE @Filtro OR f.Estado LIKE @Filtro)
                             AND (@Estado IS NULL OR f.Estado=@Estado)
                           ORDER BY f.DataFatura DESC, COALESCE(f.HoraFatura, m.Hora) DESC, f.IdFatura DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProfissional);
                cmd.Parameters.AddWithValue("@Filtro", (object)filtro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", (object)filtroEstado ?? DBNull.Value);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(ReadFatura(r));
            }
            return lista;
        }

        public static FaturaInfo GetFatura(int idFatura)
        {
            string sql = @"SELECT f.IdFatura, f.NumeroFatura, f.IdMarcacao, f.IdCliente, f.Cliente, COALESCE(f.Profissional, m.Profissional) AS Profissional,
                                  f.DataFatura, COALESCE(f.HoraFatura, m.Hora) AS HoraFatura, f.Servicos, f.Subtotal, f.Desconto, f.Total,
                                  f.ComissaoPercentual, f.MetodoPagamento, f.Estado
                           FROM dbo.Faturas f
                           LEFT JOIN dbo.Marcacoes m ON f.IdMarcacao = m.IdMarcacao
                           WHERE f.IdFatura=@Id";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idFatura);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    if (r.Read()) return ReadFatura(r);
            }
            return null;
        }

        public static List<FaturaInfo> GetHistoricoPagamentos(int idProfissional)
        {
            return GetFaturas(idProfissional, null, "Paga").Take(3).ToList();
        }

        public static Dictionary<string, int> GetDesempenhoMensal(int idProfissional)
        {
            var resultado = new Dictionary<string, int>();
            DateTime baseMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
            for (int i = 0; i < 6; i++)
            {
                DateTime mes = baseMes.AddMonths(i);
                resultado[mes.ToString("MMM", Pt)] = 0;
            }

            string sql = @"SELECT YEAR(DataMarcacao) Ano, MONTH(DataMarcacao) Mes, COUNT(*) Total
                           FROM dbo.Marcacoes
                           WHERE IdProfissional=@Id AND DataMarcacao>=@Inicio AND Estado NOT IN ('Cancelado','Cancelada')
                           GROUP BY YEAR(DataMarcacao), MONTH(DataMarcacao)";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProfissional);
                cmd.Parameters.AddWithValue("@Inicio", baseMes);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DateTime mes = new DateTime(Convert.ToInt32(r["Ano"]), Convert.ToInt32(r["Mes"]), 1);
                        string label = mes.ToString("MMM", Pt);
                        if (resultado.ContainsKey(label)) resultado[label] = Convert.ToInt32(r["Total"]);
                    }
                }
            }
            return resultado;
        }

        public static List<SearchItem> PesquisarTudo(int idProfissional, string termo)
        {
            var lista = new List<SearchItem>();
            if (string.IsNullOrWhiteSpace(termo)) return lista;
            string filtro = "%" + termo.Trim() + "%";

            using (SqlConnection conn = Conexao.Conectar())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 5 Cliente, Servico, Estado, DataMarcacao, Hora FROM dbo.Marcacoes
                                                       WHERE IdProfissional=@Id AND (Cliente LIKE @Filtro OR Servico LIKE @Filtro OR Estado LIKE @Filtro)
                                                       ORDER BY DataMarcacao DESC, Hora DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idProfissional);
                    cmd.Parameters.AddWithValue("@Filtro", filtro);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            DateTime d = Convert.ToDateTime(r["DataMarcacao"]);
                            TimeSpan h = (TimeSpan)r["Hora"];
                            lista.Add(new SearchItem
                            {
                                Tipo = "Marcação",
                                Titulo = ReadString(r, "Cliente") + " · " + ReadString(r, "Servico"),
                                Subtitulo = d.ToString("dd/MM/yyyy") + " às " + h.ToString(@"hh\:mm") + " · " + ReadString(r, "Estado")
                            });
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 5 NumeroFatura, Cliente, Servicos, Estado, Total FROM dbo.Faturas
                                                       WHERE IdProfissional=@Id AND (NumeroFatura LIKE @Filtro OR Cliente LIKE @Filtro OR Servicos LIKE @Filtro OR Estado LIKE @Filtro)
                                                       ORDER BY DataFatura DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idProfissional);
                    cmd.Parameters.AddWithValue("@Filtro", filtro);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            lista.Add(new SearchItem
                            {
                                Tipo = "Fatura",
                                Titulo = ReadString(r, "NumeroFatura") + " · " + ReadString(r, "Cliente"),
                                Subtitulo = ReadString(r, "Servicos") + " · " + FormatarMoeda(ReadDecimal(r, "Total", 0m)) + " · " + ReadString(r, "Estado")
                            });
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 5 Nome, Categoria, Preco FROM dbo.Servicos
                                                       WHERE Ativo=1 AND (Nome LIKE @Filtro OR Categoria LIKE @Filtro)
                                                       ORDER BY Nome", conn))
                {
                    cmd.Parameters.AddWithValue("@Filtro", filtro);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            lista.Add(new SearchItem
                            {
                                Tipo = "Serviço",
                                Titulo = ReadString(r, "Nome"),
                                Subtitulo = ReadString(r, "Categoria") + " · " + FormatarMoeda(ReadDecimal(r, "Preco", 0m))
                            });
                        }
                    }
                }
            }
            return lista.Take(8).ToList();
        }

        public static List<NotificacaoInfo> GetNotificacoes(int idProfissional)
        {
            var lista = new List<NotificacaoInfo>();
            string sql = @"SELECT TOP 20 Id, Mensagem, DataNotificacao, Lida
                           FROM dbo.Notificacoes
                           WHERE IdUsuario=@Id
                           ORDER BY Lida ASC, DataNotificacao DESC";
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProfissional);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        lista.Add(new NotificacaoInfo
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Mensagem = ReadString(r, "Mensagem"),
                            DataNotificacao = Convert.ToDateTime(r["DataNotificacao"]),
                            Lida = Convert.ToBoolean(r["Lida"])
                        });
                    }
            }
            return lista;
        }

        public static int GetNotificacoesNaoLidas(int idProfissional)
        {
            return ExecuteScalarInt("SELECT COUNT(*) FROM dbo.Notificacoes WHERE IdUsuario=@Id AND Lida=0", Param("@Id", idProfissional));
        }

        public static void MarcarNotificacaoComoLida(int idNotificacao)
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE Id=@Id", Param("@Id", idNotificacao));
        }

        public static void MarcarTodasNotificacoesComoLidas(int idProfissional)
        {
            ExecuteNonQuery("UPDATE dbo.Notificacoes SET Lida=1 WHERE IdUsuario=@Id", Param("@Id", idProfissional));
        }

        public static void AtualizarFotoProfissional(int idProfissional, string caminhoFoto)
        {
            ExecuteNonQuery("UPDATE dbo.Usuarios SET Foto=@Foto WHERE IdUsuario=@Id", Param("@Foto", caminhoFoto, "@Id", idProfissional));
            UsuarioLogado.Foto = caminhoFoto;
        }

        public static string FormatarMoeda(decimal valor)
        {
            return valor.ToString("N2", Pt) + " €";
        }

        public static string PrimeiroNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "";
            return nome.Trim().Split(' ')[0];
        }

        private static MarcacaoInfo ReadMarcacao(SqlDataReader r)
        {
            return new MarcacaoInfo
            {
                IdMarcacao = Convert.ToInt32(r["IdMarcacao"]),
                IdCliente = r["IdCliente"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdCliente"]),
                Cliente = ReadString(r, "Cliente"),
                Servico = ReadString(r, "Servico"),
                DataMarcacao = Convert.ToDateTime(r["DataMarcacao"]),
                Hora = (TimeSpan)r["Hora"],
                Valor = ReadDecimal(r, "Valor", 0m),
                Estado = ReadString(r, "Estado"),
                Observacoes = ReadString(r, "Observacoes"),
                DuracaoMinutos = ReadInt(r, "DuracaoMinutos", 60),
                ClienteFoto = ReadString(r, "ClienteFoto")
            };
        }

        private static FaturaInfo ReadFatura(SqlDataReader r)
        {
            TimeSpan? hora = null;
            if (r["HoraFatura"] != DBNull.Value) hora = (TimeSpan)r["HoraFatura"];
            return new FaturaInfo
            {
                IdFatura = Convert.ToInt32(r["IdFatura"]),
                NumeroFatura = ReadString(r, "NumeroFatura"),
                IdMarcacao = r["IdMarcacao"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdMarcacao"]),
                IdCliente = r["IdCliente"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["IdCliente"]),
                Cliente = ReadString(r, "Cliente"),
                Profissional = ReadString(r, "Profissional"),
                DataFatura = Convert.ToDateTime(r["DataFatura"]),
                Hora = hora,
                Servicos = ReadString(r, "Servicos"),
                Subtotal = ReadDecimal(r, "Subtotal", 0m),
                Desconto = ReadDecimal(r, "Desconto", 0m),
                Total = ReadDecimal(r, "Total", 0m),
                ComissaoPercentual = ReadDecimal(r, "ComissaoPercentual", 40m),
                MetodoPagamento = ReadString(r, "MetodoPagamento"),
                Estado = ReadString(r, "Estado")
            };
        }

        public static string ReadString(SqlDataReader r, string column)
        {
            int i = r.GetOrdinal(column);
            if (r.IsDBNull(i)) return "";
            return Convert.ToString(r[i]);
        }

        public static int ReadInt(SqlDataReader r, string column, int fallback)
        {
            int i = r.GetOrdinal(column);
            if (r.IsDBNull(i)) return fallback;
            return Convert.ToInt32(r[i]);
        }

        public static decimal ReadDecimal(SqlDataReader r, string column, decimal fallback)
        {
            int i = r.GetOrdinal(column);
            if (r.IsDBNull(i)) return fallback;
            return Convert.ToDecimal(r[i]);
        }

        private static Dictionary<string, object> Param(params object[] dados)
        {
            var d = new Dictionary<string, object>();
            for (int i = 0; i + 1 < dados.Length; i += 2)
                d[dados[i].ToString()] = dados[i + 1];
            return d;
        }

        private static void ExecuteNonQuery(string sql, Dictionary<string, object> parametros)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parametros != null)
                    foreach (var p in parametros)
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static int ExecuteScalarInt(string sql, Dictionary<string, object> parametros)
        {
            object result = ExecuteScalar(sql, parametros);
            if (result == null || result == DBNull.Value) return 0;
            return Convert.ToInt32(result);
        }

        private static decimal ExecuteScalarDecimal(string sql, Dictionary<string, object> parametros)
        {
            object result = ExecuteScalar(sql, parametros);
            if (result == null || result == DBNull.Value) return 0m;
            return Convert.ToDecimal(result);
        }

        private static object ExecuteScalar(string sql, Dictionary<string, object> parametros)
        {
            using (SqlConnection conn = Conexao.Conectar())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parametros != null)
                    foreach (var p in parametros)
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
