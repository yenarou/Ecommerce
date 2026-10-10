import { gql } from './client'

const PRODUCT_FIELDS = `
  id
  name
  description
  price
  currency
  stock
  createdAt
  categoryId
  categoryName
  images { id url alt }
`

export async function getAdminProducts() {
  const data = await gql(`query { products { items { ${PRODUCT_FIELDS} } total totalPages page } }`)
  return data.products?.items || []
}

export async function getProduct(id) {
  const data = await gql(`query($id: UUID!) { product(id: $id) { ${PRODUCT_FIELDS} } }`, { id })
  return data.product
}

export async function createProduct(input) {
  const data = await gql(`
    mutation($input: CreateProductInput!) {
      createProduct(input: $input) { ${PRODUCT_FIELDS} }
    }
  `, { input })
  return data.createProduct
}

export async function updateProduct(productId, input) {
  const data = await gql(`
    mutation($productId: UUID!, $input: UpdateProductInput!) {
      updateProduct(productId: $productId, input: $input) { ${PRODUCT_FIELDS} }
    }
  `, { productId, input })
  return data.updateProduct
}

export async function updateProductPrice(productId, price, currency) {
  const data = await gql(`
    mutation($productId: UUID!, $price: Decimal!, $currency: String!) {
      updateProductPrice(productId: $productId, price: $price, currency: $currency) { ${PRODUCT_FIELDS} }
    }
  `, { productId, price, currency })
  return data.updateProductPrice
}

export async function updateProductStock(productId, stock) {
  const data = await gql(`
    mutation($productId: UUID!, $stock: Int!) {
      updateProductStock(productId: $productId, stock: $stock) { ${PRODUCT_FIELDS} }
    }
  `, { productId, stock })
  return data.updateProductStock
}

export async function deleteProduct(productId) {
  return gql(`mutation($productId: UUID!) { deleteProduct(productId: $productId) }`, { productId })
}

export async function setProductPublication(productId, published) {
  const data = await gql(`
    mutation($productId: UUID!, $published: Boolean!) {
      setProductPublication(productId: $productId, published: $published) { ${PRODUCT_FIELDS} }
    }
  `, { productId, published })
  return data.setProductPublication
}

export async function getCategories() {
  const data = await gql(`query { categories { id name slug description } }`)
  return data.categories || []
}

export async function createCategory(input) {
  const data = await gql(`
    mutation($input: CreateCategoryInput!) {
      createCategory(input: $input) { id name slug description }
    }
  `, { input })
  return data.createCategory
}

export async function updateCategory(categoryId, input) {
  const data = await gql(`
    mutation($categoryId: UUID!, $input: UpdateCategoryInput!) {
      updateCategory(categoryId: $categoryId, input: $input) { id name slug description }
    }
  `, { categoryId, input })
  return data.updateCategory
}

export async function deleteCategory(categoryId) {
  return gql(`mutation($categoryId: UUID!) { deleteCategory(categoryId: $categoryId) }`, { categoryId })
}

export async function getAdminOrders() {
  const data = await gql(`query {
    adminOrders {
      id createdAt updatedAt status total
      user { id email }
      items { id quantity unitPrice product { id name } }
    }
  }`)
  return data.adminOrders || []
}

export async function getAdminOrder(id) {
  const data = await gql(`query($id: UUID!) {
    adminOrder(id: $id) {
      id createdAt updatedAt status total
      user { id email }
      items { id quantity unitPrice product { id name } }
    }
  }`, { id })
  return data.adminOrder
}

export async function updateOrderStatus(orderId, status) {
  const data = await gql(`
    mutation($orderId: UUID!, $status: String!) {
      updateOrderStatus(orderId: $orderId, status: $status) { id status updatedAt }
    }
  `, { orderId, status })
  return data.updateOrderStatus
}

export async function uploadProductImage(file) {
  const token = localStorage.getItem('ecommerce_admin_token')
  const operations = JSON.stringify({ query: `mutation($file: Upload!) { uploadProductImage(file: $file) }`, variables: { file: null } })
  const map = JSON.stringify({ '0': ['variables.file'] })
  const form = new FormData()
  form.append('operations', operations)
  form.append('map', map)
  form.append('0', file)

  const response = await fetch((import.meta.env.PUBLIC_GRAPHQL_URL || 'http://localhost:14001/graphql'), {
    method: 'POST',
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    body: form,
  })
  const payload = await response.json()
  if (payload.errors?.length) throw new Error(payload.errors.map(e => e.message).join('\n'))
  return payload.data.uploadProductImage
}
