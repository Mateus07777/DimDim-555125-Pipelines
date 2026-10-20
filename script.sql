-- DDL equivalente ao modelo EF Core. Executar no banco dimdimdb, nunca em master.
-- Pode ser executado antes do primeiro deploy; o app cria o mesmo modelo em banco vazio.
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.Clientes (
  Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
  Nome nvarchar(100) NOT NULL,
  Email nvarchar(150) NOT NULL
 );
 CREATE UNIQUE INDEX IX_Clientes_Email ON dbo.Clientes(Email);
END;
IF OBJECT_ID(N'dbo.Contas', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.Contas (
  Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Contas PRIMARY KEY,
  Numero nvarchar(20) NOT NULL,
  Saldo decimal(18,2) NOT NULL,
  ClienteId int NOT NULL,
  CONSTRAINT FK_Contas_Clientes_ClienteId FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(Id)
 );
 CREATE UNIQUE INDEX IX_Contas_Numero ON dbo.Contas(Numero);
 CREATE INDEX IX_Contas_ClienteId ON dbo.Contas(ClienteId);
END;
