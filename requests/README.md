# Requisicoes CRUD
Use a URL HTTPS da nuvem. Copie o ID retornado no POST: ele pode ser diferente de 1.

|Tabela|Operacao|Metodo e caminho|Corpo|
|---|---|---|---|
|Clientes|Create|POST /api/clientes|cliente-post.json|
|Clientes|Read|GET /api/clientes e GET /api/clientes/{id}|Sem corpo|
|Clientes|Update|PUT /api/clientes/{id}|cliente-put.json|
|Clientes|Delete|DELETE /api/clientes/{id}|Sem corpo|
|Contas|Create|POST /api/contas|conta-post.json; ajustar clienteId|
|Contas|Read|GET /api/contas e GET /api/contas/{id}|Sem corpo|
|Contas|Update|PUT /api/contas/{id}|conta-put.json; ajustar clienteId|
|Contas|Delete|DELETE /api/contas/{id}|Sem corpo|

GET e DELETE nao possuem JSON de entrada; veja seus exemplos na colecao Postman.
No Swagger: abra operacao > Try it out > cole o JSON > Execute.
Crie cliente antes da conta. Exclua conta antes do cliente. A API bloqueia exclusao de cliente com conta.
Status esperados: POST 201; GET 200; PUT/DELETE 204; GET de excluido 404.
