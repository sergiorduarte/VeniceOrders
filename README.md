

\## 🚀 Como Rodar



\### \*\*Opção 1: Docker Compose (Recomendado)\*\*



```bash

\# 1. Clone o repositório

git clone <seu-repositorio>

cd VeniceOrders



\# 2. Suba todos os serviços

docker compose up --build



\# 3. Aguarde todos os containers iniciarem (pode levar 1-2 minutos)

\# A API estará disponível em: http://localhost:5000

```



\### \*\*Opção 2: Rodar Localmente\*\*



```bash

\# 1. Suba apenas as dependências

docker compose up sqlserver mongodb redis rabbitmq -d



\# 2. Configure a connection string no appsettings.json



\# 3. Execute migrations

cd src/VeniceOrders.API

dotnet ef database update



\# 4. Rode a aplicação

dotnet run

```



---



\## 🔐 Autenticação



\### \*\*1. Obter Token JWT\*\*



```bash

POST http://localhost:5000/api/auth/login

Content-Type: application/json



{

&nbsp; "username": "admin",

&nbsp; "password": "Venice@2025"

}

```



\*\*Resposta:\*\*

```json

{

&nbsp; "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

}

```



---



\## 📡 Endpoints



\### \*\*Base URL\*\*: `http://localhost:5000/api`



\### \*\*1. Criar Pedido\*\*



```bash

POST /orders

Authorization: Bearer {seu-token}

Content-Type: application/json



{

&nbsp; "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

&nbsp; "itens": \[

&nbsp;   {

&nbsp;     "produto": "Notebook Dell",

&nbsp;     "quantidade": 2,

&nbsp;     "precoUnitario": 3500.00

&nbsp;   },

&nbsp;   {

&nbsp;     "produto": "Mouse Logitech",

&nbsp;     "quantidade": 3,

&nbsp;     "precoUnitario": 150.00

&nbsp;   }

&nbsp; ]

}

```



\*\*Resposta (201 Created):\*\*

```json

{

&nbsp; "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",

&nbsp; "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

&nbsp; "data": "2025-01-14T10:30:00Z",

&nbsp; "status": 0,

&nbsp; "itens": \[...],

&nbsp; "total": 7450.00

}

```



\### \*\*2. Buscar Pedido por ID\*\*



```bash

GET /orders/{id}

Authorization: Bearer {seu-token}

```



\*\*Resposta (200 OK):\*\*

```json

{

&nbsp; "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",

&nbsp; "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

&nbsp; "data": "2025-01-14T10:30:00Z",

&nbsp; "status": 0,

&nbsp; "itens": \[

&nbsp;   {

&nbsp;     "produto": "Notebook Dell",

&nbsp;     "quantidade": 2,

&nbsp;     "precoUnitario": 3500.00

&nbsp;   }

&nbsp; ],

&nbsp; "total": 7450.00

}

