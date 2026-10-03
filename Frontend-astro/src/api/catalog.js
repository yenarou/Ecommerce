import { gql } from './client'

function normalizeCustomization(c) {
  if (!c) return null
  return {
    id: c.id,
    description: c.description,
    image_url: c.imageUrl,
    additional_price: c.additionalPrice,
  }
}

function normalizeProduct(p) {
  if (!p) return null
  return {
    id: p.id,
    name: p.name,
    description: p.description,
    price: p.price,
    stock: p.stock,
    image_url: p.imageUrl,
    image_url_alt: p.imageUrlAlt,
    days_to_make: p.daysToMake,
    category: p.category, // { slug, name } cuando venga incluido
    customizationOptions: p.customizationOptions
      ? p.customizationOptions.map(normalizeCustomization)
      : undefined,
  }
}

function normalizeCategory(c) {
  if (!c) return null
  return {
    id: c.id,
    slug: c.slug,
    name: c.name,
    description: c.description,
    products: c.products ? c.products.map(normalizeProduct) : undefined,
  }
}

export async function fetchCategories() {
  const data = await gql(`
    query {
      categories {
        slug name description
        products { id name price stock imageUrl daysToMake }
      }
    }
  `)
  return data.categories.map(normalizeCategory)
}

export async function fetchCategoryBySlug(slug) {
  const data = await gql(`
    query($slug: String!) {
      category(slug: $slug) {
        slug name description
        products { id name price stock imageUrl daysToMake }
      }
    }
  `, { slug })
  return normalizeCategory(data.category)
}

export async function fetchProducts({ page = 1, pageSize = 12, categorySlug } = {}) {
  const data = await gql(`
    query($page: Int, $pageSize: Int, $categorySlug: String) {
      products(page: $page, pageSize: $pageSize, categorySlug: $categorySlug) {
        total totalPages page
        items { id name price stock imageUrl daysToMake }
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
        id name description price stock imageUrl imageUrlAlt daysToMake
        category { slug name }
        customizationOptions { id description additionalPrice }
      }
    }
  `, { id })
  return normalizeProduct(data.product)
}

export async function createOrder(userId, items) {
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