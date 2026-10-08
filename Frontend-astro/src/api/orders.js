import { gql } from './client'
import { useAuth } from '../stores/auth'

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

export async function startPayment(orderId, paymentOption) {
  const mutation = `
    mutation CreatePaymentPreference($request: CreatePaymentPreferenceRequestInput!) {
      createPaymentPreference(request: $request) {
        initPoint
      }
    }
  `

  const token = useAuth.getState().user?.token
  const headers = token ? { Authorization: `Bearer ${token}` } : {}

  return gql(mutation, { request: { orderId, paymentOption } }, headers)
}
