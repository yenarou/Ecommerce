import { gql } from './client'
import { useAuth } from '../stores/auth'

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
          product {
            id
            name
            price
            currency
          }
          customization {
            description
            isWrap
          }
        }
      }
    }
  `

  const data = await gql(query, {}, { Authorization: `Bearer ${token}` })
  return data.orders
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

export async function payOrder({ orderId, paymentOption, payer, card }) {
  const mutation = `
    mutation PayOrder($request: PayOrderRequestInput!) {
      payOrder(request: $request) {
        orderId
        status
        statusDetail
        ticketUrl
        reference
        paid
      }
    }
  `

  const token = useAuth.getState().user?.token
  const headers = token ? { Authorization: `Bearer ${token}` } : {}

  return gql(
    mutation,
    { request: { orderId, paymentOption, payer, card: card ?? null } },
    headers
  )
}
