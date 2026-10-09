import { gql } from './client'
import { useAuth } from '../stores/auth'
import { fetchProductById } from './catalog'

export async function getOrders(token = useAuth.getState().user?.token) {
  if (!token) {
    throw new Error('Debes iniciar sesión para consultar tus órdenes')
  }

  const query = `
    query Orders {
      orders {
        id
        createdAt
        updatedAt
        address {
          street
          city
          state
          zipCode
          country
        }
        status {
          value
        }
        items {
          id
          productId
          quantity
        }
      }
    }
  `

  const data = await gql(query, {}, { Authorization: `Bearer ${token}` })
  const productIds = [...new Set(
    data.orders.flatMap((order) => order.items.map((item) => item.productId)),
  )]
  const products = await Promise.all(
    productIds.map(async (productId) => [productId, await fetchProductById(productId)]),
  )
  const productsById = new Map(products)

  return data.orders.map((order) => ({
    ...order,
    items: order.items.map((item) => ({
      ...item,
      product: productsById.get(item.productId),
    })),
  }))
}

export async function createOrder(address) {
  const mutation = `
    mutation CreateOrder($request: CreateOrderRequestInput!) {
      createOrder(request: $request) {
        id
        total
      }
    }
  `

  const variables = {
    request: {
      address: {
        street: address.street,
        city: address.city,
        state: address.state,
        zipCode: address.zipCode,
        country: address.country
      }
    }
  }

  const token = useAuth.getState().user?.token
  const headers = {}
  
  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  return gql(mutation, variables, headers)
}
