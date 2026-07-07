# Expense Control (Fullstack)

Sistema completo de controle de gastos (receitas e despesas) construído com **.NET 10 (Arquitetura Hexagonal)** no Backend e **React + Vite + Tailwind v4** no Frontend. O objetivo do sistema é gerenciar pessoas e suas transações financeiras, garantindo regras de negócio (como a proibição de menores de idade cadastrarem receitas) de ponta a ponta.

## Tecnologias Utilizadas

### Backend
- **.NET SDK:** `10.0-preview`
- **Linguagem:** C# 14
- **Banco de Dados:** PostgreSQL `15-alpine`
- **ORM:** Entity Framework Core `10.0.9`
- **Provider PostgreSQL:** Npgsql.EntityFrameworkCore.PostgreSQL `10.0.2`
- **Documentação de API:** Scalar.AspNetCore `2.16.10`
- **Containerização:** Docker e Docker Compose (Multi-stage build)

### Frontend
- **Framework:** React 19 + Vite
- **Estilização:** Tailwind CSS v4
- **Ícones:** Lucide React
- **Comunicação HTTP:** Axios
- **Linguagem:** TypeScript

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
- `GET /api/persons` - Retorna uma lista paginada de pessoas. Suporta paginação (`?page=1&pageSize=10`) e filtros por nome (`?name=Joao`) ou idade (`?age=30`).
- `GET /api/persons/{id}` - Retorna os detalhes de uma pessoa e todas as suas transações.
- `POST /api/persons` - Cria uma nova pessoa. (Validações: Nome min 2 caracteres, Idade entre 1 e 150)
- `PUT /api/persons/{id}` - Atualiza os dados de uma pessoa (Nome e Idade).
- `DELETE /api/persons/{id}` - Deleta uma pessoa e, em cascata, todas as suas transações.

### Transações (`/api/transactions`)
- `GET /api/transactions` - Retorna uma lista paginada de transações. Suporta paginação (`?page=1&pageSize=10`) e filtros por tipo (`?type=Income`) ou ID da pessoa (`?personId=...`).
- `POST /api/transactions` - Cria uma nova transação. (Validações: Valor positivo, tipo `Income` ou `Expense`, menores de 18 anos não podem cadastrar `Income`).
- `PUT /api/transactions/{id}` - Atualiza uma transação. (Validações: Não é possível trocar o tipo de uma criança para `Income`).
- `DELETE /api/transactions/{id}` - Deleta uma transação.

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

### Configuração Inicial (Variáveis de Ambiente)
Antes de rodar o projeto, você precisa configurar as variáveis de ambiente.

1. **Na raiz do projeto (Backend e Banco de Dados):**
   Copie o arquivo `.env.example` para `.env` e preencha os dados do banco (ou mantenha os padrões para testes locais):
   ```bash
   cp .env.example .env
   ```

2. **Na pasta `frontend` (se for rodar localmente):**
   Copie o arquivo `.env.example` para `.env` e aponte para a URL da API:
   ```bash
   cd frontend
   cp .env.example .env
   # Conteúdo padrão: VITE_API_URL=http://localhost:5023/api
   ```

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

### Rodando o Frontend (Localmente)

Após subir o Backend, abra uma nova aba do terminal:
1. Entre na pasta do frontend:
   ```bash
   cd frontend
   ```
2. Instale as dependências do Node:
   ```bash
   npm install
   ```
3. Rode o servidor de desenvolvimento:
   ```bash
   npm run dev
   ```
4. A interface web estará disponível em: `http://localhost:5173`

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
   - Fazer o build otimizado (Multi-stage com Nginx) e iniciar o container do Frontend (`expense_control_frontend`).
3. A API estará disponível em: `http://localhost:8080`
4. A Interface Web estará disponível em: `http://localhost:5173`

Para ver os logs da aplicação no Docker:
```bash
docker compose logs app -f
```

Para derrubar os containers:
```bash
docker compose down
```

---

## Testes Unitários

O projeto possui uma robusta suíte de testes unitários na camada de Aplicação (`ExpenseControl.Tests`), garantindo a estabilidade e prevenindo regressões de regras de negócio.

### Tecnologias de Teste Utilizadas:
- **xUnit:** Framework base para estruturação e execução.
- **Moq:** Isolamento e simulação da camada de repositórios (não necessita de banco real).
- **FluentAssertions:** Asserções legíveis e expressivas.

### Cobertura:
- **PersonService:** Cenários completos de paginação, resgate, criação, atualização e deleção em cascata.
- **TransactionService:** Validação rigorosa de regras de negócio complexas (ex: restrição de menores de idade registrando receitas) e verificação matemática na consolidação de saldos.

Para rodar os testes localmente, basta executar na raiz do projeto:
```bash
dotnet test
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

## Padrões de Código e Formatação

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
