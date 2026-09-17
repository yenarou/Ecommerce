const { gql } = require('graphql-tag')

const typeDefs = gql`
  type Category {
    id: ID!
    slug: String!
    name: String!
    description: String
    products: [Product!]!
  }

  type Customization {
    id: ID!
    description: String!
    imageUrl: String
    additionalPrice: Float!
  }

  type Product {
    id: ID!
    name: String!
    description: String
    price: Float!
    stock: Int!
    imageUrl: String
    imageUrlAlt: String
    daysToMake: Int!
    createdAt: String!
    category: Category!
    customizationOptions: [Customization!]!
  }

  type User {
    id: ID!
    username: String!
    email: String!
    authProvider: String!
  }

  type OrderItem {
    id: ID!
    quantity: Int!
    unitPrice: Float!
    subtotal: Float!
    product: Product!
    customization: Customization
  }

  type Order {
    id: ID!
    status: String!
    createdAt: String!
    updatedAt: String!
    total: Float!
    user: User!
    items: [OrderItem!]!
  }

  # --- paginación del catálogo ---
  type ProductPage {
    items: [Product!]!
    total: Int!
    page: Int!
    pageSize: Int!
    totalPages: Int!
  }

  type Query {
    # Lectura
    categories: [Category!]!
    category(slug: String!): Category
    products(page: Int = 1, pageSize: Int = 12, categorySlug: String): ProductPage!
    product(id: ID!): Product
    orders(userId: ID!): [Order!]!
  }

  input ProductInput {
    categoryId: ID!
    name: String!
    description: String
    price: Float!
    stock: Int!
    imageUrl: String
    imageUrlAlt: String
    daysToMake: Int!
  }

  input OrderItemInput {
    productId: ID!
    customizationId: ID
    quantity: Int!
  }

  input CreateOrderInput {
    userId: ID!
    items: [OrderItemInput!]!
  }

  type Mutation {
    # Escritura
    createProduct(input: ProductInput!): Product!
    updateProduct(id: ID!, input: ProductInput!): Product!
    deleteProduct(id: ID!): Boolean!
    createOrder(input: CreateOrderInput!): Order!
  }
`

module.exports = typeDefs
