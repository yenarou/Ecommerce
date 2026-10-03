const DEFAULT_API_URL = 'http://localhost:4000/'
const API_URL = import.meta.env.SSR
  ? process.env.API_INTERNAL_URL || import.meta.env.PUBLIC_API_URL || DEFAULT_API_URL
  : import.meta.env.PUBLIC_API_URL || DEFAULT_API_URL

export async function gql(query, variables = {}) {
  const res = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ query, variables }),
  })
  const { data, errors } = await res.json()
  if (errors) throw new Error(errors[0].message)
  return data
}