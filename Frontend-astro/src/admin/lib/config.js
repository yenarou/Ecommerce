const GRAPHQL_ENV = import.meta.env.PUBLIC_API_URL || 'http://localhost:14001/graphql'

export const GRAPHQL_URL = GRAPHQL_ENV
export const API_URL = GRAPHQL_ENV.replace(/\/graphql\/?$/, '')

export const ORDER_STATUSES = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled']