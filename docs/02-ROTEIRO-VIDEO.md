# Roteiro de gravacao

Resolucao minima 720p, texto legivel e explicacao em voz. O PDF nao fixa duracao maxima.
Evite cortes que escondam alteracao, PR, execucao de pipelines ou evidencia no banco.
Aguarde CI/CD mantendo a gravacao; nao precisa fingir execucao instantanea.

## Antes de gravar

- CI e CD manuais aprovadas; API e Azure SQL funcionando na nuvem.
- Task Active, feature relacionada e main protegida.
- Taxa inicial 1%; se voce ja mudou antes, sera necessario preparar outra alteracao real com testes.
- Abra abas do Boards, Repos, PRs, Pipelines, Releases, Portal, Swagger e Query Editor.
- Crie cliente e conta ficticios com saldo 1000 para mostrar rendimento inicial 10.
- Nao deixe senhas, tokens ou strings de conexao visiveis.

## Sequencia obrigatoria para a gravacao

1. Apresentacao: "Sou Mateus Teni Pierro, RM555125. Esta e a API DimDim Clientes e Contas, com Azure SQL e pipeline de CI/CD no Azure DevOps."
2. Mostre projeto, Task atribuida e Active, branch relacionada e politicas da main.
3. Mostre o endpoint de rendimento inicial em nuvem: saldo 1000, taxa 0.01, rendimento 10.
4. Na feature, altere taxa para 0.02 e os valores esperados nos testes. Mostre codigo e diff.
5. Commit associado a Task. Abra PR, mostre Work item e as politicas.
6. Aprove. Ao concluir, preserve branch e solicite conclusao dos Work items.
7. Mostre Task Closed, PR concluida e branch preservada.
8. Mostre CI automatico de main: motivo/commit, logs de restore/build/test/publish, aba Tests e pacote.
9. Mostre Release automatica: numero do artefato/build consumido, tarefas e deploy concluido.
10. Volte a URL em nuvem e mostre taxa 0.02, rendimento 20 para saldo 1000.
11. Mostre Portal: RG, Web App, SQL server, banco e plano. Abra script de criacao e compare nomes.
12. Mostre existencia das variaveis de ambiente com valores secretos ocultos.
13. Demonstre TODOS os CRUDs de Clientes e Contas na API, alternando para SELECT no banco apos cada operacao, conforme guia.
14. Mostre professor cadastrado com Basic e os grupos do projeto.
15. Encerramento: informe que fonte, DDL, scripts, JSONs e YAML estao no Repos.

A API retorna 204 sem corpo para PUT/DELETE. A prova de alteracao/exclusao e o GET e SELECT.
Ao excluir, primeiro conta, depois cliente. Use IDs reais; nao presuma ID 1.

## Depois

Publique no YouTube com acesso pelo link, teste em janela anonima e copie o endereco.
Nao exclua a branch nem recursos enquanto a correcao estiver pendente.
