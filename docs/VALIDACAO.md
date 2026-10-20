# Validacao do pacote preparado

Foram revisados: estrutura da solucao, XML dos projetos, JSONs, YAML, rotas CRUD,
relacionamento EF/DDL, caminhos do artefato de CI para CD e instrucoes de entrega.

O ambiente de preparacao nao conseguiu executar o .NET CLI: a leitura de informacoes
de processo pelo runtime falhou antes de carregar a solucao (System.Diagnostics.Process).
Portanto nao ha afirmacao de build/testes aprovados neste ambiente nem de deploy Azure executado.

Primeiro passo no Windows: `dotnet restore` e `dotnet test --configuration Release`.
Depois, validar banco Azure SQL e CRUD em nuvem, alem das pipelines na conta do aluno.
A compilacao/testes locais e execucoes no Azure continuam pendentes ate essa verificacao.
