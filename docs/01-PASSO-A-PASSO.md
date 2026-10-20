# Guia do zero ate a entrega

Leia uma etapa por vez. Todos os comandos locais abaixo sao para **PowerShell do Windows**.
Os nomes da interface podem aparecer em ingles ou portugues.
A entrega exige executar tudo na sua conta: os arquivos por si so nao comprovam o Azure.

## 1. Preparar e validar localmente

Extraia o ZIP. Abra a pasta `DimDimPipelines`, com o arquivo `DimDimPipelines.sln`.
Clique na barra de endereco do Explorador > digite `powershell` > Enter.
O terminal ja deve abrir na pasta correta; confirme com `Get-Location` e `Get-ChildItem`.

```powershell
dotnet --list-sdks
dotnet restore
dotnet test --configuration Release
dotnet run --project .\src\DimDim.Api
```

Esperado: restore e testes aprovados; depois `Now listening on: http://localhost:5080`.
Abra http://localhost:5080/swagger. Se faltar SDK 8, instale pelo link do README e reabra o terminal.
No Visual Studio: abra a solucao, defina DimDim.Api como projeto de inicializacao e F5.
O teste local usa SQLite com FK; a entrega usa SQL Server/Azure SQL, que tambem
precisa ser validado na nuvem. Pare a aplicacao com Ctrl+C antes de seguir.

## 2. Criar um repositorio de origem para a migracao

No GitHub, crie um repositorio vazio chamado `DimDim-555125-Pipelines`, sem README automatico.
Pode usar um repositorio publico contendo somente estes dados ficticios. Nunca suba credenciais.
Copie a URL HTTPS real do seu repositorio.
No mesmo PowerShell, na pasta da solucao:

```powershell
git --version
git init
git branch -M main
git add .
git status
git commit -m "Projeto DimDim com clientes, contas, testes e infraestrutura"
```

Se pedir identificacao, configure `git config user.name "Mateus Pierro"` e
`git config user.email "SEU_EMAIL_DO_GITHUB"` e repita o commit.
Substitua a URL abaixo pela que voce copiou; depois rode:

```powershell
git remote add origin https://github.com/SEU_USUARIO/DimDim-555125-Pipelines.git
git push -u origin main
```

Entre pelo navegador quando o Git solicitar. Nao use a senha comum da conta como PAT.
Confirme no GitHub que aparecem `src`, `tests`, `script.sql` e `azure-pipeline.yml`.
Caso `git` nao exista: instale Git for Windows, reabra PowerShell na pasta e repita.

## 3. Criar projeto Azure DevOps e importar Repos

1. Acesse https://dev.azure.com e entre com sua conta Microsoft.
2. Crie uma organizacao se ainda nao tiver. Azure DevOps e Portal Azure sao interfaces diferentes.
3. New project: nome `DimDim-555125-Pipelines`; visibilidade **Private**.
4. Advanced: Version control **Git**, Work item process **Agile**. Create.
5. Repos > Files > Import repository. Se nao aparecer, menu de repositorios > Import repository.
6. Repository type Git, Clone URL do GitHub, nome `DimDim-555125-Pipelines`.
7. Import. Aguarde e confirme que a branch `main` e os arquivos chegaram.
8. Caso outro nome de branch esteja como padrao, defina `main` como default em Repos > Branches.

O repositorio Azure passa a ser o destino das proximas alteracoes.
Para trabalho local posterior, copie a URL de Clone do Azure e troque o remote:

```powershell
git remote set-url origin https://dev.azure.com/SUA_ORGANIZACAO/DimDim-555125-Pipelines/_git/DimDim-555125-Pipelines
git fetch origin
git branch --set-upstream-to=origin/main main
```

Use a URL EXATA de Clone se houver caracteres codificados no endereco.
Para a demonstracao podemos editar os arquivos no Repos pelo navegador.

## 4. Boards: criar Task e branch relacionada

1. Boards > Work Items > New Work Item > Task.
2. Titulo: `Implementar DimDim Clientes e Contas no Azure Devops`.
3. Assigned to: selecione seu usuario; State: **Active**. Salve.
4. Description: "Migrar API .NET, configurar CI com testes e CD no Azure; demonstrar CRUD em Clientes e Contas com Azure SQL. Responsavel: Mateus Teni Pierro, RM555125."
5. Anote o ID real da Task.
6. Dentro da Task, na secao Development, Create a branch.
7. Nome `feature/ID-implementar-dimdim`, trocando ID pelo numero real; base `main`.
8. Confirme o vinculo da branch na secao Development.

Nao conclua a Task agora e nao altere a taxa ainda. Primeiro valide build/deploy da versao inicial.

## 5. Proteger main antes da alteracao de demonstracao

Repos > Branches > main > menu de tres pontos > Branch policies:

- Minimum number of reviewers: **1**.
- Marque **Allow requestors to approve their own changes**, pois o enunciado permite autoaprovacao.
- Nao habilite bloqueio do voto do autor do ultimo push neste fluxo individual.
- Check for linked work items: **Required**.
- Automatically included reviewers: adicione seu proprio usuario como revisor **Required**.
- Associe sua identidade ao RM555125 (nome de exibicao, quando permitido, e identificacao na Task).
  O campo de revisor deve resolver sua conta real; nao tente criar um usuario ficticio chamado RM555125.
- Opcional: depois de criar CI, adicione Build validation obrigatoria apontando para `DimDim-CI`.

Nao use bypass de politicas na gravacao. O primeiro import foi feito antes das politicas;
agora as mudancas de main devem vir por PR.

## 6. Criar infraestrutura via Azure CLI

Necessario: assinatura Azure ativa e permissao para criar recursos. Azure SQL Basic e pago.
O plano App Service padrao e F1; se indisponivel, avalie outra regiao permitida ou B1 pago.
Nao existe garantia de disponibilidade de SKU ou credito na sua assinatura.

Instale Azure CLI se faltar: https://aka.ms/installazurecliwindows . Reabra o PowerShell.
Na pasta da solucao:

```powershell
az version
az login
az account list --output table
```

Escolha a assinatura certa; nao precisa compartilhar IDs ou senhas comigo.
Rode o script com PowerShell em um processo separado, sem alterar permanentemente a politica:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\infra\01-criar-recursos.ps1 -Location brazilsouth
```

O comando usa apenas o script deste pacote. Leia o arquivo antes de executar.
Se necessario, acrescente `-SubscriptionId "ID_DA_ASSINATURA"`.
O script pede senha SQL sem exibi-la. Guarde-a para o Query Editor, sem colocar no Git.
Usa `brazilsouth` como regiao inicial; se houver bloqueio por politica, confira a lista
permitida no erro antes de tentar outra. Nao escolha uma regiao bloqueada repetidamente.
Os nomes sao armazenados em `infra/recursos.local.json` antes da criacao; em nova tentativa,
o script reutiliza esses nomes. Para corrigir a regiao apos falha inicial, ajuste `location`
nesse arquivo somente se ainda nao houver recursos criados ou use um novo conjunto de nomes.

O script cria RG, servidor SQL, banco `dimdimdb`, plano e Web App Linux .NET 8.
Configura HTTPS, startup, variaveis de ambiente e firewall para IPs de saida do App Service
e seu IP publico. Nao abre todo o SQL para a Internet. Mudou de rede? atualize a regra
SeuComputador no servidor SQL. Escalou o App Service? revise os IPs de saida no firewall.

Quando terminar:

```powershell
Get-Content .\infra\recursos.local.json
```

Esse arquivo tem nomes, nao tem senha, e e ignorado pelo Git.
No https://portal.azure.com, abra o Resource Group criado e mostre todos os recursos.
No Web App > Settings > Environment variables, confira a existencia de
`ConnectionStrings__DimDim`, `DatabaseProvider` e `ASPNETCORE_ENVIRONMENT` sem revelar os valores secretos.
Neste momento a aplicacao ainda nao foi publicada.

No banco `dimdimdb` > Query editor (preview), entre com `dimdimadmin` e sua senha.
Se o editor pedir liberacao de IP, adicione seu IP atual ao firewall do servidor.
Execute o `script.sql` completo para criar Clientes, Contas, indices e FK explicitamente.
O primeiro inicio da API tambem suporta criar o schema em um banco vazio.
Nao execute o script dentro de `master`.

## 7. CI: criar Build e testar manualmente

Pipelines > Pipelines > New pipeline > Azure Repos Git > selecione o repositorio >
Existing Azure Pipelines YAML file > main > `/azure-pipeline.yml` > Continue.
Use Run para iniciar uma execucao manual. Renomeie a pipeline para `DimDim-CI`.
Nao escolha um template que substitua o YAML fornecido.

A execucao deve:
- Instalar .NET 8, restaurar e compilar.
- Executar testes unitarios e integracao com SQLite isolado.
- Publicar resultados na aba Tests (10 casos na versao inicial).
- Gerar artefato `drop`, contendo `DimDim.Api.zip`.

Abra a execucao > Jobs > cada tarefa e confira logs. Abra Tests e o artefato.
Os testes de SQLite nao substituem a demonstracao CRUD com Azure SQL.

Se aparecer **No hosted parallelism has been purchased or granted**:
Organization settings > Pipelines > Parallel jobs. Solicite habilitacao gratuita
Microsoft-hosted pelo link mostrado ou configure um agente self-hosted no seu Windows.
A concessao hosted pode nao ser imediata.
Para self-hosted: Organization settings > Agent pools > crie `DimDimLocal` > New agent > Windows.
Siga os comandos de download/configuracao mostrados no proprio portal; use um PAT de escopo
Agent Pools (Read & manage), nunca compartilhe esse token. Instale SDK .NET 8 e Git na maquina.
Execute `run.cmd` do agente em outro terminal e mantenha-o aberto.
Troque no YAML `pool: { vmImage: ubuntu-latest }` pelo bloco abaixo, por PR se main ja protegida:

```yaml
pool:
  name: DimDimLocal
```

Escolha esse mesmo pool na Release. Autorize a pipeline a usar o pool se solicitado.
O YAML usa tasks multiplataforma. O agente e separado do terminal da API.

## 8. Service connection com Azure

Project settings > Service connections > New service connection > Azure Resource Manager.
Prefira Workload identity federation (automatic), se sua conta permitir criar a identidade.
Selecione a assinatura e restrinja ao Resource Group deste projeto.
Nome: `sc-dimdim-azure`.
A conexao precisa poder implantar no Web App (permissao Contributor no RG ou funcao equivalente).
Nao coloque senha SQL nem client secret no repositorio.
Autorize a pipeline especifica quando solicitar permissao para usar a conexao.
Se criacao automatica falhar por permissao do Entra, sera necessario apoio do administrador
ou um metodo permitido pela sua assinatura; nao invente IDs nem credenciais.

## 9. CD: Release classica e teste manual

O PDF pede Release separada e **Create release**. Use a interface classica.
Se Releases estiver ausente ou criacao bloqueada, confira Organization settings e Project settings >
Pipelines > Settings > **Disable creation of classic release pipelines**.
O administrador da organizacao precisa permitir criacao classica.

1. Pipelines > Releases > New pipeline > Empty job. Stage: `Producao`.
2. Nome da Release: `DimDim-CD`.
3. Add an artifact: source type **Build**, pipeline `DimDim-CI`, default version Latest.
4. Source alias: **`_DimDim-CI`**. Confira exatamente esse nome para o caminho do pacote abaixo.
5. No raio do artefato: Continuous deployment trigger **Enabled**, branch filter **main**.
   Nao dispare CD para feature. Deixe pull request trigger desligado.
6. Stage > Pre-deployment conditions: **After release**, sem aprovacao manual adicional,
   para que o deploy seja automatico apos a aprovacao da PR.
7. Tasks > Agent job: Azure Pipelines, ubuntu-latest (ou seu pool self-hosted).
8. Confirme que o download do artefato esta habilitado no Agent job.
9. Adicione task **Azure App Service deploy**, versao 4 (`AzureRmWebAppDeployment@4`).
10. Connection type: Azure Resource Manager; connection `sc-dimdim-azure`.
11. App type: **Web App on Linux**; App Service name: nome real de `webApp` do JSON local.
12. Package or folder: `$(System.DefaultWorkingDirectory)/_DimDim-CI/drop/DimDim.Api.zip`.
13. Runtime stack: .NET 8, se o campo aparecer; Startup command: `dotnet DimDim.Api.dll`.
14. Save. **Create release** manual, selecione o artefato do build aprovado de main e Create.
15. Abra a release > Producao > Logs. Aguarde sucesso.
16. Abra `https://NOME_REAL.azurewebsites.net/health`, `/swagger` e `/`.

Se a Release disser pacote nao encontrado, confira o alias `_DimDim-CI`, download de artefatos
habilitado e a existencia de `drop/DimDim.Api.zip` no Build.
Se a aplicacao falhar: verifique Log stream do Web App, conexao SQL, schema e firewall.
F1 pode demorar no primeiro acesso. So avance quando health e Swagger responderem na nuvem.

## 10. Video: alteracao funcional e PR

Com tudo funcionando na versao 1%, siga `02-ROTEIRO-VIDEO.md`.
A partir daqui grave a execucao completa das etapas 06 e 07 do PDF.

1. Confirme Task Active e a branch vinculada.
2. No Repos, selecione a **feature** criada pela Task.
3. Edite `src/DimDim.Api/Services/RendimentoService.cs`: `0.01m` -> `0.02m`.
4. No mesmo branch, edite `tests/DimDim.Tests/RendimentoTests.cs`:
   - `[InlineData(1000, 10)]` -> `[InlineData(1000, 20)]`
   - `[InlineData(2500, 25)]` -> `[InlineData(2500, 50)]`
   - `[InlineData(0, 0)]` continua igual.
   - `[InlineData(123.45, 1.23)]` -> `[InlineData(123.45, 2.47)]`.
5. Commit com mensagem `AB#ID Atualiza rendimento demonstrativo para 2 por cento`, usando ID real.
   Pode fazer dois commits no portal; o ultimo precisa conter codigo e testes atualizados.
6. Abra PR da feature para main; inclua Task em Work items e voce como revisor.
7. Mostre os arquivos alterados, politicas, Task e resultado do build da feature.
8. Aprove a PR pelo seu usuario e aguarde todas as politicas.
9. Complete: **desmarque Delete source branch** e marque **Complete associated work items**.
10. Conclua sem bypass. Confirme Task Closed e branch ainda existente.
11. Abra o CI disparado automaticamente em main apos merge; mostre logs, Tests e artefato.
12. Abra a CD disparada automaticamente pelo novo artefato; mostre logs e versao do build usada.
13. Abra a API na nuvem: uma conta de saldo 1000 passa a apresentar rendimento 20 e taxa 0.02.

Uma alteracao so no README nao atende. O codigo funcional e os testes precisam mudar.
Se escolher Build validation na main, podera haver build de PR e build de main: a CD deve usar main.

## 11. CRUD das duas tabelas com prova no banco

Swagger em **HTTPS da nuvem**, nao localhost.
Abra tambem o Query Editor do Azure SQL no banco `dimdimdb` e deixe os SELECTs de
`database/evidencias.sql` preparados. Nao execute todos os CRUDs antes de mostrar o banco.

1. POST cliente (JSON em requests). Anote ID retornado. Execute SELECT Clientes: mostre registro criado.
2. GET cliente por ID e GET lista. Execute SELECT Clientes: compare nome/email e ID.
3. POST conta com clienteId REAL. Anote ID retornado. Execute SELECT Contas e o JOIN.
4. GET conta por ID e GET lista. Execute SELECT Contas: compare saldo e relacionamento.
5. Com saldo 1000, GET rendimento: mostre 20 apos deploy (antes era 10).
6. PUT cliente com JSON atualizado. Execute SELECT Clientes: mostre campos alterados.
7. PUT conta com saldo 2500, clienteId real. Execute SELECT Contas: mostre saldo alterado.
8. DELETE conta. GET por ID deve dar 404. Execute SELECT Contas: mostre ausencia.
9. DELETE cliente. GET por ID deve dar 404. Execute SELECT Clientes: mostre ausencia.

Se existirem outros dados, filtre pelo ID real para deixar clara a ausencia do registro excluido.
Nao precisa reiniciar identidades nem remover tabelas. IDs nao precisam comecar em 1.
Mostre ainda os recursos criados pelo script no Portal e a existencia de variaveis sem expor segredos.

## 12. Acesso do professor

O PDF combina "administrador do Projeto" com "Contributor no Projeto".
Para atender aos dois sem dar administracao de toda a organizacao:

1. Organization settings > Users > Add users > `pf0841@fiap.com.br`.
2. Access level **Basic**, adicione ao projeto como **Contributors**.
3. Project settings > Permissions > **Project Administrators** > Members > Add:
   adicione a mesma identidade do professor.
4. Verifique que o usuario tem Basic na organizacao e acesso ao projeto, Repos, Boards e Pipelines.
5. Se convites externos estiverem bloqueados pelo Entra, resolva com o administrador antes de entregar.

Basic e nivel de acesso da organizacao; Contributor/Project Administrator sao grupos do projeto.
Nao e necessario adicionar o professor a Project Collection Administrators da organizacao.
Nao remova o acesso nem os recursos antes da correcao.

## 13. PDF final e Teams

Quando os links reais existirem, use `03-CHECKLIST-ENTREGA.md` e me envie:
- Nome definitivo do grupo e integrantes/RMs.
- Link do projeto Azure DevOps.
- Link do video no YouTube (nao listado funciona se acessivel pelo link).

Com isso geramos o PDF final `DimDimFlow_pipelines.pdf` ou com o nome escolhido.
Nao entregue um PDF com campos de preenchimento ou links ficticios.
No Teams, apenas o representante envia o PDF. O codigo completo fica no Repos.
