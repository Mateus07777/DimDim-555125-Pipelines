-- Execute estes SELECTs apos CADA operacao na API em nuvem.
-- Nao execute DELETE nem UPDATE aqui: a alteracao deve vir da API.
SELECT Id, Nome, Email FROM dbo.Clientes ORDER BY Id;
SELECT Id, Numero, Saldo, ClienteId FROM dbo.Contas ORDER BY Id;
SELECT c.Id AS ClienteId, c.Nome, ct.Id AS ContaId, ct.Numero, ct.Saldo
FROM dbo.Clientes c LEFT JOIN dbo.Contas ct ON ct.ClienteId = c.Id
ORDER BY c.Id, ct.Id;
-- Comprovar FK:
SELECT name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Contas');
