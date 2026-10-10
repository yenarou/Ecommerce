import { GRAPHQL_URL } from '../lib/config'
import { getToken } from '../lib/auth'

export async function gql(query, variables = {}) {
  const token = getToken()
  const headers = { 'Content-Type': 'application/json' }
  if (token) headers.Authorization = `Bearer ${token}`

  const response = await fetch(GRAPHQL_URL, {
    method: 'POST',
    headers,
    body: JSON.stringify({ query, variables }),
  })

  let payload
  try {
    payload = await response.json()
  } catch {
    throw new Error(`El servidor respondió con HTTP ${response.status}.`)
  }

  if (payload.errors?.length) {
    throw new Error(
      payload.errors
        .map(e => {
          const detail = e.extensions?.exception?.message
          return detail ? `${e.message}: ${detail}` : e.message
        })
        .join('\n')
    )
  }

  if (!response.ok) {
    throw new Error(payload?.message || `HTTP ${response.status}`)
  }

  /*if (!response.ok) throw new Error(payload?.message || `HTTP ${response.status}`)
  if (payload.errors?.length) throw new Error(payload.errors.map(e => e.message).join('\n'))*/
  return payload.data
}
