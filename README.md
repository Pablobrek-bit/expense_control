# Expense Control API 

Sistema de controle de gastos (receitas e despesas) construído com **.NET 10** e **Arquitetura Hexagonal**. O objetivo da API é gerenciar pessoas e suas transações financeiras, com regras de negócio específicas (ex: menores de idade não podem registrar despesas) e fornecimento de resumos consolidados.

## Tecnologias Utilizadas

- **.NET SDK:** `10.0-preview`
- **Linguagem:** C# 14
- **Banco de Dados:** PostgreSQL `15-alpine`
- **ORM:** Entity Framework Core `10.0.9`
- **Provider PostgreSQL:** Npgsql.EntityFrameworkCore.PostgreSQL `10.0.2`
- **Documentação de API:** Scalar.AspNetCore `2.16.10`
- **Containerização:** Docker e Docker Compose (Multi-stage build)

---

## Arquitetura

O projeto foi desenhado seguindo os princípios da **Arquitetura Hexagonal (Ports and Adapters)**, garantindo baixo acoplamento e alta coesão:

- **Domain:** Entidades (`Person`, `Transaction`), Enums (`TransactionType`) e interfaces de repositório. Totalmente isolada de frameworks externos.
- **Application:** Casos de uso (`PersonService`, `TransactionService`, `SummaryService`), regras de negócio e DTOs.
- **Infrastructure:** Adaptadores de entrada e saída, incluindo os `Controllers`, configuração do EF Core, e implementações de repositórios (`AppDbContext`, `PersonRepository`).

---

## Rotas da API (Endpoints)

A API possui as seguintes rotas principais:

### Pessoas (`/api/persons`)
- `GET /api/persons` - Lista todas as pessoas.
- `POST /api/persons` - Cria uma nova pessoa. (Validações: Nome min 2 caracteres, Idade entre 1 e 150)
- `DELETE /api/persons/{id}` - Deleta uma pessoa e, em cascata, todas as suas transações.

### Transações (`/api/transactions`)
- `GET /api/transactions` - Lista todas as transações.
- `POST /api/transactions` - Cria uma nova transação. (Validações: Valor positivo, tipo `Income` ou `Expense`, menores de 18 anos não podem cadastrar `Expense`).

### Resumo (`/api/summary`)
- `GET /api/summary` - Retorna um balanço consolidado contendo:
  - Total de Receitas (Geral)
  - Total de Despesas (Geral)
  - Saldo Líquido (Geral)
  - Lista de pessoas com seus respectivos totais e saldos individuais.

---

## Documentação (Scalar / OpenAPI)

A API possui uma interface gráfica moderna para testes e visualização da documentação gerada via **Scalar**.

Para acessar a documentação:
1. Rode a aplicação em ambiente de desenvolvimento (`Development`).
2. Acesse no navegador: `http://localhost:5023` (você será redirecionado para a UI do Scalar).

> **Aviso:** A interface do Scalar está configurada para ser exibida apenas em ambiente de Desenvolvimento (`app.Environment.IsDevelopment()`). Quando a aplicação rodar via Docker (configurado como `Production`), essa rota estará desabilitada por segurança.

Além do Scalar, você pode utilizar o arquivo **`ExpenseControl.http`** incluído na raiz do projeto para realizar testes rápidos das rotas diretamente pelo Visual Studio ou VS Code tendo a extensão REST Client instalada.

---

## Como Executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker e Docker Compose](https://www.docker.com/products/docker-desktop/)

### Opção 1: Rodando Localmente

1. Suba apenas o banco de dados usando o arquivo compose:
   ```bash
   docker compose up db -d
   ```
2. Navegue até o diretório da infraestrutura:
   ```bash
   cd ExpenseControl.Infrastructure
   ```
3. Inicie a aplicação (as migrações serão aplicadas automaticamente no banco de dados):
   ```bash
   dotnet run
   ```
4. A API estará disponível em: `http://localhost:5023`

### Opção 2: Rodando tudo com Docker

O projeto possui um `Dockerfile` otimizado utilizando **Multi-stage Build**, garantindo uma imagem final extremamente leve, sem o SDK do .NET, contendo apenas os binários necessários.

1. Na raiz do projeto, execute:
   ```bash
   docker compose up --build -d
   ```
2. O Docker Compose irá:
   - Subir o container do PostgreSQL (`expense_control_db`).
   - Aguardar o banco de dados estar saudável (via *Healthcheck*).
   - Fazer o build e iniciar o container da API (`expense_control_app`).
3. A API estará disponível em: `http://localhost:8080`

Para ver os logs da aplicação no Docker:
```bash
docker compose logs app -f
```

Para derrubar os containers:
```bash
docker compose down
```

---

## Tratamento de Erros

A API possui um **Filtro Global de Exceções** (`GlobalExceptionFilter`) que intercepta erros e garante que o cliente sempre receba uma resposta padronizada no formato JSON:

```json
{
  "status": 400,
  "message": "Mensagem descritiva do erro em português."
}
```

Erros de validação (como campos obrigatórios vazios, valores negativos ou UUIDs inválidos) também são interceptados e formatados de maneira amigável em português, sem vazar nomenclaturas internas do .NET.

---

## 💅 Padrões de Código e Formatação

O projeto utiliza o **`.editorconfig`** nativo do ecossistema .NET para garantir que todo o código mantenha o mesmo padrão profissional. 

As seguintes regras estão ativas:
- Indentação de 4 espaços.
- `PascalCase` para Classes e Métodos.
- Prefixo `I` obrigatório para Interfaces (ex: `IPersonRepository`).
- `_camelCase` obrigatório para campos privados (como injeções de dependência).
- Limpeza e ordenação automática de `using` (imports).

Para verificar e formatar automaticamente todo o projeto (similar ao `eslint --fix`), basta executar na raiz:
```bash
dotnet format
```

Para ver o log detalhado e checar quais avisos o compilador encontrou (sem quebrar a formatação), você pode utilizar:
```bash
dotnet format -v diag
```
