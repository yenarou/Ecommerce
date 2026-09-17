# Backend — GraphQL (tejidos / crochet shop)

API GraphQL en **un solo endpoint** que sirve al frontend de React (WebProyecto).

## Stack

- Node.js + [Apollo Server](https://www.apollographql.com/docs/apollo-server) (standalone) — servidor GraphQL
- [better-sqlite3](https://github.com/WiseLibs/better-sqlite3) — base de datos SQLite real, en un archivo
- `db/db.sql` — esquema + datos semilla, 100% replicable en cualquier máquina, sin depender de la nube

## Requisitos

- Node.js 18 o superior

## Instalación y arranque

```bash
cd backend
npm install
npm run db:init
npm run dev
```

Verás:

```
Servidor GraphQL listo en http://localhost:4000/
```

Ese es el único endpoint: `http://localhost:4000/`. Ábrelo en el navegador
para usar el Apollo Sandbox o mándale peticiones POST directamente.

> Si alguna vez quieres borrar todo y volver a los datos de ejemplo, corre de
> nuevo `npm run db:init` (elimina `db/data.sqlite` y lo vuelve a crear).


## Estructura

```
backend/
  db/
    db.sql          -> DDL + datos de ejemplo, en un solo archivo (el DER llevado a SQL)
    init.js          -> script que arma data.sqlite desde db.sql
    data.sqlite       -> (se genera solo, no se sube a git)
  src/
    db.js           -> conexión a la base de datos
    typeDefs.js      -> schema GraphQL (SDL): tipos, queries, mutations
    resolvers.js     -> lógica de cada query/mutation y de las relaciones anidadas
    server.js        -> arranca Apollo Server en un solo endpoint HTTP
```

## Operaciones disponibles

### Queries (lectura)

| Operación | Qué hace |
|---|---|
| `categories` | Lista todas las categorías, cada una con sus `products` anidados |
| `category(slug)` | Trae una categoría puntual por slug |
| `products(page, pageSize, categorySlug)` | Catálogo paginado, con filtro opcional por categoría |
| `product(id)` | Un producto puntual, con su `category` y `customizationOptions` |
| `orders(userId)` | Historial de pedidos de un usuario, con sus `items` |

### Mutations (escritura)

| Operación | Qué hace |
|---|---|
| `createProduct(input)` | Registra un producto nuevo |
| `updateProduct(id, input)` | Actualiza un producto existente |
| `deleteProduct(id)` | Elimina un producto |
| `createOrder(input)` | Registra un pedido junto con sus renglones (`items`), descontando stock |

## Ejemplos rápidos (Apollo Sandbox o curl)

```graphql
query {
  categories {
    name
    products { id name price stock }
  }
}
```

```graphql
mutation {
  createOrder(input: {
    userId: "11111111-1111-1111-1111-111111111111"
    items: [{ productId: "p1000000-0000-0000-0000-000000000001", quantity: 2 }]
  }) {
    id
    total
    items { product { name } subtotal }
  }
}
```