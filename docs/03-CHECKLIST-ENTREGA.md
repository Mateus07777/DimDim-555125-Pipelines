# Checklist conforme o PDF do professor

Os itens abaixo precisam ser conferidos na conta real. Arquivo presente nao significa execucao comprovada.

## Projeto e Azure DevOps

- [ ] Projeto Java/.NET sem ser Sprint: API .NET DimDim deste pacote.
- [ ] Duas tabelas relacionadas: Clientes e Contas, FK ClienteId.
- [ ] Banco Azure SQL em nuvem.
- [ ] Projeto Azure DevOps novo, Git e Agile.
- [ ] Task com titulo exigido, responsavel e estado inicial Active.
- [ ] Repositorio importado.
- [ ] Branch criada a partir da Task e vinculada.
- [ ] Main protegida: 1 revisor, Work item obrigatorio e seu usuario/RM como revisor padrao.
- [ ] CI manual aprovado, testes reais publicados e pacote gerado.
- [ ] Release CD manual aprovada pelo comando Create release.
- [ ] Alteracao em codigo funcional e testes na feature.
- [ ] PR aberta, vinculada, aprovada e concluida.
- [ ] Branch preservada e Task fechada pela conclusao da PR.
- [ ] CI automatico apos merge em main.
- [ ] Release automatica usando o pacote desse build.
- [ ] Logs de cada tarefa nas duas pipelines mostrados.
- [ ] Resultado dos testes unitarios publicado no CI mostrado.
- [ ] Aplicacao acessivel em nuvem depois da alteracao.

## Repositorio

- [ ] Codigo completo, solucao e projetos de testes.
- [ ] script.sql com DDL de Clientes, Contas e FK.
- [ ] infra/01-criar-recursos.ps1 com Azure CLI.
- [ ] azure-pipeline.yml (nome do arquivo usado na configuracao CI).
- [ ] JSONs de POST e PUT das duas tabelas e colecao com GET/DELETE sem corpo.
- [ ] Sem credenciais, PATs, dumps de senhas, arquivos locais de banco ou bin/obj.
- [ ] Nao usa Docker: Dockerfile e compose nao se aplicam a esta solucao App Service.

## Evidencias

- [ ] Recursos criados via script e exibidos no Portal Azure.
- [ ] Variaveis de ambiente e segredos protegidos.
- [ ] CRUD inteiro de Clientes na API em nuvem + SELECT apos cada operacao.
- [ ] CRUD inteiro de Contas na API em nuvem + SELECT apos cada operacao.
- [ ] FK evidenciada por JOIN/consulta e dados relacionados.
- [ ] Video completo dos passos 06 e 07 do enunciado.
- [ ] Video pelo menos 720p, com voz e sem cortes que prejudiquem a prova.
- [ ] Professor pf0841@fiap.com.br com Basic na organizacao.
- [ ] Professor nos grupos Contributors e Project Administrators do projeto.

## PDF para o Teams

- [ ] Nome: <nomeGrupo>_pipelines.pdf (sugestao DimDimFlow_pipelines.pdf).
- [ ] Folha de rosto com grupo, nomes completos e RMs de TODOS os integrantes.
- [ ] Link REAL do projeto Azure DevOps.
- [ ] Link REAL do video no YouTube, acessivel pelo link.
- [ ] Apenas representante faz upload no Teams.

O PDF final sera criado depois que os links existirem. Este checklist nao substitui o PDF.
Sem video/codigo ou sem acesso do professor, a correcao pode ser inviabilizada.
