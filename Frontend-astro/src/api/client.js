const DEFAULT_API_URL = 'http://localhost:4000/'
const API_URL = import.meta.env.SSR
  ? process.env.API_INTERNAL_URL || import.meta.env.PUBLIC_API_URL || DEFAULT_API_URL
  : import.meta.env.PUBLIC_API_URL || DEFAULT_API_URL

export async function gql(query, variables = {}, headers = {}) {
  const res = await fetch(API_URL, {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      ...headers 
    },
    body: JSON.stringify({ query, variables }),
  })
  const { data, errors } = await res.json()
  if (errors) throw new Error(errors[0].message)
  return data
}

export function getGraphQLWebSocketUrl() {
  const url = new URL(API_URL, window.location.href)
  url.protocol = url.protocol === 'https:' ? 'wss:' : 'ws:'
  return url.toString()
}