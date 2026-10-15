export const GRAPHQL_URL = import.meta.env.PUBLIC_GRAPHQL_URL || 'http://localhost:14001/graphql'
export const API_URL = import.meta.env.PUBLIC_API_URL || 'http://localhost:14001'
export const AUTH_URL = import.meta.env.PUBLIC_AUTH_URL || API_URL

export const ORDER_STATUSES = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled']
