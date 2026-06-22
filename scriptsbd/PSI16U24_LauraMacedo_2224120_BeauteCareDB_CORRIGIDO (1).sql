-- [BeauteCareDB]
USE [master]
GO

/****** Object:  Database [BeauteCareDB]    Script Date: 6/22/2026 7:48:49 PM ******/
IF DB_ID(N'BeauteCareDB') IS NULL
BEGIN
    CREATE DATABASE [BeauteCareDB];
END
GO

USE [BeauteCareDB]
GO

-- [dbo].[Avaliacoes]
USE [BeauteCareDB]
GO

/****** Object:  Table [dbo].[Avaliacoes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Avaliacoes](
	[IdAvaliacao] [int] IDENTITY(1,1) NOT NULL,
	[IdMarcacao] [int] NOT NULL,
	[IdCliente] [int] NOT NULL,
	[IdProfissional] [int] NOT NULL,
	[IdServico] [int] NOT NULL,
	[Classificacao] [decimal](3, 2) NOT NULL,
	[Comentario] [nvarchar](max) NULL,
	[DataAvaliacao] [datetime] NOT NULL,
	[NotaProfissional] [int] NULL,
	[NotaEspaco] [int] NULL,
	[NotaServico] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[IdAvaliacao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [dbo].[Clientes]
/****** Object:  Table [dbo].[Clientes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[IdCliente] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Observacoes] [nvarchar](max) NULL,
	[PontosCliente] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [dbo].[Cupoes]
/****** Object:  Table [dbo].[Cupoes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cupoes](
	[IdCupao] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](40) NOT NULL,
	[IdCliente] [int] NOT NULL,
	[Tipo] [nvarchar](30) NOT NULL,
	[IdServico] [int] NULL,
	[NomeServico] [nvarchar](120) NULL,
	[PercentualDesconto] [decimal](5, 2) NOT NULL,
	[ValorDesconto] [decimal](10, 2) NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[DataValidade] [datetime] NOT NULL,
	[Usado] [bit] NOT NULL,
	[DataUso] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[IdCupao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Espacos]
/****** Object:  Table [dbo].[Espacos]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Espacos](
	[IdEspaco] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](80) NOT NULL,
	[Descricao] [nvarchar](200) NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdEspaco] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Faturas]
/****** Object:  Table [dbo].[Faturas]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Faturas](
	[IdFatura] [int] IDENTITY(1,1) NOT NULL,
	[NumeroFatura] [nvarchar](30) NOT NULL,
	[IdMarcacao] [int] NULL,
	[IdCliente] [int] NOT NULL,
	[DataFatura] [datetime] NOT NULL,
	[ValorTotal] [decimal](10, 2) NOT NULL,
	[Estado] [nvarchar](30) NOT NULL,
	[IdPagamento] [int] NULL,
	[Cliente] [nvarchar](120) NOT NULL,
	[Servicos] [nvarchar](300) NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
	[Desconto] [decimal](10, 2) NOT NULL,
	[Total] [decimal](10, 2) NOT NULL,
	[MetodoPagamento] [nvarchar](60) NOT NULL,
	[IdProfissional] [int] NULL,
	[Profissional] [nvarchar](120) NULL,
	[ComissaoPercentual] [decimal](5, 2) NOT NULL,
	[HoraFatura] [time](7) NULL,
	[IdCupao] [int] NULL,
	[CodigoCupao] [nvarchar](40) NULL,
	[DataCriacao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdFatura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Importacoes]
/****** Object:  Table [dbo].[Importacoes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Importacoes](
	[IdImportacao] [int] IDENTITY(1,1) NOT NULL,
	[Tipo] [nvarchar](60) NOT NULL,
	[Ficheiro] [nvarchar](400) NULL,
	[LinhasImportadas] [int] NOT NULL,
	[DataImportacao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdImportacao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[MarcacaoServicos]
/****** Object:  Table [dbo].[MarcacaoServicos]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MarcacaoServicos](
	[IdMarcacaoServico] [int] IDENTITY(1,1) NOT NULL,
	[IdMarcacao] [int] NOT NULL,
	[IdServico] [int] NOT NULL,
	[NomeServico] [nvarchar](120) NOT NULL,
	[DuracaoMinutos] [int] NOT NULL,
	[Preco] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMarcacaoServico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Marcacoes]
/****** Object:  Table [dbo].[Marcacoes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Marcacoes](
	[IdMarcacao] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NULL,
	[IdServico] [int] NOT NULL,
	[IdProfissional] [int] NOT NULL,
	[IdEspaco] [int] NOT NULL,
	[DataMarcacao] [date] NOT NULL,
	[Hora] [time](7) NOT NULL,
	[Cliente] [nvarchar](120) NOT NULL,
	[Servico] [nvarchar](120) NOT NULL,
	[Profissional] [nvarchar](120) NOT NULL,
	[Espaco] [nvarchar](80) NOT NULL,
	[Valor] [decimal](10, 2) NOT NULL,
	[Estado] [nvarchar](30) NOT NULL,
	[Observacoes] [nvarchar](max) NULL,
	[CriadoEm] [datetime] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[DuracaoMinutos] [int] NOT NULL,
	[Avaliada] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMarcacao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [dbo].[Mensagens]
/****** Object:  Table [dbo].[Mensagens]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mensagens](
	[IdMensagem] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [int] NULL,
	[DestinatarioTipo] [nvarchar](30) NULL,
	[Canal] [nvarchar](20) NULL,
	[Assunto] [nvarchar](150) NULL,
	[Mensagem] [nvarchar](max) NULL,
	[DataCriacao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMensagem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [dbo].[MensagensEnviadas]
/****** Object:  Table [dbo].[MensagensEnviadas]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MensagensEnviadas](
	[IdMensagem] [int] IDENTITY(1,1) NOT NULL,
	[IdRemetente] [int] NULL,
	[IdDestinatario] [int] NULL,
	[DestinatarioNome] [nvarchar](120) NULL,
	[Canal] [nvarchar](30) NOT NULL,
	[Assunto] [nvarchar](160) NULL,
	[Mensagem] [nvarchar](700) NOT NULL,
	[DataEnvio] [datetime] NOT NULL,
	[EstadoEnvio] [nvarchar](60) NULL,
	[ErroEnvio] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMensagem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Notificacoes]
/****** Object:  Table [dbo].[Notificacoes]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notificacoes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Mensagem] [nvarchar](300) NOT NULL,
	[Tipo] [nvarchar](50) NOT NULL,
	[IdUsuario] [int] NULL,
	[DataNotificacao] [datetime] NOT NULL,
	[Lida] [bit] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[Titulo] [nvarchar](150) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Pagamentos]
/****** Object:  Table [dbo].[Pagamentos]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pagamentos](
	[IdPagamento] [int] IDENTITY(1,1) NOT NULL,
	[IdFatura] [int] NOT NULL,
	[DataPagamento] [datetime] NOT NULL,
	[Valor] [decimal](10, 2) NOT NULL,
	[MetodoPagamento] [nvarchar](60) NOT NULL,
	[Estado] [nvarchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdPagamento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Profissionais]
/****** Object:  Table [dbo].[Profissionais]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profissionais](
	[IdProfissional] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Especialidade] [nvarchar](120) NULL,
	[Avaliacao] [decimal](3, 2) NOT NULL,
	[ComissaoPercentual] [decimal](5, 2) NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdProfissional] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[ProfissionalServicos]
/****** Object:  Table [dbo].[ProfissionalServicos]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfissionalServicos](
	[IdProfissional] [int] NOT NULL,
	[IdServico] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdProfissional] ASC,
	[IdServico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[ServicoCategorias]
/****** Object:  Table [dbo].[ServicoCategorias]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServicoCategorias](
	[IdCategoria] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](80) NOT NULL,
	[Descricao] [nvarchar](250) NULL,
	[Foto] [nvarchar](400) NULL,
	[Ativa] [bit] NOT NULL,
	[Ordem] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Servicos]
/****** Object:  Table [dbo].[Servicos]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Servicos](
	[IdServico] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](120) NOT NULL,
	[Categoria] [nvarchar](80) NULL,
	[Descricao] [nvarchar](max) NULL,
	[DuracaoMinutos] [int] NOT NULL,
	[Preco] [decimal](10, 2) NOT NULL,
	[Foto] [nvarchar](400) NULL,
	[Ativo] [bit] NOT NULL,
	[Popularidade] [int] NOT NULL,
	[Avaliacao] [decimal](3, 2) NOT NULL,
	[Excluido] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdServico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [dbo].[Usuarios]
/****** Object:  Table [dbo].[Usuarios]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](120) NOT NULL,
	[Email] [nvarchar](180) NOT NULL,
	[Telefone] [nvarchar](30) NULL,
	[Senha] [nvarchar](128) NOT NULL,
	[TipoUsuario] [nvarchar](30) NOT NULL,
	[Ativo] [bit] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UltimaVisita] [datetime] NULL,
	[Foto] [nvarchar](400) NULL,
	[Especialidade] [nvarchar](120) NULL,
	[Avaliacao] [decimal](3, 2) NOT NULL,
	[ComissaoPercentual] [decimal](5, 2) NOT NULL,
	[PontosCliente] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Usuarios_Backup_20260616152626]
/****** Object:  Table [dbo].[Usuarios_Backup_20260616152626]    Script Date: 6/22/2026 7:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios_Backup_20260616152626](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](120) NOT NULL,
	[Email] [nvarchar](160) NOT NULL,
	[Telefone] [nvarchar](40) NULL,
	[Senha] [nvarchar](128) NOT NULL,
	[TipoUsuario] [nvarchar](30) NOT NULL,
	[Foto] [nvarchar](400) NULL,
	[Ativo] [bit] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UltimaVisita] [datetime] NULL,
	[Especialidade] [nvarchar](120) NULL,
	[Avaliacao] [decimal](3, 2) NOT NULL,
	[ComissaoPercentual] [decimal](5, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- [dbo].[Clientes]
SET IDENTITY_INSERT [dbo].[Clientes] ON 
INSERT [dbo].[Clientes] ([IdCliente], [IdUsuario], [Observacoes], [PontosCliente]) VALUES (1, 2, NULL, 0)
INSERT [dbo].[Clientes] ([IdCliente], [IdUsuario], [Observacoes], [PontosCliente]) VALUES (2, 5, NULL, 0)
SET IDENTITY_INSERT [dbo].[Clientes] OFF
GO

-- [dbo].[Cupoes]
SET IDENTITY_INSERT [dbo].[Cupoes] ON 
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (1, N'PROMO0626-2-4', 2, N'Promocao', 4, N'Design de sobrancelhas', CAST(20.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-20T12:22:44.643' AS DateTime), CAST(N'2026-07-20T12:22:44.643' AS DateTime), 0, NULL)
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (2, N'PROMO0626-2-2', 2, N'Promocao', 2, N'Manicure completa', CAST(20.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-21T13:22:43.953' AS DateTime), CAST(N'2026-07-21T13:22:43.953' AS DateTime), 0, NULL)
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (3, N'PROMO0626-2-3', 2, N'Promocao', 3, N'Massagem relaxante', CAST(20.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-21T16:13:13.817' AS DateTime), CAST(N'2026-07-21T16:13:13.817' AS DateTime), 0, NULL)
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (4, N'PROMO0626-2-7', 2, N'Promocao', 7, N'masagem capilar', CAST(20.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-21T20:58:55.967' AS DateTime), CAST(N'2026-07-21T20:58:55.967' AS DateTime), 1, CAST(N'2026-06-21T21:04:38.120' AS DateTime))
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (5, N'PROMO0626-14-3', 14, N'Promocao', 3, N'Massagem relaxante', CAST(20.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-22T09:53:37.243' AS DateTime), CAST(N'2026-07-22T09:53:37.243' AS DateTime), 1, CAST(N'2026-06-22T09:58:09.833' AS DateTime))
INSERT [dbo].[Cupoes] ([IdCupao], [Codigo], [IdCliente], [Tipo], [IdServico], [NomeServico], [PercentualDesconto], [ValorDesconto], [DataCriacao], [DataValidade], [Usado], [DataUso]) VALUES (6, N'PROMO0626-14-8', 14, N'Promocao', 8, N'Micro Pigmentacao', CAST(35.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-06-22T11:05:15.100' AS DateTime), CAST(N'2026-07-22T11:05:15.100' AS DateTime), 1, CAST(N'2026-06-22T19:00:47.390' AS DateTime))
SET IDENTITY_INSERT [dbo].[Cupoes] OFF
GO

-- [dbo].[Espacos]
SET IDENTITY_INSERT [dbo].[Espacos] ON 
INSERT [dbo].[Espacos] ([IdEspaco], [Nome], [Descricao], [Ativo]) VALUES (1, N'Sala 1', N'Sala facial', 1)
INSERT [dbo].[Espacos] ([IdEspaco], [Nome], [Descricao], [Ativo]) VALUES (2, N'Sala 2', N'Sala corporal', 1)
INSERT [dbo].[Espacos] ([IdEspaco], [Nome], [Descricao], [Ativo]) VALUES (3, N'Espaço Unhas', N'Bancada de manicure', 1)
SET IDENTITY_INSERT [dbo].[Espacos] OFF
GO

-- [dbo].[Faturas]
SET IDENTITY_INSERT [dbo].[Faturas] ON 
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (1, N'FAT2026/0001', 1, 2, CAST(N'2026-06-13T20:16:02.603' AS DateTime), CAST(35.00 AS Decimal(10, 2)), N'Paga', NULL, N'', N'', CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), N'Multibanco', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'10:00:00' AS Time), NULL, NULL, CAST(N'2026-06-21T15:54:06.557' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (10, N'FAT2026/0002', 15, 10, CAST(N'2026-06-21T00:00:00.000' AS DateTime), CAST(45.00 AS Decimal(10, 2)), N'Pendente', NULL, N'Kaliny Pessoa', N'Massagem relaxante', CAST(45.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(45.00 AS Decimal(10, 2)), N'Cartão', 7, N'Inês Rocha', CAST(42.00 AS Decimal(5, 2)), CAST(N'09:00:00' AS Time), NULL, NULL, CAST(N'2026-06-21T18:19:59.757' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (11, N'FAT20260621182304', 16, 2, CAST(N'2026-06-22T00:00:00.000' AS DateTime), CAST(35.00 AS Decimal(10, 2)), N'Paga', NULL, N'Maria Silva', N'Limpeza de pele', CAST(35.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), N'MBWay', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'10:00:00' AS Time), NULL, NULL, CAST(N'2026-06-21T18:23:04.563' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (12, N'FAT2026/0004', 18, 1, CAST(N'2026-06-25T00:00:00.000' AS DateTime), CAST(150.00 AS Decimal(10, 2)), N'Pendente', NULL, N'Laura Macedo (admin)', N'masagem capilar', CAST(150.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(150.00 AS Decimal(10, 2)), N'Cartão', 5, N'Juliana Santos', CAST(40.00 AS Decimal(5, 2)), CAST(N'09:00:00' AS Time), NULL, NULL, CAST(N'2026-06-21T18:53:18.890' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (13, N'FAT2026/0005', 19, 4, CAST(N'2026-06-26T00:00:00.000' AS DateTime), CAST(20.00 AS Decimal(10, 2)), N'Pendente', NULL, N'Carla Mendes', N'Manicure completa', CAST(20.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), N'Cartão', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'11:00:00' AS Time), NULL, NULL, CAST(N'2026-06-21T18:53:49.803' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (14, N'FAT20260621210438', 22, 2, CAST(N'2026-06-23T00:00:00.000' AS DateTime), CAST(185.00 AS Decimal(10, 2)), N'Pendente', NULL, N'Maria Silva', N'Manicure completa, Massagem relaxante, masagem capilar', CAST(215.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), CAST(185.00 AS Decimal(10, 2)), N'Cartão', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'12:00:00' AS Time), 4, N'PROMO0626-2-7', CAST(N'2026-06-21T21:04:38.110' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (15, N'FAT20260622095809', 23, 14, CAST(N'2026-06-24T00:00:00.000' AS DateTime), CAST(78.00 AS Decimal(10, 2)), N'Cancelada', NULL, N'Lais Macedo', N'Pedicure, Manicure completa, Massagem relaxante', CAST(87.00 AS Decimal(10, 2)), CAST(9.00 AS Decimal(10, 2)), CAST(78.00 AS Decimal(10, 2)), N'MBWay', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'11:00:00' AS Time), 5, N'PROMO0626-14-3', CAST(N'2026-06-22T09:58:09.827' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (16, N'FAT20260622095933', 24, 14, CAST(N'2026-06-23T00:00:00.000' AS DateTime), CAST(150.00 AS Decimal(10, 2)), N'Pendente', NULL, N'Lais Macedo', N'masagem capilar', CAST(150.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(150.00 AS Decimal(10, 2)), N'Dinheiro', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'10:00:00' AS Time), NULL, NULL, CAST(N'2026-06-22T09:59:33.507' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (17, N'FAT20260622100205', 27, 14, CAST(N'2026-06-24T00:00:00.000' AS DateTime), CAST(35.00 AS Decimal(10, 2)), N'Paga', NULL, N'Lais Macedo', N'Limpeza de pele', CAST(35.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), N'MBWay', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'16:00:00' AS Time), NULL, NULL, CAST(N'2026-06-22T10:02:05.583' AS DateTime))
INSERT [dbo].[Faturas] ([IdFatura], [NumeroFatura], [IdMarcacao], [IdCliente], [DataFatura], [ValorTotal], [Estado], [IdPagamento], [Cliente], [Servicos], [Subtotal], [Desconto], [Total], [MetodoPagamento], [IdProfissional], [Profissional], [ComissaoPercentual], [HoraFatura], [IdCupao], [CodigoCupao], [DataCriacao]) VALUES (18, N'FAT20260622190047', 28, 14, CAST(N'2026-06-27T00:00:00.000' AS DateTime), CAST(65.00 AS Decimal(10, 2)), N'Paga', NULL, N'Lais Macedo', N'Micro Pigmentacao', CAST(100.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), CAST(65.00 AS Decimal(10, 2)), N'Cartão', 3, N'Ana Costa', CAST(40.00 AS Decimal(5, 2)), CAST(N'10:00:00' AS Time), 6, N'PROMO0626-14-8', CAST(N'2026-06-22T19:00:47.387' AS DateTime))
SET IDENTITY_INSERT [dbo].[Faturas] OFF
GO

-- [dbo].[MarcacaoServicos]
SET IDENTITY_INSERT [dbo].[MarcacaoServicos] ON 
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (2, 16, 1, N'Limpeza de pele', 60, CAST(35.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (3, 22, 2, N'Manicure completa', 45, CAST(20.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (4, 22, 3, N'Massagem relaxante', 60, CAST(45.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (5, 22, 7, N'masagem capilar', 60, CAST(150.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (6, 23, 5, N'Pedicure', 45, CAST(22.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (7, 23, 2, N'Manicure completa', 45, CAST(20.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (8, 23, 3, N'Massagem relaxante', 60, CAST(45.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (9, 24, 7, N'masagem capilar', 60, CAST(150.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (10, 27, 1, N'Limpeza de pele', 60, CAST(35.00 AS Decimal(10, 2)))
INSERT [dbo].[MarcacaoServicos] ([IdMarcacaoServico], [IdMarcacao], [IdServico], [NomeServico], [DuracaoMinutos], [Preco]) VALUES (11, 28, 8, N'Micro Pigmentacao', 60, CAST(100.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[MarcacaoServicos] OFF
GO

-- [dbo].[Marcacoes]
SET IDENTITY_INSERT [dbo].[Marcacoes] ON 
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (1, 2, 1, 3, 1, CAST(N'2026-06-13' AS Date), CAST(N'10:00:00' AS Time), N'Maria Silva', N'Limpeza de pele', N'Ana Costa', N'Sala 1', CAST(35.00 AS Decimal(10, 2)), N'Cancelada', NULL, CAST(N'2026-06-13T20:16:02.600' AS DateTime), CAST(N'2026-06-16T15:26:26.540' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (15, 10, 3, 7, 1, CAST(N'2026-06-21' AS Date), CAST(N'09:00:00' AS Time), N'Kaliny Pessoa', N'Massagem relaxante', N'Inês Rocha', N'Sala 1', CAST(45.00 AS Decimal(10, 2)), N'Confirmada', N'Criada pela administração', CAST(N'2026-06-21T18:19:59.703' AS DateTime), CAST(N'2026-06-21T18:19:59.703' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (16, 2, 1, 3, 1, CAST(N'2026-06-22' AS Date), CAST(N'10:00:00' AS Time), N'Maria Silva', N'Limpeza de pele', N'Ana Costa', N'Sala 1', CAST(35.00 AS Decimal(10, 2)), N'Confirmada', N'', CAST(N'2026-06-21T18:23:04.523' AS DateTime), CAST(N'2026-06-21T18:23:04.523' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (18, 1, 7, 5, 1, CAST(N'2026-06-25' AS Date), CAST(N'09:00:00' AS Time), N'Laura Macedo (admin)', N'masagem capilar', N'Juliana Santos', N'Sala 1', CAST(150.00 AS Decimal(10, 2)), N'Confirmada', N'Criada pela administração para a própria admin', CAST(N'2026-06-21T18:53:18.853' AS DateTime), CAST(N'2026-06-21T18:53:18.853' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (19, 4, 2, 3, 1, CAST(N'2026-06-26' AS Date), CAST(N'11:00:00' AS Time), N'Carla Mendes', N'Manicure completa', N'Ana Costa', N'Sala 1', CAST(20.00 AS Decimal(10, 2)), N'Confirmada', N'Criada pela administração', CAST(N'2026-06-21T18:53:49.783' AS DateTime), CAST(N'2026-06-21T18:53:49.783' AS DateTime), 45, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (22, 2, 2, 3, 1, CAST(N'2026-06-23' AS Date), CAST(N'12:00:00' AS Time), N'Maria Silva', N'Manicure completa, Massagem relaxante, masagem capilar', N'Ana Costa', N'Sala 1', CAST(185.00 AS Decimal(10, 2)), N'Confirmada', N'Cupão: PROMO0626-2-7', CAST(N'2026-06-21T21:04:38.070' AS DateTime), CAST(N'2026-06-21T21:04:38.070' AS DateTime), 165, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (23, 14, 5, 3, 1, CAST(N'2026-06-24' AS Date), CAST(N'11:00:00' AS Time), N'Lais Macedo', N'Pedicure, Manicure completa, Massagem relaxante', N'Ana Costa', N'Sala 1', CAST(78.00 AS Decimal(10, 2)), N'Cancelado', N'Cupão: PROMO0626-14-3', CAST(N'2026-06-22T09:58:09.813' AS DateTime), CAST(N'2026-06-22T09:58:09.813' AS DateTime), 150, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (24, 14, 7, 3, 1, CAST(N'2026-06-23' AS Date), CAST(N'10:00:00' AS Time), N'Lais Macedo', N'masagem capilar', N'Ana Costa', N'Sala 1', CAST(150.00 AS Decimal(10, 2)), N'Confirmada', N'', CAST(N'2026-06-22T09:59:33.473' AS DateTime), CAST(N'2026-06-22T09:59:33.473' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (27, 14, 1, 3, 1, CAST(N'2026-06-24' AS Date), CAST(N'16:00:00' AS Time), N'Lais Macedo', N'Limpeza de pele', N'Ana Costa', N'Sala 1', CAST(35.00 AS Decimal(10, 2)), N'Confirmada', N'', CAST(N'2026-06-22T10:02:05.573' AS DateTime), CAST(N'2026-06-22T10:02:05.573' AS DateTime), 60, 0)
INSERT [dbo].[Marcacoes] ([IdMarcacao], [IdCliente], [IdServico], [IdProfissional], [IdEspaco], [DataMarcacao], [Hora], [Cliente], [Servico], [Profissional], [Espaco], [Valor], [Estado], [Observacoes], [CriadoEm], [DataCriacao], [DuracaoMinutos], [Avaliada]) VALUES (28, 14, 8, 3, 1, CAST(N'2026-06-27' AS Date), CAST(N'10:00:00' AS Time), N'Lais Macedo', N'Micro Pigmentacao', N'Ana Costa', N'Sala 1', CAST(65.00 AS Decimal(10, 2)), N'Confirmada', N'Cupão: PROMO0626-14-8', CAST(N'2026-06-22T19:00:47.380' AS DateTime), CAST(N'2026-06-22T19:00:47.380' AS DateTime), 60, 0)
SET IDENTITY_INSERT [dbo].[Marcacoes] OFF
GO

-- [dbo].[MensagensEnviadas]
SET IDENTITY_INSERT [dbo].[MensagensEnviadas] ON 
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (1, 1, 9, N'Lais Macedo', N'Email', N'Pagamento', N'Seu pagamento sera realizado dia 5.', CAST(N'2026-06-21T12:01:37.887' AS DateTime), NULL, NULL)
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (2, 1, 8, N'Urania Macedo', N'Email', N'oioi', N'me pague.', CAST(N'2026-06-21T15:55:46.190' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (3, 1, 9, N'Lais Macedo', N'Email', N'oioi', N'me pague', CAST(N'2026-06-21T15:56:47.583' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (4, 1, 10, N'Kaliny Pessoa', N'Email', N'oioi', N'me pague caloteira', CAST(N'2026-06-21T16:00:15.957' AS DateTime), N'Email não enviado', N'Configure SmtpHost, SmtpUser, SmtpPass e SmtpFrom no App.config para envio real de email.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (5, 1, 9, N'Lais Macedo', N'Email', N'', N'me mande120 euros agora', CAST(N'2026-06-21T17:14:35.030' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (6, 1, 5, N'Juliana Santos', N'Email', N'', N'oi', CAST(N'2026-06-21T17:16:33.893' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (7, 1, 8, N'Urania Macedo', N'Email', N'', N'vai vim quando aqui no salao?', CAST(N'2026-06-21T18:16:52.943' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (8, 1, 10, N'Kaliny Pessoa', N'Email', N'oi', N'oi kaliny', CAST(N'2026-06-21T18:17:17.820' AS DateTime), N'SMS não enviado', N'Configure TwilioAccountSid, TwilioAuthToken e TwilioFromNumber no App.config para envio real de SMS.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (9, 1, 8, N'Urania Macedo', N'Email', N'oi', N'oioioi', CAST(N'2026-06-21T22:06:53.770' AS DateTime), N'Email enviado', N'')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (10, 1, 8, N'Urania Macedo', N'Email', N'oi', N'me mande 30 euros', CAST(N'2026-06-21T22:08:46.727' AS DateTime), N'Email enviado', N'')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (11, 1, 9, N'Lais Macedo', N'Email', N'oi', N'me pague, caloteira', CAST(N'2026-06-22T09:44:05.040' AS DateTime), N'Email não enviado', N'Configure SmtpHost, SmtpUser, SmtpPass e SmtpFrom no App.config para envio real de email.')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (12, 1, 14, N'Lais Macedo', N'Email', N'me pague', N'Lais me pague ag', CAST(N'2026-06-22T10:06:25.210' AS DateTime), N'Email enviado', N'')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (13, 1, 14, N'Lais Macedo', N'Email', N'', N'oioi', CAST(N'2026-06-22T10:07:04.843' AS DateTime), N'Email enviado', N'')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (14, 1, 8, N'Urania Macedo', N'Email', N'', N'oi senhoira sou eu beauteccare', CAST(N'2026-06-22T10:08:31.083' AS DateTime), N'Email enviado', N'')
INSERT [dbo].[MensagensEnviadas] ([IdMensagem], [IdRemetente], [IdDestinatario], [DestinatarioNome], [Canal], [Assunto], [Mensagem], [DataEnvio], [EstadoEnvio], [ErroEnvio]) VALUES (15, 1, 12, N'Emanoel Macedo', N'Email', N'', N'oi emanoeeel em espanha', CAST(N'2026-06-22T10:21:45.593' AS DateTime), N'Email enviado', N'')
SET IDENTITY_INSERT [dbo].[MensagensEnviadas] OFF
GO

-- [dbo].[Notificacoes]
SET IDENTITY_INSERT [dbo].[Notificacoes] ON 
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (1, N'Há marcações para hoje.', N'Marcacao', 1, CAST(N'2026-06-13T20:16:02.603' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (2, N'Existe faturação pendente para acompanhar.', N'Fatura', 1, CAST(N'2026-06-13T20:16:02.603' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (3, N'Cliente inativo identificado: Carla Mendes.', N'Cliente', 1, CAST(N'2026-06-13T20:16:02.603' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (4, N'Cliente cancelou a marcação #1', N'Marcacao', 1, CAST(N'2026-06-14T11:41:09.423' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (5, N'Cliente cancelou a marcação #1', N'Marcacao', 1, CAST(N'2026-06-14T11:41:11.577' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (6, N'Bem-vinda ao seu painel profissional. Confira as suas marcações de hoje.', N'Sistema', 3, CAST(N'2026-06-16T16:45:26.793' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (7, N'Bem-vinda ao seu painel profissional. Confira as suas marcações de hoje.', N'Sistema', 5, CAST(N'2026-06-16T16:45:26.793' AS DateTime), 0, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (8, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 2, CAST(N'2026-06-20T11:44:35.197' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (9, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 4, CAST(N'2026-06-20T11:44:35.197' AS DateTime), 0, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (10, N'Agenda de hoje pronta para acompanhar.', N'Sistema', 1, CAST(N'2026-06-20T21:57:16.847' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (11, N'Clientes inativos precisam de contacto.', N'Sistema', 1, CAST(N'2026-06-20T21:57:16.853' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (12, N'Bem-vinda ao seu painel profissional. Confira as suas marcações de hoje.', N'Sistema', 7, CAST(N'2026-06-20T22:32:52.253' AS DateTime), 0, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (13, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 6, CAST(N'2026-06-20T22:32:52.293' AS DateTime), 0, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (15, N'Há marcações pendentes para confirmar.', N'Sistema', 1, CAST(N'2026-06-21T12:44:24.073' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (16, N'Cliente inativo há mais de 30 dias identificado.', N'Sistema', 1, CAST(N'2026-06-21T09:44:24.073' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (18, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 8, CAST(N'2026-06-21T12:44:24.193' AS DateTime), 0, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (19, N'Clientes inativos precisam de contacto.', N'Sistema', 1, CAST(N'2026-06-21T12:44:24.287' AS DateTime), 1, CAST(N'2026-06-21T15:44:16.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (23, N'me pague.', N'Sistema', 8, CAST(N'2026-06-21T15:55:46.190' AS DateTime), 0, CAST(N'2026-06-21T15:55:46.190' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (24, N'me pague', N'Sistema', 9, CAST(N'2026-06-21T15:56:47.610' AS DateTime), 0, CAST(N'2026-06-21T15:56:47.610' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (25, N'me pague caloteira', N'Sistema', 10, CAST(N'2026-06-21T16:00:15.980' AS DateTime), 0, CAST(N'2026-06-21T16:00:15.980' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (26, N'me mande120 euros agora', N'Sistema', 9, CAST(N'2026-06-21T17:14:35.053' AS DateTime), 0, CAST(N'2026-06-21T17:14:35.053' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (27, N'oi', N'Sistema', 5, CAST(N'2026-06-21T17:16:33.917' AS DateTime), 0, CAST(N'2026-06-21T17:16:33.917' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (28, N'vai vim quando aqui no salao?', N'Sistema', 8, CAST(N'2026-06-21T18:16:52.970' AS DateTime), 0, CAST(N'2026-06-21T18:16:52.970' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (29, N'oi kaliny', N'Sistema', 10, CAST(N'2026-06-21T18:17:17.847' AS DateTime), 0, CAST(N'2026-06-21T18:17:17.847' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (30, N'Marcação criada para 22/06/2026 às 10:00.', N'Sistema', 2, CAST(N'2026-06-21T18:23:04.573' AS DateTime), 0, CAST(N'2026-06-21T18:23:04.573' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (31, N'Nova marcação de Maria Silva para 22/06/2026 às 10:00.', N'Sistema', 3, CAST(N'2026-06-21T18:23:04.573' AS DateTime), 0, CAST(N'2026-06-21T18:23:04.573' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (32, N'Tem uma marcação agendada. Confira o horário para não se atrasar.', N'Sistema', 2, CAST(N'2026-06-21T18:23:10.523' AS DateTime), 1, CAST(N'2026-06-21T18:23:10.523' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (33, N'Há marcações pendentes para confirmar ou acompanhar.', N'Sistema', 3, CAST(N'2026-06-21T18:24:07.790' AS DateTime), 0, CAST(N'2026-06-21T18:24:07.790' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (34, N'Marcação confirmada: Maria Silva - Limpeza de pele.', N'Sistema', 3, CAST(N'2026-06-21T19:41:27.640' AS DateTime), 0, CAST(N'2026-06-21T19:41:27.640' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (35, N'Marcação criada para 23/06/2026 às 12:00.', N'Sistema', 2, CAST(N'2026-06-21T21:04:38.130' AS DateTime), 0, CAST(N'2026-06-21T21:04:38.130' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (36, N'Nova marcação de Maria Silva para 23/06/2026 às 12:00.', N'Sistema', 3, CAST(N'2026-06-21T21:04:38.130' AS DateTime), 0, CAST(N'2026-06-21T21:04:38.130' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (37, N'Marcação confirmada: Maria Silva - Manicure completa, Massagem relaxante, masagem capilar.', N'Sistema', 3, CAST(N'2026-06-21T21:10:57.667' AS DateTime), 0, CAST(N'2026-06-21T21:10:57.667' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (38, N'Marcação cancelada: Maria Silva - Manicure completa, Massagem relaxante, masagem capilar.', N'Sistema', 3, CAST(N'2026-06-21T21:11:08.180' AS DateTime), 0, CAST(N'2026-06-21T21:11:08.180' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (39, N'oioioi', N'Sistema', 8, CAST(N'2026-06-21T22:06:53.797' AS DateTime), 0, CAST(N'2026-06-21T22:06:53.797' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (40, N'me mande 30 euros', N'Sistema', 8, CAST(N'2026-06-21T22:08:46.750' AS DateTime), 0, CAST(N'2026-06-21T22:08:46.750' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (41, N'Bem-vinda à BeautéCare! A sua conta foi criada com sucesso.', N'Sistema', 13, CAST(N'2026-06-21T22:42:32.807' AS DateTime), 0, CAST(N'2026-06-21T22:42:32.807' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (42, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 13, CAST(N'2026-06-21T22:42:47.307' AS DateTime), 0, CAST(N'2026-06-21T22:42:47.307' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (43, N'Marcação confirmada: Maria Silva - Limpeza de pele.', N'Sistema', 3, CAST(N'2026-06-21T22:45:48.330' AS DateTime), 0, CAST(N'2026-06-21T22:45:48.330' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (44, N'A sua marcação foi confirmada: Limpeza de pele em 22/06/2026 às 10:00.', N'Sistema', 2, CAST(N'2026-06-21T22:45:48.333' AS DateTime), 0, CAST(N'2026-06-21T22:45:48.333' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (45, N'Marcação confirmada: Maria Silva - Manicure completa, Massagem relaxante, masagem capilar.', N'Sistema', 3, CAST(N'2026-06-21T22:45:58.590' AS DateTime), 0, CAST(N'2026-06-21T22:45:58.590' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (46, N'A sua marcação foi confirmada: Manicure completa, Massagem relaxante, masagem capilar em 23/06/2026 às 12:00.', N'Sistema', 2, CAST(N'2026-06-21T22:45:58.590' AS DateTime), 0, CAST(N'2026-06-21T22:45:58.590' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (47, N'Marcação cancelada: Maria Silva - Manicure completa, Massagem relaxante, masagem capilar.', N'Sistema', 3, CAST(N'2026-06-21T22:46:02.200' AS DateTime), 0, CAST(N'2026-06-21T22:46:02.200' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (48, N'A sua marcação foi cancelada: Manicure completa, Massagem relaxante, masagem capilar em 23/06/2026 às 12:00.', N'Sistema', 2, CAST(N'2026-06-21T22:46:02.200' AS DateTime), 0, CAST(N'2026-06-21T22:46:02.200' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (49, N'Bem-vinda à BeautéCare! A sua conta foi criada com sucesso.', N'Sistema', 14, CAST(N'2026-06-21T23:36:19.510' AS DateTime), 0, CAST(N'2026-06-21T23:36:19.510' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (50, N'Marcação confirmada: Maria Silva - Manicure completa, Massagem relaxante, masagem capilar.', N'Sistema', 3, CAST(N'2026-06-21T23:43:29.347' AS DateTime), 0, CAST(N'2026-06-21T23:43:29.347' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (51, N'A sua marcação foi confirmada: Manicure completa, Massagem relaxante, masagem capilar em 23/06/2026 às 12:00.', N'Sistema', 2, CAST(N'2026-06-21T23:43:29.347' AS DateTime), 0, CAST(N'2026-06-21T23:43:29.347' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (52, N'me pague, caloteira', N'Sistema', 9, CAST(N'2026-06-22T09:44:05.047' AS DateTime), 0, CAST(N'2026-06-22T09:44:05.047' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (53, N'Bem-vinda ao seu painel de cliente. Veja as próximas marcações e promoções.', N'Sistema', 14, CAST(N'2026-06-22T09:52:25.280' AS DateTime), 0, CAST(N'2026-06-22T09:52:25.280' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (54, N'Marcação criada para 24/06/2026 às 11:00.', N'Sistema', 14, CAST(N'2026-06-22T09:58:09.840' AS DateTime), 1, CAST(N'2026-06-22T09:58:09.840' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (55, N'Nova marcação de Lais Macedo para 24/06/2026 às 11:00.', N'Sistema', 3, CAST(N'2026-06-22T09:58:09.840' AS DateTime), 0, CAST(N'2026-06-22T09:58:09.840' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (56, N'Tem uma marcação agendada. Confira o horário para não se atrasar.', N'Sistema', 14, CAST(N'2026-06-22T09:58:25.743' AS DateTime), 0, CAST(N'2026-06-22T09:58:25.743' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (57, N'Marcação criada para 23/06/2026 às 10:00.', N'Sistema', 14, CAST(N'2026-06-22T09:59:33.513' AS DateTime), 0, CAST(N'2026-06-22T09:59:33.513' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (58, N'Nova marcação de Lais Macedo para 23/06/2026 às 10:00.', N'Sistema', 3, CAST(N'2026-06-22T09:59:33.513' AS DateTime), 0, CAST(N'2026-06-22T09:59:33.513' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (59, N'Marcação criada para 24/06/2026 às 16:00.', N'Sistema', 14, CAST(N'2026-06-22T10:02:05.593' AS DateTime), 0, CAST(N'2026-06-22T10:02:05.593' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (60, N'Nova marcação de Lais Macedo para 24/06/2026 às 16:00.', N'Sistema', 3, CAST(N'2026-06-22T10:02:05.593' AS DateTime), 0, CAST(N'2026-06-22T10:02:05.593' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (61, N'Marcação confirmada: Lais Macedo - masagem capilar.', N'Sistema', 3, CAST(N'2026-06-22T10:03:57.310' AS DateTime), 0, CAST(N'2026-06-22T10:03:57.310' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (62, N'A sua marcação foi confirmada: masagem capilar em 23/06/2026 às 10:00.', N'Sistema', 14, CAST(N'2026-06-22T10:03:57.310' AS DateTime), 0, CAST(N'2026-06-22T10:03:57.310' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (63, N'Marcação cancelada: Lais Macedo - Pedicure, Manicure completa, Massagem relaxante.', N'Sistema', 3, CAST(N'2026-06-22T10:04:29.070' AS DateTime), 0, CAST(N'2026-06-22T10:04:29.070' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (64, N'A sua marcação foi cancelada: Pedicure, Manicure completa, Massagem relaxante em 24/06/2026 às 11:00.', N'Sistema', 14, CAST(N'2026-06-22T10:04:29.070' AS DateTime), 0, CAST(N'2026-06-22T10:04:29.070' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (65, N'Marcação confirmada: Lais Macedo - Limpeza de pele.', N'Sistema', 3, CAST(N'2026-06-22T10:04:35.107' AS DateTime), 0, CAST(N'2026-06-22T10:04:35.107' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (66, N'A sua marcação foi confirmada: Limpeza de pele em 24/06/2026 às 16:00.', N'Sistema', 14, CAST(N'2026-06-22T10:04:35.110' AS DateTime), 0, CAST(N'2026-06-22T10:04:35.110' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (67, N'Lais me pague ag', N'Sistema', 14, CAST(N'2026-06-22T10:06:25.237' AS DateTime), 0, CAST(N'2026-06-22T10:06:25.237' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (68, N'oioi', N'Sistema', 14, CAST(N'2026-06-22T10:07:04.870' AS DateTime), 1, CAST(N'2026-06-22T10:07:04.870' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (69, N'oi senhoira sou eu beauteccare', N'Sistema', 8, CAST(N'2026-06-22T10:08:31.107' AS DateTime), 0, CAST(N'2026-06-22T10:08:31.107' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (70, N'oi emanoeeel em espanha', N'Sistema', 12, CAST(N'2026-06-22T10:21:45.620' AS DateTime), 0, CAST(N'2026-06-22T10:21:45.620' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (71, N'Marcação criada para 27/06/2026 às 10:00.', N'Sistema', 14, CAST(N'2026-06-22T19:00:47.397' AS DateTime), 0, CAST(N'2026-06-22T19:00:47.397' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (72, N'Nova marcação de Lais Macedo para 27/06/2026 às 10:00.', N'Sistema', 3, CAST(N'2026-06-22T19:00:47.397' AS DateTime), 0, CAST(N'2026-06-22T19:00:47.397' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (73, N'Marcação confirmada: Lais Macedo - Micro Pigmentacao.', N'Sistema', 3, CAST(N'2026-06-22T19:03:19.430' AS DateTime), 0, CAST(N'2026-06-22T19:03:19.430' AS DateTime), NULL)
INSERT [dbo].[Notificacoes] ([Id], [Mensagem], [Tipo], [IdUsuario], [DataNotificacao], [Lida], [DataCriacao], [Titulo]) VALUES (74, N'A sua marcação foi confirmada: Micro Pigmentacao em 27/06/2026 às 10:00.', N'Sistema', 14, CAST(N'2026-06-22T19:03:19.430' AS DateTime), 0, CAST(N'2026-06-22T19:03:19.430' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Notificacoes] OFF
GO

-- [dbo].[Profissionais]
SET IDENTITY_INSERT [dbo].[Profissionais] ON 
INSERT [dbo].[Profissionais] ([IdProfissional], [IdUsuario], [Especialidade], [Avaliacao], [ComissaoPercentual], [DataCriacao]) VALUES (1, 3, N'Estética facial', CAST(4.80 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), CAST(N'2026-06-21T15:54:06.543' AS DateTime))
INSERT [dbo].[Profissionais] ([IdProfissional], [IdUsuario], [Especialidade], [Avaliacao], [ComissaoPercentual], [DataCriacao]) VALUES (2, 4, N'Unhas e maquilhagem', CAST(4.60 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), CAST(N'2026-06-21T15:54:06.543' AS DateTime))
SET IDENTITY_INSERT [dbo].[Profissionais] OFF
GO

-- [dbo].[ProfissionalServicos]
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (3, 1)
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (3, 3)
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (3, 4)
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (4, 1)
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (4, 2)
INSERT [dbo].[ProfissionalServicos] ([IdProfissional], [IdServico]) VALUES (4, 4)
GO

-- [dbo].[ServicoCategorias]
SET IDENTITY_INSERT [dbo].[ServicoCategorias] ON 
INSERT [dbo].[ServicoCategorias] ([IdCategoria], [Nome], [Descricao], [Foto], [Ativa], [Ordem]) VALUES (1, N'Rosto', N'Tratamentos faciais e sobrancelhas', NULL, 1, 1)
INSERT [dbo].[ServicoCategorias] ([IdCategoria], [Nome], [Descricao], [Foto], [Ativa], [Ordem]) VALUES (2, N'Unhas', N'Manicure, pedicure e cuidados de unhas', NULL, 1, 2)
INSERT [dbo].[ServicoCategorias] ([IdCategoria], [Nome], [Descricao], [Foto], [Ativa], [Ordem]) VALUES (3, N'Corpo', N'Massagens e tratamentos corporais', NULL, 1, 3)
INSERT [dbo].[ServicoCategorias] ([IdCategoria], [Nome], [Descricao], [Foto], [Ativa], [Ordem]) VALUES (4, N'Depilação', N'Serviços de depilação estética', NULL, 1, 4)
SET IDENTITY_INSERT [dbo].[ServicoCategorias] OFF
GO

-- [dbo].[Servicos]
SET IDENTITY_INSERT [dbo].[Servicos] ON 
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (1, N'Limpeza de pele', N'Facial', N'Tratamento profundo para limpeza, hidratação e renovação da pele.', 60, CAST(35.00 AS Decimal(10, 2)), NULL, 1, 1, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (2, N'Manicure completa', N'Unhas', N'Corte, cuidado de cutículas, hidratação e verniz.', 45, CAST(20.00 AS Decimal(10, 2)), NULL, 1, 2, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (3, N'Massagem relaxante', N'Corpo', N'Massagem corporal para relaxamento e alívio de tensão.', 60, CAST(45.00 AS Decimal(10, 2)), NULL, 1, 1, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (4, N'Design de sobrancelhas', N'Facial', N'Modelagem das sobrancelhas de acordo com o rosto.', 30, CAST(15.00 AS Decimal(10, 2)), NULL, 0, 0, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (5, N'Pedicure', N'Unhas', N'Tratamento completo dos pés', 45, CAST(22.00 AS Decimal(10, 2)), NULL, 1, 0, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (6, N'Maquiagem', N'Rosto', N'Fazemos todos os tipos de maquiagem.', 60, CAST(100.00 AS Decimal(10, 2)), N'C:\Users\laurita\Downloads\ProjetoFinal_Admin_Ajustes_Visuais_Finais\ProjetoFinal\bin\Debug\FotosServicos\servico_20260621160314.jpg', 0, 0, CAST(5.00 AS Decimal(3, 2)), 1)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (7, N'masagem capilar', N'Cabelo', N'Massagem e laser para o cabelo crescer mais rapido.', 60, CAST(150.00 AS Decimal(10, 2)), NULL, 1, 1, CAST(5.00 AS Decimal(3, 2)), 0)
INSERT [dbo].[Servicos] ([IdServico], [Nome], [Categoria], [Descricao], [DuracaoMinutos], [Preco], [Foto], [Ativo], [Popularidade], [Avaliacao], [Excluido]) VALUES (8, N'Micro Pigmentacao', N'Facial', N'', 60, CAST(100.00 AS Decimal(10, 2)), NULL, 1, 0, CAST(5.00 AS Decimal(3, 2)), 0)
SET IDENTITY_INSERT [dbo].[Servicos] OFF
GO

-- [dbo].[Usuarios]
SET IDENTITY_INSERT [dbo].[Usuarios] ON 
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (1, N'Laura Macedo', N'admin@beautecare.pt', N'913885275', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', N'Admin', 1, CAST(N'2026-06-16T15:26:26.597' AS DateTime), CAST(N'2026-06-22T19:04:28.200' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (2, N'Maria Silva', N'cliente@beautecare.pt', N'912345678', N'09a31a7001e261ab1e056182a71d3cf57f582ca9a29cff5eb83be0f0549730a9', N'Cliente', 1, CAST(N'2026-04-27T15:26:26.603' AS DateTime), CAST(N'2026-06-21T23:46:04.613' AS DateTime), N'C:\Users\laurita\Downloads\ProjetoFinal_Cliente_Corrigido\ProjetoFinal\bin\Debug\FotosUtilizadores\cliente_2.jpg', NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 146)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (3, N'Ana Costa', N'ana@beautecare.pt', N'914567890', N'00624b02e1f9b996a3278f559d5d55313552ad2c0bafc82adfd975c12df61eaf', N'Profissional', 1, CAST(N'2026-02-16T15:26:26.603' AS DateTime), CAST(N'2026-06-22T19:01:42.273' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (4, N'Carla Mendes', N'carla@beautecare.pt', N'913456789', N'09a31a7001e261ab1e056182a71d3cf57f582ca9a29cff5eb83be0f0549730a9', N'Cliente', 1, CAST(N'2026-05-27T15:26:26.603' AS DateTime), CAST(N'2026-05-07T15:26:26.603' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (5, N'Juliana Santos', N'juliana@beautecare.pt', N'915555111', N'00624b02e1f9b996a3278f559d5d55313552ad2c0bafc82adfd975c12df61eaf', N'Profissional', 1, CAST(N'2026-03-18T15:26:26.607' AS DateTime), CAST(N'2026-06-16T15:26:26.607' AS DateTime), NULL, N'Unhas e estética facial', CAST(4.80 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (6, N'Patrícia Gomes', N'patricia@beautecare.pt', N'916222333', N'09a31a7001e261ab1e056182a71d3cf57f582ca9a29cff5eb83be0f0549730a9', N'Cliente', 1, CAST(N'2025-12-22T21:57:16.863' AS DateTime), CAST(N'2026-04-11T21:57:16.863' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (7, N'Inês Rocha', N'ines@beautecare.pt', N'917333444', N'00624b02e1f9b996a3278f559d5d55313552ad2c0bafc82adfd975c12df61eaf', N'Profissional', 1, CAST(N'2026-02-20T21:57:16.863' AS DateTime), CAST(N'2026-06-20T21:57:16.863' AS DateTime), NULL, N'Massagem e corpo', CAST(4.90 AS Decimal(3, 2)), CAST(42.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (8, N'Urania Macedo', N'macedourania@gmail.com', N'912098317', N'229bf1eb8196b8a9e1f0fc9eeea71670afe41486a3cb369880465a6f6d86d3fb', N'Cliente', 1, CAST(N'2026-06-21T11:55:53.667' AS DateTime), NULL, NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (9, N'Lais Macedo', N'macedoolais@gmail.com', N'960266357', N'dfdae70e3feb1cc8f089496e257b81aedbfb12f9b2694f302e3559701ca731f6', N'Profissional', 1, CAST(N'2026-06-21T12:00:47.387' AS DateTime), NULL, NULL, N'Manicure', CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (10, N'Kaliny Pessoa', N'kalinysilveira5@gmail.com', N'913908319', N'6926e574d72b146b469c2257e9106c86948495bab823a1f9f807cb1ff568d0be', N'Cliente', 1, CAST(N'2026-06-21T15:59:55.667' AS DateTime), CAST(N'2026-06-21T00:00:00.000' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (11, N'Uedja silva', N'uedja@gmail.com', N'912098765', N'e92e3a9c0eeeffb6ee722f234510cbda30f1bc6db3e6e3cb41f31dfb3bc96012', N'Profissional', 1, CAST(N'2026-06-21T16:10:25.997' AS DateTime), NULL, NULL, N'maquiagem', CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (12, N'Emanoel Macedo', N'emanoeldocavaco@hotmail.com', N'915719366', N'ba89f8de31f5e7b969899d514f06fb5dcdc6cede274a585e899b5e5ada855dab', N'Cliente', 1, CAST(N'2026-06-21T22:05:29.820' AS DateTime), NULL, NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (13, N'Maria Jullya', N'mariajulia@gmail.com', N'123456789', N'055cbbfc7f69a61ab5ba39b5f69ca2490ac714f709e163567c64d6cffd941523', N'Cliente', 1, CAST(N'2026-06-21T22:42:32.780' AS DateTime), CAST(N'2026-06-21T22:42:45.480' AS DateTime), NULL, NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 0)
INSERT [dbo].[Usuarios] ([IdUsuario], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Ativo], [DataCriacao], [UltimaVisita], [Foto], [Especialidade], [Avaliacao], [ComissaoPercentual], [PontosCliente]) VALUES (14, N'Lais Macedo', N'macedolais@icloud.com', N'987654322', N'1e27fb4d7fbc0fc76ee2daf62deafbe52dcc57b6f57db01d4c20252919bf7888', N'Cliente', 1, CAST(N'2026-06-21T23:36:19.507' AS DateTime), CAST(N'2026-06-22T18:56:15.603' AS DateTime), N'C:\Users\laurita\Downloads\ProjetoFinal_corrigido_FaturasClientes\ProjetoFinal\bin\Debug\FotosUtilizadores\cliente_14.jpg', NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)), 118)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO

-- [dbo].[Usuarios_Backup_20260616152626]
SET IDENTITY_INSERT [dbo].[Usuarios_Backup_20260616152626] ON 
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (1, N'Laura Macedo', N'admin@beautecare.pt', N'910000001', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Admin', NULL, 1, CAST(N'2026-06-13T20:16:02.550' AS DateTime), CAST(N'2026-06-13T20:16:02.550' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (2, N'Maria Silva', N'cliente@beautecare.pt', N'910000002', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Cliente', NULL, 1, CAST(N'2026-04-13T20:16:02.550' AS DateTime), CAST(N'2026-06-03T20:16:02.550' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (3, N'Ana Costa', N'prof@beautecare.pt', N'910000003', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Profissional', NULL, 1, CAST(N'2026-01-13T20:16:02.550' AS DateTime), CAST(N'2026-06-13T20:16:02.550' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (4, N'Beatriz Santos', N'beatriz@beautecare.pt', N'910000004', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Profissional', NULL, 1, CAST(N'2025-12-13T20:16:02.550' AS DateTime), CAST(N'2026-06-13T20:16:02.550' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (5, N'Carla Mendes', N'carla@cliente.pt', N'910000005', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Cliente', NULL, 1, CAST(N'2025-09-13T20:16:02.550' AS DateTime), CAST(N'2026-04-29T20:16:02.550' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (6, N'Ana Costa', N'ana@beautecare.pt', N'914567890', N'00624b02e1f9b996a3278f559d5d55313552ad2c0bafc82adfd975c12df61eaf', N'Profissional', NULL, 1, CAST(N'2026-02-14T14:14:09.240' AS DateTime), CAST(N'2026-06-14T14:14:09.240' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (7, N'Carla Mendes', N'carla@beautecare.pt', N'913456789', N'09a31a7001e261ab1e056182a71d3cf57f582ca9a29cff5eb83be0f0549730a9', N'Cliente', NULL, 1, CAST(N'2026-05-25T14:14:09.243' AS DateTime), CAST(N'2026-05-05T14:14:09.243' AS DateTime), NULL, CAST(5.00 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
INSERT [dbo].[Usuarios_Backup_20260616152626] ([Id], [Nome], [Email], [Telefone], [Senha], [TipoUsuario], [Foto], [Ativo], [DataCriacao], [UltimaVisita], [Especialidade], [Avaliacao], [ComissaoPercentual]) VALUES (8, N'Juliana Santos', N'juliana@beautecare.pt', N'915555111', N'00624b02e1f9b996a3278f559d5d55313552ad2c0bafc82adfd975c12df61eaf', N'Profissional', NULL, 1, CAST(N'2026-03-16T14:14:09.243' AS DateTime), CAST(N'2026-06-14T14:14:09.243' AS DateTime), N'Unhas e estética facial', CAST(4.80 AS Decimal(3, 2)), CAST(40.00 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[Usuarios_Backup_20260616152626] OFF
GO

-- [UQ__Avaliaco__0FFD43308A974788]
/****** Object:  Index [UQ__Avaliaco__0FFD43308A974788]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Avaliacoes] ADD UNIQUE NONCLUSTERED 
(
	[IdMarcacao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Clientes__5B65BF96C822DC86]
/****** Object:  Index [UQ__Clientes__5B65BF96C822DC86]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Clientes] ADD UNIQUE NONCLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Cupoes__06370DACC9AC6D77]
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Cupoes__06370DACC9AC6D77]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Cupoes] ADD UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Faturas__FD26B4CE79A4B778]
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Faturas__FD26B4CE79A4B778]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Faturas] ADD UNIQUE NONCLUSTERED 
(
	[NumeroFatura] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [IX_Marcacoes_Conflito]
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Marcacoes_Conflito]    Script Date: 6/22/2026 7:48:50 PM ******/
CREATE NONCLUSTERED INDEX [IX_Marcacoes_Conflito] ON [dbo].[Marcacoes]
(
	[IdProfissional] ASC,
	[DataMarcacao] ASC,
	[Hora] ASC,
	[Estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UX_Marcacao_Espaco_Hora]
/****** Object:  Index [UX_Marcacao_Espaco_Hora]    Script Date: 6/22/2026 7:48:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Marcacao_Espaco_Hora] ON [dbo].[Marcacoes]
(
	[IdEspaco] ASC,
	[DataMarcacao] ASC,
	[Hora] ASC
)
WHERE ([Estado]<>'Cancelada')
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UX_Marcacao_Profissional_Hora]
/****** Object:  Index [UX_Marcacao_Profissional_Hora]    Script Date: 6/22/2026 7:48:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Marcacao_Profissional_Hora] ON [dbo].[Marcacoes]
(
	[IdProfissional] ASC,
	[DataMarcacao] ASC,
	[Hora] ASC
)
WHERE ([Estado]<>'Cancelada')
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Profissi__5B65BF961F9F4726]
/****** Object:  Index [UQ__Profissi__5B65BF961F9F4726]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Profissionais] ADD UNIQUE NONCLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Usuarios__A9D10534570FD4B8]
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Usuarios__A9D10534570FD4B8]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Usuarios] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UX_Usuarios_IdUsuario_HOTFIX]
/****** Object:  Index [UX_Usuarios_IdUsuario_HOTFIX]    Script Date: 6/22/2026 7:48:50 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Usuarios_IdUsuario_HOTFIX] ON [dbo].[Usuarios]
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [UQ__Usuarios__A9D105348273E80F]
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Usuarios__A9D105348273E80F]    Script Date: 6/22/2026 7:48:50 PM ******/
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- [DF__Avaliacoe__DataA__05D8E0BE]
ALTER TABLE [dbo].[Avaliacoes] ADD  DEFAULT (getdate()) FOR [DataAvaliacao]
GO

-- [DF_Clientes_PontosCliente_HOTFIX]
ALTER TABLE [dbo].[Clientes] ADD  CONSTRAINT [DF_Clientes_PontosCliente_HOTFIX]  DEFAULT ((0)) FOR [PontosCliente]
GO

-- [DF__Cupoes__Tipo__3F115E1A]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT ('Promocao') FOR [Tipo]
GO

-- [DF__Cupoes__Percentu__40058253]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT ((0)) FOR [PercentualDesconto]
GO

-- [DF__Cupoes__ValorDes__40F9A68C]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT ((0)) FOR [ValorDesconto]
GO

-- [DF__Cupoes__DataCria__41EDCAC5]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Cupoes__DataVali__42E1EEFE]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT (dateadd(day,(30),getdate())) FOR [DataValidade]
GO

-- [DF__Cupoes__Usado__43D61337]
ALTER TABLE [dbo].[Cupoes] ADD  DEFAULT ((0)) FOR [Usado]
GO

-- [DF__Espacos__Ativo__619B8048]
ALTER TABLE [dbo].[Espacos] ADD  DEFAULT ((1)) FOR [Ativo]
GO

-- [DF__Faturas__DataFat__6FE99F9F]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT (getdate()) FOR [DataFatura]
GO

-- [DF_Faturas_ValorTotal_Final_2]
ALTER TABLE [dbo].[Faturas] ADD  CONSTRAINT [DF_Faturas_ValorTotal_Final_2]  DEFAULT ((0)) FOR [ValorTotal]
GO

-- [DF__Faturas__Estado__70DDC3D8]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ('Pendente') FOR [Estado]
GO

-- [DF__Faturas__Cliente__1DB06A4F]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ('') FOR [Cliente]
GO

-- [DF__Faturas__Servico__1EA48E88]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ('') FOR [Servicos]
GO

-- [DF__Faturas__Subtota__1F98B2C1]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ((0)) FOR [Subtotal]
GO

-- [DF__Faturas__Descont__208CD6FA]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ((0)) FOR [Desconto]
GO

-- [DF__Faturas__Total__2180FB33]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ((0)) FOR [Total]
GO

-- [DF__Faturas__MetodoP__22751F6C]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ('Multibanco') FOR [MetodoPagamento]
GO

-- [DF__Faturas__Comissa__29221CFB]
ALTER TABLE [dbo].[Faturas] ADD  DEFAULT ((40)) FOR [ComissaoPercentual]
GO

-- [DF_Faturas_DataCriacao_HOTFIX]
ALTER TABLE [dbo].[Faturas] ADD  CONSTRAINT [DF_Faturas_DataCriacao_HOTFIX]  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Importaco__Linha__531856C7]
ALTER TABLE [dbo].[Importacoes] ADD  DEFAULT ((0)) FOR [LinhasImportadas]
GO

-- [DF__Importaco__DataI__540C7B00]
ALTER TABLE [dbo].[Importacoes] ADD  DEFAULT (getdate()) FOR [DataImportacao]
GO

-- [DF__MarcacaoS__Durac__2BFE89A6]
ALTER TABLE [dbo].[MarcacaoServicos] ADD  DEFAULT ((60)) FOR [DuracaoMinutos]
GO

-- [DF__MarcacaoS__Preco__2CF2ADDF]
ALTER TABLE [dbo].[MarcacaoServicos] ADD  DEFAULT ((0)) FOR [Preco]
GO

-- [DF_Marcacoes_IdEspaco_Final2]
ALTER TABLE [dbo].[Marcacoes] ADD  CONSTRAINT [DF_Marcacoes_IdEspaco_Final2]  DEFAULT ((1)) FOR [IdEspaco]
GO

-- [DF_Marcacoes_Espaco_Final2]
ALTER TABLE [dbo].[Marcacoes] ADD  CONSTRAINT [DF_Marcacoes_Espaco_Final2]  DEFAULT (N'Sala 1') FOR [Espaco]
GO

-- [DF__Marcacoes__Estad__68487DD7]
ALTER TABLE [dbo].[Marcacoes] ADD  DEFAULT ('Pendente') FOR [Estado]
GO

-- [DF__Marcacoes__Criad__6A30C649]
ALTER TABLE [dbo].[Marcacoes] ADD  DEFAULT (getdate()) FOR [CriadoEm]
GO

-- [DF__Marcacoes__DataC__1CBC4616]
ALTER TABLE [dbo].[Marcacoes] ADD  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Marcacoes__Durac__282DF8C2]
ALTER TABLE [dbo].[Marcacoes] ADD  DEFAULT ((60)) FOR [DuracaoMinutos]
GO

-- [DF__Marcacoes__Avali__3B40CD36]
ALTER TABLE [dbo].[Marcacoes] ADD  DEFAULT ((0)) FOR [Avaliada]
GO

-- [DF_Mensagens_DataCriacao_HOTFIX]
ALTER TABLE [dbo].[Mensagens] ADD  CONSTRAINT [DF_Mensagens_DataCriacao_HOTFIX]  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Mensagens__Canal__4F47C5E3]
ALTER TABLE [dbo].[MensagensEnviadas] ADD  DEFAULT ('App') FOR [Canal]
GO

-- [DF__Mensagens__DataE__503BEA1C]
ALTER TABLE [dbo].[MensagensEnviadas] ADD  DEFAULT (getdate()) FOR [DataEnvio]
GO

-- [DF__Notificaco__Tipo__7A672E12]
ALTER TABLE [dbo].[Notificacoes] ADD  DEFAULT ('Sistema') FOR [Tipo]
GO

-- [DF__Notificac__DataN__7C4F7684]
ALTER TABLE [dbo].[Notificacoes] ADD  DEFAULT (getdate()) FOR [DataNotificacao]
GO

-- [DF__Notificaco__Lida__7D439ABD]
ALTER TABLE [dbo].[Notificacoes] ADD  DEFAULT ((0)) FOR [Lida]
GO

-- [DF_Notificacoes_DataCriacao_FIX]
ALTER TABLE [dbo].[Notificacoes] ADD  CONSTRAINT [DF_Notificacoes_DataCriacao_FIX]  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Pagamento__DataP__75A278F5]
ALTER TABLE [dbo].[Pagamentos] ADD  DEFAULT (getdate()) FOR [DataPagamento]
GO

-- [DF__Pagamento__Estad__76969D2E]
ALTER TABLE [dbo].[Pagamentos] ADD  DEFAULT ('Concluido') FOR [Estado]
GO

-- [DF__Profissio__Avali__5629CD9C]
ALTER TABLE [dbo].[Profissionais] ADD  DEFAULT ((0)) FOR [Avaliacao]
GO

-- [DF_Profissionais_Comissao_FIX]
ALTER TABLE [dbo].[Profissionais] ADD  CONSTRAINT [DF_Profissionais_Comissao_FIX]  DEFAULT ((40)) FOR [ComissaoPercentual]
GO

-- [DF_Profissionais_DataCriacao_HOTFIX]
ALTER TABLE [dbo].[Profissionais] ADD  CONSTRAINT [DF_Profissionais_DataCriacao_HOTFIX]  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__ServicoCa__Ativa__56E8E7AB]
ALTER TABLE [dbo].[ServicoCategorias] ADD  DEFAULT ((1)) FOR [Ativa]
GO

-- [DF__ServicoCa__Ordem__57DD0BE4]
ALTER TABLE [dbo].[ServicoCategorias] ADD  DEFAULT ((0)) FOR [Ordem]
GO

-- [DF__Servicos__Duraca__59063A47]
ALTER TABLE [dbo].[Servicos] ADD  DEFAULT ((60)) FOR [DuracaoMinutos]
GO

-- [DF__Servicos__Preco__59FA5E80]
ALTER TABLE [dbo].[Servicos] ADD  DEFAULT ((0)) FOR [Preco]
GO

-- [DF__Servicos__Ativo__5AEE82B9]
ALTER TABLE [dbo].[Servicos] ADD  DEFAULT ((1)) FOR [Ativo]
GO

-- [DF__Servicos__Popula__1BC821DD]
ALTER TABLE [dbo].[Servicos] ADD  DEFAULT ((0)) FOR [Popularidade]
GO

-- [DF__Servicos__Avalia__2739D489]
ALTER TABLE [dbo].[Servicos] ADD  DEFAULT ((5)) FOR [Avaliacao]
GO

-- [DF_Servicos_Excluido_Final]
ALTER TABLE [dbo].[Servicos] ADD  CONSTRAINT [DF_Servicos_Excluido_Final]  DEFAULT ((0)) FOR [Excluido]
GO

-- [DF__Usuarios__TipoUs__17036CC0]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ('Cliente') FOR [TipoUsuario]
GO

-- [DF__Usuarios__Ativo__17F790F9]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Ativo]
GO

-- [DF__Usuarios__DataCr__18EBB532]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Usuarios__Avalia__19DFD96B]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((5)) FOR [Avaliacao]
GO

-- [DF__Usuarios__Comiss__1AD3FDA4]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((40)) FOR [ComissaoPercentual]
GO

-- [DF__Usuarios__Pontos__3A4CA8FD]
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((0)) FOR [PontosCliente]
GO

-- [DF__Usuarios__Ativo__4CA06362]
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626] ADD  DEFAULT ((1)) FOR [Ativo]
GO

-- [DF__Usuarios__DataCr__4D94879B]
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626] ADD  DEFAULT (getdate()) FOR [DataCriacao]
GO

-- [DF__Usuarios__Avalia__06CD04F7]
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626] ADD  DEFAULT ((5)) FOR [Avaliacao]
GO

-- [DF__Usuarios__Comiss__07C12930]
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626] ADD  DEFAULT ((40)) FOR [ComissaoPercentual]
GO

-- [FK__Avaliacoe__IdMar__01142BA1]
ALTER TABLE [dbo].[Avaliacoes]  WITH CHECK ADD FOREIGN KEY([IdMarcacao])
REFERENCES [dbo].[Marcacoes] ([IdMarcacao])
GO

-- [FK__Avaliacoe__IdSer__03F0984C]
ALTER TABLE [dbo].[Avaliacoes]  WITH CHECK ADD FOREIGN KEY([IdServico])
REFERENCES [dbo].[Servicos] ([IdServico])
GO

-- [FK_Clientes_Usuarios_FIX]
ALTER TABLE [dbo].[Clientes]  WITH NOCHECK ADD  CONSTRAINT [FK_Clientes_Usuarios_FIX] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Clientes_Usuarios_FIX]
GO

-- [FK__Faturas__IdMarca__6E01572D]
ALTER TABLE [dbo].[Faturas]  WITH CHECK ADD FOREIGN KEY([IdMarcacao])
REFERENCES [dbo].[Marcacoes] ([IdMarcacao])
GO

-- [FK_Faturas_ClienteUsuario_FIX]
ALTER TABLE [dbo].[Faturas]  WITH NOCHECK ADD  CONSTRAINT [FK_Faturas_ClienteUsuario_FIX] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Faturas] CHECK CONSTRAINT [FK_Faturas_ClienteUsuario_FIX]
GO

-- [FK_Faturas_Pagamentos]
ALTER TABLE [dbo].[Faturas]  WITH CHECK ADD  CONSTRAINT [FK_Faturas_Pagamentos] FOREIGN KEY([IdPagamento])
REFERENCES [dbo].[Pagamentos] ([IdPagamento])
GO
ALTER TABLE [dbo].[Faturas] CHECK CONSTRAINT [FK_Faturas_Pagamentos]
GO

-- [FK__Marcacoes__IdEsp__6754599E]
ALTER TABLE [dbo].[Marcacoes]  WITH CHECK ADD FOREIGN KEY([IdEspaco])
REFERENCES [dbo].[Espacos] ([IdEspaco])
GO

-- [FK__Marcacoes__IdSer__656C112C]
ALTER TABLE [dbo].[Marcacoes]  WITH CHECK ADD FOREIGN KEY([IdServico])
REFERENCES [dbo].[Servicos] ([IdServico])
GO

-- [FK_Marcacoes_ClienteUsuario_FIX]
ALTER TABLE [dbo].[Marcacoes]  WITH NOCHECK ADD  CONSTRAINT [FK_Marcacoes_ClienteUsuario_FIX] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Marcacoes] CHECK CONSTRAINT [FK_Marcacoes_ClienteUsuario_FIX]
GO

-- [FK_Marcacoes_ProfissionalUsuario_FIX]
ALTER TABLE [dbo].[Marcacoes]  WITH NOCHECK ADD  CONSTRAINT [FK_Marcacoes_ProfissionalUsuario_FIX] FOREIGN KEY([IdProfissional])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Marcacoes] CHECK CONSTRAINT [FK_Marcacoes_ProfissionalUsuario_FIX]
GO

-- [FK_Notificacoes_Usuarios_FIX]
ALTER TABLE [dbo].[Notificacoes]  WITH NOCHECK ADD  CONSTRAINT [FK_Notificacoes_Usuarios_FIX] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Notificacoes] CHECK CONSTRAINT [FK_Notificacoes_Usuarios_FIX]
GO

-- [FK__Pagamento__IdFat__74AE54BC]
ALTER TABLE [dbo].[Pagamentos]  WITH CHECK ADD FOREIGN KEY([IdFatura])
REFERENCES [dbo].[Faturas] ([IdFatura])
GO

-- [FK_Profissionais_Usuarios_FIX]
ALTER TABLE [dbo].[Profissionais]  WITH NOCHECK ADD  CONSTRAINT [FK_Profissionais_Usuarios_FIX] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[Profissionais] CHECK CONSTRAINT [FK_Profissionais_Usuarios_FIX]
GO

-- [FK__Profissio__IdSer__5EBF139D]
ALTER TABLE [dbo].[ProfissionalServicos]  WITH CHECK ADD FOREIGN KEY([IdServico])
REFERENCES [dbo].[Servicos] ([IdServico])
GO

-- [FK_ProfissionalServicos_Usuario_FIX]
ALTER TABLE [dbo].[ProfissionalServicos]  WITH NOCHECK ADD  CONSTRAINT [FK_ProfissionalServicos_Usuario_FIX] FOREIGN KEY([IdProfissional])
REFERENCES [dbo].[Usuarios] ([IdUsuario])
GO
ALTER TABLE [dbo].[ProfissionalServicos] CHECK CONSTRAINT [FK_ProfissionalServicos_Usuario_FIX]
GO

-- [CK__Avaliacoe__Class__04E4BC85]
ALTER TABLE [dbo].[Avaliacoes]  WITH CHECK ADD CHECK  (([Classificacao]>=(0) AND [Classificacao]<=(5)))
GO

-- [CK_Faturas_Estado_BeauteCare_HF]
ALTER TABLE [dbo].[Faturas]  WITH CHECK ADD  CONSTRAINT [CK_Faturas_Estado_BeauteCare_HF] CHECK  (([Estado]=N'Liquidada' OR [Estado]=N'Nao paga' OR [Estado]=N'Não paga' OR [Estado]=N'Cancelado' OR [Estado]=N'Cancelada' OR [Estado]=N'Pago' OR [Estado]=N'Paga' OR [Estado]=N'Pendente'))
GO
ALTER TABLE [dbo].[Faturas] CHECK CONSTRAINT [CK_Faturas_Estado_BeauteCare_HF]
GO

-- [CK_Marcacoes_Estado_BeauteCare_Final]
ALTER TABLE [dbo].[Marcacoes]  WITH NOCHECK ADD  CONSTRAINT [CK_Marcacoes_Estado_BeauteCare_Final] CHECK  (([Estado] IS NULL OR ([Estado]=N'Concluído' OR [Estado]=N'Concluido' OR [Estado]=N'Concluída' OR [Estado]=N'Concluida' OR [Estado]=N'Cancelado' OR [Estado]=N'Cancelada' OR [Estado]=N'Confirmado' OR [Estado]=N'Confirmada' OR [Estado]=N'Pendente')))
GO
ALTER TABLE [dbo].[Marcacoes] CHECK CONSTRAINT [CK_Marcacoes_Estado_BeauteCare_Final]
GO

-- [CK__Usuarios__TipoUs__4BAC3F29]
ALTER TABLE [dbo].[Usuarios_Backup_20260616152626]  WITH CHECK ADD CHECK  (([TipoUsuario]='Profissional' OR [TipoUsuario]='Cliente' OR [TipoUsuario]='Admin'))
GO

-- Base de dados BeauteCareDB criada e preenchida.

