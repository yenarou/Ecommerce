import { gql } from './client'

function normalizeProduct(p) {
  if (!p) return null
  return {
    id: p.id,
    categoryId: p.categoryId,
    name: p.name,
    description: p.description,
    price: p.price,
    currency: p.currency || 'MXN',
    stock: p.stock,
    images: p.images || [],
    category: p.category || { name: p.categoryName }, // Adaptar a lo que venga
  }
}

function normalizeCategory(c) {
  if (!c) return null
  return {
    id: c.id,
    slug: c.slug,
    name: c.name,
    description: c.description,
  }
}

export async function fetchCategories() {
  const data = await gql(`
    query {
      categories {
        id slug name description
      }
    }
  `)
  return data.categories.map(normalizeCategory)
}

export async function fetchCategoryBySlug(slug) {
  const data = await gql(`
    query($slug: String!) {
      category(slug: $slug) {
        id slug name description
      }
    }
  `, { slug })
  const category = normalizeCategory(data.category)
  if (!category) return null

  const firstPage = await fetchProducts({ page: 1, pageSize: 50 })
  const products = [...firstPage.items]

  for (let page = 2; page <= firstPage.totalPages; page += 1) {
    const nextPage = await fetchProducts({ page, pageSize: 50 })
    products.push(...nextPage.items)
  }

  return {
    ...category,
    products: products.filter(product => product.categoryId === category.id),
  }
}

export async function fetchProducts({ page = 1, pageSize = 12, categorySlug } = {}) {
  const data = await gql(`
    query($page: Int, $pageSize: Int, $categorySlug: String) {
      products(page: $page, pageSize: $pageSize, categorySlug: $categorySlug) {
        total totalPages page pageSize
        items { id categoryId name price stock images { url alt } categoryName }
      }
    }
  `, { page, pageSize, categorySlug })

  return {
    ...data.products,
    items: data.products.items.map(normalizeProduct),
  }
}

export async function fetchProductById(id) {
  const data = await gql(`
    query($id: UUID!) {
      product(id: $id) {
        id categoryId name description price stock images { id url alt }
        categoryName
      }
    }
  `, { id })
  return normalizeProduct(data.product)
}

const USE_MOCK_ORDERS = true
export async function createOrder(userId, items) {
  if (USE_MOCK_ORDERS) {
    return { id: `pedido-${Date.now()}`, status: 'pending', total: 0, items: [] }
  }
  const data = await gql(`
    mutation($input: CreateOrderInput!) {
      createOrder(input: $input) {
        id status total
        items { quantity unitPrice product { name } }
      }
    }
  `, { input: { userId, items } })
  return data.createOrder
}