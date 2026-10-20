# DimDim Clientes e Contas - Pipelines CI/CD

Mateus Teni Pierro - RM555125. Grupo sugerido: **DimDim Flow**.
Projeto original .NET para o checkpoint; nao utiliza projeto de Sprint.

API ASP.NET Core 8 com Entity Framework Core, Azure SQL, Swagger, xUnit e Azure DevOps.
Um cliente possui varias contas. O relacionamento e uma FK real, com exclusao restrita.
A solucao usa App Service, sem Docker. Por isso nao ha Dockerfile/docker-compose.

## Execucao local no Windows

Instale o SDK .NET 8 se `dotnet --list-sdks` nao listar uma versao 8.0.xxx:
https://dotnet.microsoft.com/download/dotnet/8.0
No Visual Studio 2022, use versao 17.8 ou posterior e a carga ASP.NET e desenvolvimento Web.

Extraia a pasta para Downloads. Abra **PowerShell dentro da pasta que contem esta solucao**:

```powershell
dotnet --list-sdks
dotnet restore
dotnet test --configuration Release
dotnet run --project .\src\DimDim.Api
```

Abra http://localhost:5080/swagger e http://localhost:5080.
O launch profile configura SQLite local automaticamente. A nuvem usa Azure SQL,
com string de conexao recebida pelo ambiente; nao existem senhas no codigo.
Pressione Ctrl+C para parar. A API cria o schema no primeiro inicio de banco vazio.
`script.sql` permite criar explicitamente o schema antes do primeiro deploy.
Nao use EnsureCreated para evoluir tabelas de um sistema existente: este checkpoint
parte de um banco novo e nao inclui migrations.

## Caminho completo ate a entrega

Siga [o passo a passo](docs/01-PASSO-A-PASSO.md) na ordem:

1. Validar projeto local e testes.
2. Criar repositorio de origem no GitHub.
3. Criar projeto Azure DevOps com Git/Agile e importar o repositorio.
4. Criar Task e branch relacionada; configurar politicas da main.
5. Executar script para infraestrutura Azure.
6. Configurar Build CI e Release CD, testar os dois manualmente.
7. Gravar a alteracao de codigo, PR, aprovacao e execucoes automaticas.
8. Demonstrar CRUD nas duas tabelas com SELECT no Azure SQL.
9. Dar acesso ao professor e produzir o PDF com links reais.

## Arquivos

|Caminho|Finalidade|
|---|---|
|src/DimDim.Api|API completa e pagina inicial|
|tests/DimDim.Tests|Testes de regra, CRUD e validacoes com banco SQLite isolado|
|script.sql|DDL das duas tabelas e FK, SQL Server/Azure SQL|
|database/evidencias.sql|SELECTs para conferencia de cada operacao|
|infra/01-criar-recursos.ps1|Azure CLI: RG, Azure SQL, App Service e configuracao segura|
|azure-pipeline.yml|CI com build, testes publicados e pacote ZIP para Release|
|requests|JSONs e colecao Postman para todos os CRUDs|
|docs|Configuracao, roteiro e checklist do PDF|

## Regra que sera alterada no video

`RendimentoService.TaxaMensal` comeca em 0.01m. Na feature sera 0.02m,
com testes atualizados. Em uma conta de saldo 1000, o endpoint
`GET /api/contas/{id}/rendimento` passa de 10 para 20 depois do deploy.
E uma simulacao academica, sem significado de previsao financeira.

## Verificacao e limites

O pacote inclui testes executaveis; o resultado deve ser confirmado pelo comando
`dotnet test` e, depois, pela aba Tests da pipeline na sua organizacao.
Consulte `docs/VALIDACAO.md` para a verificacao realizada durante a preparacao.
O SDK e os agentes nao devem ser confundidos com evidencia de execucao no Azure.
Ainda sera necessario cadastrar os acessos, executar as pipelines, gravar video
e inserir os links reais no PDF final. Creditos Azure e permissoes da assinatura
determinam a disponibilidade da infraestrutura. Azure SQL Basic tem custo;
o script usa plano App Service F1 por padrao e oferece B1 como opcao paga.

## Referencias oficiais

- Branch policies: https://learn.microsoft.com/azure/devops/repos/git/branch-policies?view=azure-devops
- Artefatos da Release: https://learn.microsoft.com/azure/devops/pipelines/release/artifacts?view=azure-devops
- Gatilhos da Release: https://learn.microsoft.com/azure/devops/pipelines/release/triggers?view=azure-devops
- Agentes e paralelismo: https://learn.microsoft.com/azure/devops/pipelines/licensing/concurrent-jobs?view=azure-devops
- Azure CLI Web App: https://learn.microsoft.com/cli/azure/webapp?view=azure-cli-latest
- Azure CLI SQL DB: https://learn.microsoft.com/cli/azure/sql/db?view=azure-cli-latest
