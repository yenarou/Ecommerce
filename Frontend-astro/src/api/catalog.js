  import { gql } from './client'

function normalizeImage(images) {
  if (!images || images.length === 0) {
    return {
      image_url: null,
      image_url_alt: null,
    }
  }

  const mainImage = images[0]

  return {
    image_url: mainImage.url,
    image_url_alt: mainImage.alt,
  }
}

function normalizeProduct(p) {
  if (!p) return null

  const image = normalizeImage(p.images)

  return {
    id: p.id,
    name: p.name,
    description: p.description,
    price: p.price,
    currency: p.currency,
    stock: p.stock,
    image_url: image.image_url,
    image_url_alt: image.image_url_alt,
    category: p.categoryName
      ? {
          id: p.categoryId,
          name: p.categoryName,
        }
      : null,
  }
}

function normalizeCategory(c) {
  if (!c) return null

  return {
    id: c.id,
    slug: c.slug,
    name: c.name,
    description: c.description,
    products: c.products
      ? c.products.map(normalizeProduct)
      : undefined,
  }
}

export async function fetchCategories() {
  const data = await gql(`
    query {
      categories {
        slug
        name
        description
      }
    }
  `)

/*
products {
          id
          name
          description
          price
          currency
          stock
          categoryId
          categoryName
          images {
            id
            url
            alt
          }
        } */

  return data.categories.map(normalizeCategory)
}

export async function fetchCategoryBySlug(slug) {
  const data = await gql(`
    query($slug: String!) {
      category(slug: $slug) {
        slug
        name
        description
      }
    }
  `, { slug })

  /*
  products {
          id
          name
          description
          price
          currency
          stock
          categoryId
          categoryName
          images {
            id
            url
            alt
          }
        } */

  return normalizeCategory(data.category)
}

export async function fetchProducts({
  page = 1,
  pageSize = 12,
  categorySlug,
} = {}) {
  const data = await gql(`
    query($page: Int, $pageSize: Int, $categorySlug: String) {
      products(
        page: $page,
        pageSize: $pageSize,
        categorySlug: $categorySlug
      ) {
        total
        totalPages
        page
        items {
          id
          name
          description
          price
          currency
          stock
          categoryId
          categoryName
          images {
            id
            url
            alt
          }
        }
      }
    }
  `, {
    page,
    pageSize,
    categorySlug,
  })

  return {
    ...data.products,
    items: data.products.items.map(normalizeProduct),
  }
}

export async function fetchProductById(id) {
  const data = await gql(`
    query($id: UUID!) {
      product(id: $id) {
        id
        name
        description
        price
        currency
        stock
        createdAt
        categoryId
        categoryName
        images {
          id
          url
          alt
        }
      }
    }
  `, { id })

  return normalizeProduct(data.product)
}

const USE_MOCK_ORDERS = true

export async function createOrder(userId, items) {
  if (USE_MOCK_ORDERS) {
    return {
      id: `pedido-${Date.now()}`,
      status: 'pending',
      total: 0,
      items: [],
    }
  }

  const data = await gql(`
    mutation($input: CreateOrderInput!) {
      createOrder(input: $input) {
        id
        status
        total
        items {
          quantity
          unitPrice
          product {
            name
          }
        }
      }
    }
  `, {
    input: {
      userId,
      items,
    },
  })

  return data.createOrder
}